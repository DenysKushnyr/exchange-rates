using API.DTO;
using Application.DTO;
using Application.Services;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class ExchangeController : ControllerBase
{
    private NbuService _nbu;
    private PrivatService _privat;
    private MonoService _mono;

    public ExchangeController(NbuService nbu, PrivatService privat,
        MonoService mono)
    {
        _nbu = nbu;
        _privat = privat;
        _mono = mono;
    }

    [HttpGet("allRates")]
    [OutputCache(Duration = 600)]

    public async Task<IActionResult> GetAllRates()
    {
        var nbuRates = await _nbu.GetCurrentRates();
        var monoRates = await _mono.GetCurrentRates();
        var privatRates = await _privat.GetCurrentRates();

        var allCurrencies = nbuRates
            .Select(nbu => nbu.CurrencyCode)
            .Union(monoRates.Select(mono => mono.CurrencyCode))
            .Union(privatRates.Select(privat => privat.CurrencyCode))
            .Distinct();

        var result = allCurrencies
            .Select(currency => new RatesResponse
            {
                CurrencyCode = currency,
                NbuRate = nbuRates.FirstOrDefault(nbu => nbu.CurrencyCode == currency,
                    new CurrencyResponse { Rate = 0 }).Rate,
                MonoRate = monoRates.FirstOrDefault(mono => mono.CurrencyCode == currency,
                    new CurrencyResponse { Rate = 0 }).Rate,
                PrivatRate = privatRates.FirstOrDefault(privat => privat.CurrencyCode == currency,
                    new CurrencyResponse { Rate = 0 }).Rate
            });

        return Ok(result);
    }

    [HttpGet("currencyPriceHistory/{currency}")]
    [OutputCache(Duration = 600)]
    public async Task<IActionResult> GetCurrencyRateHistory([FromRoute] CurrencyCode currency,
        [FromQuery] string period)
    {
        try
        {
            CurrencyHistoryPeriod periodEnum = period switch
            {
                "1w" => CurrencyHistoryPeriod.Week,
                "1m" => CurrencyHistoryPeriod.Month,
                "1y" => CurrencyHistoryPeriod.OneYear,
                "5y" => CurrencyHistoryPeriod.FiveYears,
                _ => throw new ArgumentException("Invalid period value", nameof(period))
            };

            var rates = await _nbu.GetHistoryRates(currency, periodEnum);
            return Ok(rates);
            
        }
        catch
        {
            return BadRequest();
        }
        return Ok();
    }

}