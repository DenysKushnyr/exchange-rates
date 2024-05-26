using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTO;
using Asp.Versioning;
using DAL;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly HttpContext _httpContext;
    
    public AuthController(AppDbContext db, IHttpContextAccessor context)
    {
        _db = db;
        _httpContext = context.HttpContext!;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(UserAuthRequest user)
    {
        var errorMessage = CheckUser(user);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return BadRequest(new ErrorMessage(errorMessage));
        }

        var dbUser = await _db.Users.Include(user => user.Subscriptions)
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.PasswordHash))
        {
            return BadRequest(new ErrorMessage("Неправильний логін або пароль"));
        }
        
        var token = GenerateJwtToken(dbUser.Id, user.Email);
        var userSubs = dbUser.Subscriptions.Select(u => u.CurrencyCode);
        return Ok(new {Token = token, Subs = userSubs});
}
    
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Register(UserAuthRequest user)
    {
        var errorMessage = CheckUser(user); 
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return BadRequest(new ErrorMessage(errorMessage));
        }

        var checkDbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

        if (checkDbUser != null)
        {
            return BadRequest(new ErrorMessage("Користувач з такою поштою вже існує"));
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var dbUser = await _db.Users.AddAsync(new User(user.Email, passwordHash));
        
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return BadRequest(new ErrorMessage("Щось пішло не так, спробуйте пізніше"));
        }

        var token = GenerateJwtToken(dbUser.Entity.Id, user.Email);

        return Ok(new {Token = token});
    }

    
    
    private string GenerateJwtToken(int id, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(Constants.DevJwtSecretKey)), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);;
    }

    private string CheckUser(UserAuthRequest user)
    {
        if (string.IsNullOrWhiteSpace(user.Email) || 
            string.IsNullOrWhiteSpace(user.Password))
        {
            return "Логін або пароль не може бути порожнім";
        }

        if (_httpContext.User.FindFirstValue(ClaimTypes.Email) != null)
        {
            return "Ви вже зареєстровані";
        }

        return string.Empty;
    }
}