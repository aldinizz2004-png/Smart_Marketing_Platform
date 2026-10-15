using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

public class BusinessHour : BusinessEntity
{
    // 0 = Sunday ... 6 = Saturday
    public byte Weekday { get; set; }

    // Allows split shifts (e.g. 09:00-13:00 and 16:00-22:00).
    public byte SlotNo { get; set; }

    public TimeOnly OpensAt { get; set; }

    public TimeOnly ClosesAt { get; set; }

    public bool CrossesMidnight { get; set; }
}
