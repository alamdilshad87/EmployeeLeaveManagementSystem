# 🏢 Employee Leave Management System

An **ASP.NET Core Web API**–based Employee Leave Management System that demonstrates real-world CRUD operations, business rules, role-based access, LINQ reporting, and REST API best practices.

---

## 📋 Project Structure

```
EmployeeLeaveManagementSystem/
├── Controllers/
│   └── LeaveController.cs          # API endpoints
├── Data/
│   └── AppDbContext.cs              # Entity Framework context
├── Models/
│   ├── Employee.cs                  # Employee entity
│   └── LeaveRequest.cs              # Leave request entity
├── DTOs/
│   ├── ApplyLeaveDto.cs             # Create leave DTO
│   ├── UpdateLeaveStatusDto.cs      # Update leave status DTO
│   └── LeaveRequestDto.cs           # Response DTO
├── Migrations/                       # EF Core migrations
├── Program.cs                        # Configuration & startup
└── appsettings.json                 # Database & settings
```
---

# Employee Leave Management System – CRUD Operation (Medium Level)

## STEP 1: Apply Leave (CREATE)
Employee submits a leave request. The system should:
- Validate employee exists and is active
- Ensure FromDate is before ToDate
- Insert leave request with status Pending

## STEP 2: View Leave Requests (READ)
Leave requests should be viewed based on roles:
- Manager views all Pending leave requests
- HR can view all leave requests
- Filter leave requests by Employee
- Filter leave requests by Status
- Filter leave requests by Date Range

## STEP 3: Approve / Reject Leave (UPDATE)
- Manager approves or rejects a leave request
- If approved, status changes to Approved
- If rejected, status changes to Rejected
- Once approved, leave request cannot be modified again

## STEP 4: Cancel Leave (DELETE – Soft Delete)
- Employee can cancel leave only if status is Pending
- Leave request should not be deleted from the database
- Mark the leave request status as Cancelled

## STEP 5: Business Rules & Validation
- Employee cannot apply for leave if already on leave
- Overlapping leave dates are not allowed
- Only managers are allowed to approve or reject leave requests

## STEP 6: Reporting (READ + LINQ)
- HR can view total leaves taken per employee
- HR can view Approved vs Rejected leave statistics
- LINQ should be used to group and summarize leave data
---
