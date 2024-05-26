using Domain.Enums;

namespace Application.DTO;

public class MonoCurrencyResponse
{
    public CurrencyCode CurrencyCodeA { get; set; }
    public CurrencyCode CurrencyCodeB { get; set; }
    public decimal RateBuy { get; set; }
    public decimal RateSell { get; set; }
    public decimal RateCross { get; set; }
    public long Date { get; set; }
}