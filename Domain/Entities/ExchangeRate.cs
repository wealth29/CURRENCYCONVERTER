namespace CurrencyConverter.Domain.Entities
{
    public class ExchangeRate
    {
        public int Id { get; private set; }
        public Currency BaseCurrency { get; private set; }
        public Currency TargetCurrency { get; private set; }
        public decimal Rate { get; private set; }
        public DateTime TimestampUtc { get; private set; }

       
    }}