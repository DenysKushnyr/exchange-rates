using Domain.Enums;

namespace Domain.Entities;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
}