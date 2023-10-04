using System.Runtime.Serialization;

namespace Spenses.Application.Models;

public enum SortDirection
{
    [EnumMember(Value = @"asc")]
    Asc = 0,

    [EnumMember(Value = @"desc")]
    Desc = 1
}
