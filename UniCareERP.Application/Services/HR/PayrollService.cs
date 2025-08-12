using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.HR;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.HR
{
    public class PayrollService : IPayrollService
    {
        private readonly UniCareDbContext _context;
        private readonly IGeneralLedgerService _generalLedgerService;

        public PayrollService(UniCareDbContext context, IGeneralLedgerService generalLedgerService)
        {
            _context = context;
            _generalLedgerService = generalLedgerService;
        }

        public async Task<PayrollDto> GeneratePayrollAsync(Guid employeeId, DateTime payPeriodStartDate, DateTime payPeriodEndDate)
        {
            var employee = await _context.Employees
                .Include(e => e.SalaryStructure)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null || employee.SalaryStructure == null)
            {
                return null;
            }

            var salaryStructure = employee.SalaryStructure;
            var grossSalary = salaryStructure.BasicSalary +
                              salaryStructure.HouseRentAllowance +
                              salaryStructure.ConveyanceAllowance +
                              salaryStructure.MedicalAllowance;

            var totalDeductions = salaryStructure.ProvidentFund +
                                  salaryStructure.ProfessionalTax;

            var netSalary = grossSalary - totalDeductions;

            var payroll = new Payroll
            {
                EmployeeId = employeeId,
                PayPeriodStartDate = payPeriodStartDate,
                PayPeriodEndDate = payPeriodEndDate,
                GrossSalary = grossSalary,
                NetSalary = netSalary,
                TotalDeductions = totalDeductions
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            // Post to general ledger
            var salariesPayableAccount = await _context.GeneralLedgerAccounts.FirstOrDefaultAsync(a => a.AccountName == "Salaries Payable");
            var salariesExpenseAccount = await _context.GeneralLedgerAccounts.FirstOrDefaultAsync(a => a.AccountName == "Salaries Expense");

            if (salariesPayableAccount != null && salariesExpenseAccount != null)
            {
                await _generalLedgerService.PostTransactionAsync(salariesPayableAccount, salariesExpenseAccount, netSalary, $"Payroll for {employee.FirstName} {employee.LastName}");
            }

            return new PayrollDto
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                PayPeriodStartDate = payroll.PayPeriodStartDate,
                PayPeriodEndDate = payroll.PayPeriodEndDate,
                GrossSalary = payroll.GrossSalary,
                NetSalary = payroll.NetSalary,
                TotalDeductions = payroll.TotalDeductions
            };
        }

        public async Task<PayrollDto> GetPayrollByIdAsync(Guid payrollId)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.Payslips)
                .FirstOrDefaultAsync(p => p.Id == payrollId);

            if (payroll == null)
            {
                return null;
            }

            return new PayrollDto
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                EmployeeName = $"{payroll.Employee.FirstName} {payroll.Employee.LastName}",
                PayPeriodStartDate = payroll.PayPeriodStartDate,
                PayPeriodEndDate = payroll.PayPeriodEndDate,
                GrossSalary = payroll.GrossSalary,
                NetSalary = payroll.NetSalary,
                TotalDeductions = payroll.TotalDeductions,
                Payslips = payroll.Payslips.Select(ps => new PayslipDto
                {
                    Id = ps.Id,
                    PayrollId = ps.PayrollId,
                    Description = ps.Description,
                    Amount = ps.Amount
                }).ToList()
            };
        }

        public async Task<IEnumerable<PayrollDto>> GetPayrollsForEmployeeAsync(Guid employeeId)
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                .Where(p => p.EmployeeId == employeeId)
                .ToListAsync();

            return payrolls.Select(payroll => new PayrollDto
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId,
                EmployeeName = $"{payroll.Employee.FirstName} {payroll.Employee.LastName}",
                PayPeriodStartDate = payroll.PayPeriodStartDate,
                PayPeriodEndDate = payroll.PayPeriodEndDate,
                GrossSalary = payroll.GrossSalary,
                NetSalary = payroll.NetSalary,
                TotalDeductions = payroll.TotalDeductions
            });
        }
    }
}
