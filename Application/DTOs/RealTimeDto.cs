namespace CurrencyConverter.Application.DTOs;

public class RealTimeDto
{
    public string Base { get; set; }
    public DateTime Date { get; set; }
    public IReadOnlyDictionary<string, decimal> Rates { get; set; }
}

public class HistoricalDto
{
    public string Base { get; set; }
    public string Target { get; set; }
    public IReadOnlyDictionary<DateTime, decimal> Rates { get; set; }
}

public class ConvertRequest
{
    public string Base { get; set; }
    public string Target { get; set; }
    public decimal Amount { get; set; }
}

public class HistoricalConvertRequest
{
    public string Base { get; set; }
    public string Target { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}

public class HistoryQuery
{
    public string Base { get; set; }
    public string Target { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
