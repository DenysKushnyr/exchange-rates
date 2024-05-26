using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    public User()
    {
        CreationTime = DateTime.Now;
    }
    public User(string userEmail, string passwordHash)
    {
        Email = userEmail;
        PasswordHash = passwordHash;
        CreationTime = DateTime.UtcNow;
    }

    public int Id { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!;
    [Required]
    public DateTime CreationTime { get; set; }
    public List<Subscription> Subscriptions { get; set; }
}