using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Spenses.Resources.Relational.Infrastructure;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
    {
    }
}

public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
{
    public NullableDateOnlyConverter()
        : base(
            d => d == null ? null : new DateTime?(d.Value.ToDateTime(TimeOnly.MinValue)),
            d => d == null ? null : new DateOnly?(DateOnly.FromDateTime(d.Value)))
    {
    }
}
