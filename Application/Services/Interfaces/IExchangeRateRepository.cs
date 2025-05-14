namespace CurrencyConverter.Domain.Interfaces;

public interface IExchangeRateRepository
{
    Task<ExchangeRate?> GetLatestAsync(Currency baseCurrency, Currency targetCurrency);
    Task<IEnumerable<HistoricalRate>> GetHistoryAsync(Currency baseCurrency, Currency targetCurrency, DateTime from, DateTime to);
    Task AddOrUpdateAsync(params HistoricalRate[] rates);
}