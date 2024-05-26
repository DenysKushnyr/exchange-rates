using Application.DTO;
using Domain.Enums;

namespace API.DTO;

public class RatesResponse
{
    public CurrencyCode CurrencyCode { get; set; }
    public decimal NbuRate { get; set; }
    public decimal MonoRate { get; set; }
    public decimal PrivatRate { get; set; }
}