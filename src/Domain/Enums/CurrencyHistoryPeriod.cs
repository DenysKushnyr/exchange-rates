using System.Runtime.Serialization;

namespace Domain.Enums;

public enum CurrencyHistoryPeriod
{
    [EnumMember(Value = "1w")]
    Week,
    [EnumMember(Value = "1m")]
    Month,
    [EnumMember(Value = "1y")]
    OneYear,
    [EnumMember(Value = "5y")]
    FiveYears
}