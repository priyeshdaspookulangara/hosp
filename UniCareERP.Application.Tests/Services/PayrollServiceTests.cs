using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.HR;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Infrastructure.Data;
using Xunit;

namespace UniCareERP.Application.Tests.Services
{
    public class PayrollServiceTests
    {
        [Fact]
        public async Task GeneratePayrollAsync_Should_GeneratePayroll()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: "UniCareERP_Test")
                .Options;

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                SalaryStructure = new SalaryStructure
                {
                    BasicSalary = 50000,
                    HouseRentAllowance = 20000,
                    ConveyanceAllowance = 5000,
                    MedicalAllowance = 5000,
                    ProvidentFund = 5000,
                    ProfessionalTax = 200
                }
            };

            var generalLedgerServiceMock = new Mock<IGeneralLedgerService>();

            using (var context = new UniCareDbContext(options))
            {
                context.Employees.Add(employee);
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(options))
            {
                var payrollService = new PayrollService(context, generalLedgerServiceMock.Object);

                // Act
                var payroll = await payrollService.GeneratePayrollAsync(employee.Id, new DateTime(2023, 1, 1), new DateTime(2023, 1, 31));

                // Assert
                Assert.NotNull(payroll);
                Assert.Equal(80000, payroll.GrossSalary);
                Assert.Equal(5200, payroll.TotalDeductions);
                Assert.Equal(74800, payroll.NetSalary);
            }
        }
    }
}
