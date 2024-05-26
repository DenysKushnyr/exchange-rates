using Domain.Enums;

namespace Application.DTO;

public class CurrencyResponse
{
    public CurrencyCode CurrencyCode { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
    
    public CurrencyResponse()
    {
        
    }
    public CurrencyResponse(NbuCurrencyResponse nbu)
    {
        CurrencyCode = nbu.CurrencyCode;
        Rate = nbu.Rate;
        Date = DateTime.Today;
    }
    
    public CurrencyResponse(PrivatCurrencyResponse privat)
    {
        CurrencyCode = privat.Ccy;
        Rate = privat.Buy;
        Date = DateTime.Today;
    }
    
    public CurrencyResponse(MonoCurrencyResponse mono)
    {
        CurrencyCode = mono.CurrencyCodeA;
        Rate = mono.RateBuy != 0 ? mono.RateBuy : mono.RateCross;
        // Date = DateTime.Today;
    }
}