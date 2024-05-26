using System.Net.Http.Json;
using Application.DTO;

namespace Application.Services;

public class PrivatService(HttpClient httpClient)
{
    private readonly string _currentRatesUrl = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";

    public async Task<IEnumerable<CurrencyResponse>> GetCurrentRates()
    {
        try
        {
            var ratesResponse = await httpClient.GetAsync(_currentRatesUrl);
            var privatRates = await ratesResponse
                .Content.ReadFromJsonAsync<IEnumerable<PrivatCurrencyResponse>>() ?? [];
            var rates = privatRates.Select(r => new CurrencyResponse(r));
            
           
            return rates;
        }
        catch
        {
            return [];
        }
    }
}