using System.Text.Json.Serialization;
using API.Converters;

namespace API.DTO;

public class HistoryRatesResponse
{
    public decimal Rate { get; set; }
    [JsonConverter(typeof(NbuDateTimeConverter))]
    public DateTime Date { get; set; }
}