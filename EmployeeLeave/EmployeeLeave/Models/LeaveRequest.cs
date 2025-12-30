namespace EmployeeLeave.Models
{
	public class LeaveRequest
	{
		public int LeaveRequestId { get; set; }
		public int EmployeeId { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public string Status { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime AppliedOn { get; set; }
	}
}