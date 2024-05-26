using System.Net.Http.Json;
using Application.DTO;
using Domain.Enums;

namespace Application.Services;

public class MonoService(HttpClient httpClient)
{
    private readonly string _currentRatesUrl = "https://api.monobank.ua/bank/currency";

    public async Task<IEnumerable<CurrencyResponse>> GetCurrentRates()
    {
        try
        {
            var ratesResponse = await httpClient.GetAsync(_currentRatesUrl);
            var monoRates = await ratesResponse
                .Content.ReadFromJsonAsync<IEnumerable<MonoCurrencyResponse>>() ?? [];
            
            var rates = monoRates
                .Where(r => r.CurrencyCodeB == CurrencyCode.UAH)
                .Select(r => new CurrencyResponse(r));
            
            return rates;
        }
        catch
        {
            return [];
        }
    }
}