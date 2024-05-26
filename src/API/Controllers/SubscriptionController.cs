using System.Security.Claims;
using API.DTO;
using Application.Interfaces;
using Asp.Versioning;
using DAL;
using Domain.Entities;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly HttpContext _httpContext;
    private readonly IEmailSender _email;
    
    public SubscriptionController(AppDbContext db, IHttpContextAccessor context, IEmailSender email)
    {
        _db = db;
        _email = email;
        _httpContext = context.HttpContext!;
    }
    
    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> Subscribe(CurrencyCode currencyCode, DateTime time)
    {
        var email = _httpContext.User.FindFirstValue(ClaimTypes.Email);
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userId) )
        {
            return BadRequest();
        }
        
        RecurringJob.AddOrUpdate($"{email}-{currencyCode}", 
            () => _email.SendRateAsync(email, currencyCode), 
            Cron.Daily(time.Hour, time.Minute), TimeZoneInfo.Local);

        _db.Subscriptions.Add(new Subscription
        {
            UserId = Int32.Parse(userId),
            CurrencyCode = currencyCode
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new ErrorMessage("Щось пішло не так, спробуйте пізніше"));
        }

        return Ok();
    }
    
    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> Unsubscribe(CurrencyCode currencyCode)
    {
        var email = _httpContext.User.FindFirstValue(ClaimTypes.Email);
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userId) )
        {
            return BadRequest();
        }
        
        RecurringJob.RemoveIfExists($"{email}-{currencyCode}");

        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == int.Parse(userId) &&
                                                   s.CurrencyCode == currencyCode);

        if (sub == null)
        {
            return BadRequest(new ErrorMessage("Ви вже не отримуєте повідомлення про поточну валюту"));
        }

        _db.Remove(sub);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new ErrorMessage("Щось пішло не так, спробуйте пізніше"));
        }

        return Ok();
    }
}