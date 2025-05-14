namespace CurrencyConverter.Domain.ValueObjects;

public record Currency(string Code)
{
    public static bool IsValid(string code) => code.Length == 3 && code.All(char.IsLetter);
}