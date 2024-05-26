using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class UserAuthRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}