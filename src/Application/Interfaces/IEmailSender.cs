using Domain.Enums;

namespace Application.Interfaces;

public interface IEmailSender
{
    Task SendRateAsync(string toEmail, CurrencyCode currencyCode);
}