using System.Collections;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using API.DTO;
using Application.DTO;
using Domain.Enums;

namespace Application.Services;

public class NbuService(HttpClient httpClient)
{
    private readonly string _currentRatesUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
    private readonly string _historyPricesUrl = "https://bank.gov.ua/NBU_Exchange/exchange_site?json&sort=exchangedate&order=asc";

    public async Task<IEnumerable<CurrencyResponse>> GetCurrentRates()
    {
        try
        {
            var ratesResponse = await httpClient.GetAsync(_currentRatesUrl);
            var nbuRates = await ratesResponse
                .Content.ReadFromJsonAsync<IEnumerable<NbuCurrencyResponse>>() ?? [];
            
            var rates = nbuRates.Select(r => new CurrencyResponse(r));

            return rates;
        }
        catch
        {
            return [];
        }
    }

    public async Task<IEnumerable<HistoryRatesResponse>> GetHistoryRates(CurrencyCode currency, CurrencyHistoryPeriod periodEnum)
    {
        var today = DateTime.Now;
        DateTime pastDate;
        
        try
        {
            switch (periodEnum)
            {
                case CurrencyHistoryPeriod.Week:
                    pastDate = today.AddDays(-6);
                    break;
                case CurrencyHistoryPeriod.Month:
                    pastDate = today.AddMonths(-1);
                    break;
                case CurrencyHistoryPeriod.OneYear:
                    pastDate = today.AddYears(-1);
                    break;
                case CurrencyHistoryPeriod.FiveYears:
                    pastDate = today.AddYears(-5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(periodEnum), periodEnum, null);
            }
        
            var todayString = today.ToString("yyyyMMdd");
            var pastDateString = pastDate.ToString("yyyyMMdd");

            
            var ratesResponse = await httpClient.GetAsync($"{_historyPricesUrl}" +
                                                          $"&start={pastDateString}" +
                                                          $"&end={todayString}" +
                                                          $"&valcode={currency.ToString()}");
            
            var nbuRates = await ratesResponse
                .Content.ReadFromJsonAsync<IEnumerable<NbuCurrencyResponse>>() ?? [];
            var result = nbuRates.Select(r => 
                new HistoryRatesResponse()
            {
                Rate = r.Rate,
                Date = r.Date
            });
            
            return result;
        }
        catch
        {
            return [];
        }
    }
}