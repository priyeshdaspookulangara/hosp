namespace UniCareERP.Domain.Enums
{
    public enum LeaveType
    {
        Annual = 1,
        Sick = 2,
        Unpaid = 3,
        Maternity = 4,
        Paternity = 5,
        Other = 6
    }

    public enum LeaveRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
