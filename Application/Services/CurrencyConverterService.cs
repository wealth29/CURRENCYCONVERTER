public class CurrencyConverterService : ICurrencyConverterService
{
    private readonly IExchangeRateRepository _repo;
    private readonly IExternalExchangeRateClient _external;
    private readonly ILogger<CurrencyConverterService> _logger;

    public CurrencyConverterService(
        IExchangeRateRepository repo,
        IExternalExchangeRateClient external,
        ILogger<CurrencyConverterService> logger)
    {
        _repo = repo;
        _external = external;
        _logger = logger;
    }

    public async Task<decimal> ConvertAsync(ConvertRequest req)
    {
        var baseCur = new Currency(req.Base);
        var targetCur = new Currency(req.Target);

        var latest = await _repo.GetLatestAsync(baseCur, targetCur)
                     ?? await FetchAndStoreAsync(baseCur, targetCur);

        return req.Amount * latest.Rate;
    }

    public async Task<decimal> ConvertHistoricalAsync(HistoricalConvertRequest request)
    {
        var baseCur = new Currency(request.Base);
        var targetCur = new Currency(request.Target);

        var history = await _repo.GetHistoryAsync(baseCur, targetCur, request.Date, request.Date);

        var rate = history.FirstOrDefault()?.Rate;

        if (rate == 0)
        {
            _logger.LogInformation("Fetching historical rate from external for {Date}", request.Date);

            var dto = await _external.FetchHistoricalAsync(baseCur, targetCur, request.Date, request.Date);
            if (!dto.Rates.TryGetValue(request.Date, out rate))
                throw new InvalidOperationException("Rate not found for specified date.");

            var record = new HistoricalRate(baseCur, targetCur, request.Date, rate);
            await _repo.AddOrUpdateAsync(record);
        }

        return request.Amount * rate;
    }

    public async Task<IEnumerable<(DateTime Date, decimal Rate)>> GetHistoryAsync(HistoryQuery query)
    {
        var baseCur = new Currency(query.Base);
        var targetCur = new Currency(query.Target);

        var history = await _repo.GetHistoryAsync(baseCur, targetCur, query.From, query.To);

        var expectedDates = Enumerable.Range(0, (query.To - query.From).Days + 1)
            .Select(offset => query.From.AddDays(offset))
            .ToHashSet();

        var fetchedDates = history.Select(h => h.Date).ToHashSet();

        var missingDates = expectedDates.Except(fetchedDates).ToList();

        if (missingDates.Any())
        {
            var dto = await _external.FetchHistoricalAsync(baseCur, targetCur, query.From, query.To);
            var newRates = dto.Rates
                .Where(kvp => missingDates.Contains(kvp.Key))
                .Select(kvp => new HistoricalRate(baseCur, targetCur, kvp.Key, kvp.Value))
                .ToArray();

            await _repo.AddOrUpdateAsync(newRates);

            history = history.Concat(newRates);
        }

        return history
            .OrderBy(h => h.Date)
            .Select(h => (h.Date, h.Rate));
    }

    private async Task<ExchangeRate> FetchAndStoreAsync(Currency baseCur, Currency targetCur)
    {
        var dto = await _external.FetchRealTimeAsync(baseCur);
        if (!dto.Rates.TryGetValue(targetCur.Code, out var rate))
            throw new InvalidOperationException($"Rate not found for {baseCur} to {targetCur}");

        var record = new ExchangeRate(baseCur, targetCur, rate, dto.Date);
        await _repo.AddOrUpdateAsync(record);

        return record;
    }
}
