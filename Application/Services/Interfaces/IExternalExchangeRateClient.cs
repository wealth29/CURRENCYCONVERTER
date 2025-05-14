namespace CurrencyConverter.Domain.Interfaces;



public interface IExternalExchangeRateClient
{
    Task<RealTimeDto> FetchRealTimeAsync(Currency baseCurrency, CancellationToken ct = default);
    Task<HistoricalDto> FetchHistoricalAsync(Currency baseCurrency, Currency targetCurrency, DateTime from, DateTime to, CancellationToken ct = default);
}