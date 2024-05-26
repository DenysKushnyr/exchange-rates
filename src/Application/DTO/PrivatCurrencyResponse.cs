using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.DTO;

public class PrivatCurrencyResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyCode Ccy { get; set; }
    public decimal Buy { get; set; }
}