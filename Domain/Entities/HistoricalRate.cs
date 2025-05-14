namespace CurrencyConverter.Domain.Entities
{

    public class HistoricalRate
    {
        public int Id { get; private set; }
        
        public Currency BaseCurrency { get; private set; }
        public Currency TargetCurrency { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Rate { get; private set; }
    }
}