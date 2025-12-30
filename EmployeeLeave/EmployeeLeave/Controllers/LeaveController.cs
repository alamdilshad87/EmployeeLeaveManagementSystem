using EmployeeLeave.Data;
using EmployeeLeave.DTOs;
using EmployeeLeave.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EmployeeLeave.Controllers
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

            bool alreadyOnLeave = _context.LeaveRequests.Any(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.Status == "Approved" &&
                DateTime.Now >= l.FromDate &&
                DateTime.Now <= l.ToDate);

            if (alreadyOnLeave)
                return BadRequest("Employee is already on leave");

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

        [HttpGet("manager/pending")]
        public IActionResult ManagerPendingLeaves()
        {
            return Ok(_context.LeaveRequests
                .Where(l => l.Status == "Pending" && !l.IsDeleted)
                .ToList());
        }

        [HttpGet("hr/all")]
        public IActionResult HrAllLeaves()
        {
            return Ok(_context.LeaveRequests
                .Where(l => !l.IsDeleted)
                .ToList());
        }

        [HttpGet("filter")]
        public IActionResult FilterLeaves(
            int? employeeId,
            string? status,
            DateTime? from,
            DateTime? to)
        {
            var query = _context.LeaveRequests.AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(l => l.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == status);

            if (from.HasValue && to.HasValue)
                query = query.Where(l => l.FromDate >= from && l.ToDate <= to);

            return Ok(query.ToList());
        }

        [HttpPut("approve-reject/{id}")]
        public IActionResult ApproveRejectLeave(int id, UpdateLeaveStatusDto dto)
        {
            var leave = _context.LeaveRequests.Find(id);

            if (leave == null)
                return NotFound();

            if (leave.Status == "Approved")
                return BadRequest("Approved leave cannot be modified");

            leave.Status = dto.Status;
            _context.SaveChanges();

            return Ok("Leave updated");
        }
        [HttpDelete("cancel/{id}")]
        public IActionResult CancelLeave(int id)
        {
            var leave = _context.LeaveRequests.Find(id);

            if (leave == null)
                return NotFound();

            if (leave.Status != "Pending")
                return BadRequest("Only pending leaves can be cancelled");

            leave.Status = "Cancelled";
            leave.IsDeleted = true;

            _context.SaveChanges();
            return Ok("Leave cancelled");
        }


        [HttpGet("report/total-leaves")]
        public IActionResult TotalLeavesPerEmployee()
        {
            var result = _context.Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    TotalLeaves = _context.LeaveRequests
                        .Count(l => l.EmployeeId == e.EmployeeId && l.Status == "Approved")
                })
                .ToList();

            return Ok(result);
        }


        [HttpGet("report/status-summary")]
        public IActionResult LeaveStatusSummary()
        {
            var report = _context.LeaveRequests
                .GroupBy(l => l.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                });

            return Ok(report);
        }
    }
}

