using System.Text.Json.Serialization;
using API.Converters;
using Domain.Enums;

namespace Application.DTO;

public class NbuCurrencyResponse
{
    [JsonPropertyName("r030")]
    public CurrencyCode CurrencyCode { get; set; }
    public decimal Rate { get; set; }
    [JsonPropertyName("exchangedate")]
    [JsonConverter(typeof(NbuDateTimeConverter))]
    public DateTime Date { get; set; }
}