using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 465;
    private readonly string _fromEmail;
    private readonly string _password;

    public EmailSender()
    {
        _fromEmail = Environment.GetEnvironmentVariable("EXCHANGERATES_SMTP_EMAIL") ?? "";
        _password = Environment.GetEnvironmentVariable("EXCHANGERATES_SMTP_PASSWORD") ?? "";
        if (string.IsNullOrWhiteSpace(_fromEmail) || string.IsNullOrWhiteSpace(_password))
        {
            throw new Exception("Smtp email and password can't be empty");
        }
    }

    public async Task SendRateAsync(string toEmail, CurrencyCode currencyCode)
    {
        var nbu = new NbuService(new HttpClient());

        var rates = await nbu.GetCurrentRates();
        var currency = rates.First(r => r.CurrencyCode == currencyCode);

        using var mail = new MailMessage();

        mail.From = new MailAddress(_fromEmail);
        mail.To.Add(toEmail);

        mail.Subject = $"Оновлення курсу валюти {currencyCode} на " +
                       $"{DateTime.Now:dd.MM.yyyy}";
        mail.Body = mail.Body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f0f0f0;
                        padding: 20px;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        padding: 30px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }}
                    h1 {{
                        color: #333333;
                    }}
                    b {{
                        color: #0066cc;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Поточна ціна валюти {currencyCode}:</h1>
                    <br>
                    <h3><b>1 {currencyCode}</b> = <b>{$"{currency.Rate:F4}"} UAH</b></h3>
                </div>
            </body>
            </html>";
        mail.IsBodyHtml = true;

        using var client = new SmtpClient("smtp.gmail.com", 587);

        client.EnableSsl = true;

        client.Credentials = new NetworkCredential(_fromEmail, _password);

        await client.SendMailAsync(mail);
    }
}