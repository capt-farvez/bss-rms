namespace BssRms.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    PreparedToServe = 3,
    Served = 4,
    Paid = 5
}
