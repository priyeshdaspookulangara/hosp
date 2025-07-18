using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;
using Xunit;

namespace UniCareERP.Application.Tests.Services
{
    public class PatientBillingServiceTests
    {
        private readonly DbContextOptions<UniCareDbContext> _dbContextOptions;
        private readonly Mock<ILogger<PatientBillingService>> _loggerMock;

        public PatientBillingServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: "UniCareTestDb")
                .Options;
            _loggerMock = new Mock<ILogger<PatientBillingService>>();
        }

        [Fact]
        public async Task ProcessPaymentAsync_Should_CreatePaymentAndAdjustBalance()
        {
            // Arrange
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new PatientBillingService(context, _loggerMock.Object);
                var patientId = Guid.NewGuid();
                var paymentDto = new PatientPaymentDto
                {
                    PatientId = patientId,
                    PaymentDate = DateTime.UtcNow,
                    Amount = 100,
                    PaymentMethod = PaymentMethod.Cash
                };

                // Act
                var paymentId = await service.ProcessPaymentAsync(paymentDto);

                // Assert
                var payment = await context.PatientPayments.FindAsync(paymentId);
                Assert.NotNull(payment);
                Assert.Equal(100, payment.Amount);

                var account = await context.PatientAccounts.FirstOrDefaultAsync(pa => pa.PatientId == patientId);
                Assert.NotNull(account);
                Assert.Equal(100, account.TotalPayments);
                Assert.Equal(-100, account.CurrentBalance);
            }
        }
    }
}
