using EmployeeLeave.Models;
using EmployeeLeave.DTOs;
using EmployeeLeave.Data;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/leave")]
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        public IActionResult ApplyLeave(ApplyLeaveDto dto)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeId == dto.EmployeeId && e.IsActive);

            if (employee == null)
                return BadRequest("Employee not found or inactive");

            if (dto.FromDate >= dto.ToDate)
                return BadRequest("FromDate must be before ToDate");

            bool overlap = _context.LeaveRequests.Any(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.Status != "Rejected" &&
                l.Status != "Cancelled" &&
                dto.FromDate <= l.ToDate &&
                dto.ToDate >= l.FromDate);

            if (overlap)
                return BadRequest("Overlapping leave not allowed");

            var leave = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Status = "Pending",
                IsDeleted = false,
                AppliedOn = DateTime.Now
            };

            _context.LeaveRequests.Add(leave);
            _context.SaveChanges();

            return Ok("Leave applied successfully");
        }
    }
}

