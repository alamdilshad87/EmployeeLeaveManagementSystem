namespace EmployeeLeave.DTOs
{
    public class ApplyLeaveDto
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}