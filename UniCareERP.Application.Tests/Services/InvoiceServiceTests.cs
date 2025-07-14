using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class InvoiceServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Invoice>> _mockInvoiceDbSet;
        private Mock<DbSet<Patient>> _mockPatientDbSet;
        private Mock<ILogger<InvoiceService>> _mockLogger;
        private InvoiceService _invoiceService;

        private List<Patient> _seedPatients;
        private List<Invoice> _seedInvoices;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockInvoiceDbSet = new Mock<DbSet<Invoice>>();
            _mockPatientDbSet = new Mock<DbSet<Patient>>();
            _mockLogger = new Mock<ILogger<InvoiceService>>();

            // Seed Data
            _seedPatients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FirstName = "Bill", LastName = "Payer", PatientCode = "P00200" }
            };
            _seedInvoices = new List<Invoice>();

            // Setup DbSets
            SetupMockDbSet(_mockInvoiceDbSet, _seedInvoices);
            SetupMockDbSet(_mockPatientDbSet, _seedPatients);

            _mockContext.Setup(c => c.Invoices).Returns(_mockInvoiceDbSet.Object);
            _mockContext.Setup(c => c.Patients).Returns(_mockPatientDbSet.Object);

            // Mock transaction
            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            _mockContext.Setup(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockTransaction.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _invoiceService = new InvoiceService(_mockContext.Object, _mockLogger.Object);
        }

        // Generic DbSet Mock Setup (copied from previous test classes)
        private void SetupMockDbSet<TEntity>(Mock<DbSet<TEntity>> mockDbSet, List<TEntity> sourceList) where TEntity : class
        {
            var queryableList = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(queryableList.GetEnumerator()));

            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => {
                    if (typeof(TEntity) == typeof(Patient)) return sourceList.FirstOrDefault(e => (e as Patient).Id == (Guid)ids[0]) as TEntity;
                    if (typeof(TEntity) == typeof(Invoice)) return sourceList.FirstOrDefault(e => (e as Invoice).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreateInvoiceAsync_ValidData_ReturnsInvoiceDtoAndCalculatesTotal()
        {
            // Arrange
            var patient = _seedPatients.First();
            var createDto = new CreateInvoiceDto
            {
                PatientId = patient.Id,
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(30),
                InvoiceItems = new List<CreateInvoiceItemDto>
                {
                    new CreateInvoiceItemDto { Description = "Service A", Quantity = 1, UnitPrice = 100 },
                    new CreateInvoiceItemDto { Description = "Service B", Quantity = 2, UnitPrice = 25 }
                }
            };

            // Act
            var resultDto = await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(patient.Id, resultDto.PatientId);
            Assert.AreEqual(150.00m, resultDto.TotalAmount); // 100 + (2 * 25)
            Assert.AreEqual(2, resultDto.InvoiceItems.Count);
            _mockInvoiceDbSet.Verify(db => db.Add(It.IsAny<Invoice>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateInvoiceAsync_PatientNotFound_ReturnsNull()
        {
            // Arrange
            var createDto = new CreateInvoiceDto
            {
                PatientId = Guid.NewGuid(), // Non-existent patient
                InvoiceItems = new List<CreateInvoiceItemDto> { new CreateInvoiceItemDto { Description = "Test", Quantity = 1, UnitPrice = 10 } }
            };

            // Act
            var resultDto = await _invoiceService.CreateInvoiceAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            _mockInvoiceDbSet.Verify(db => db.Add(It.IsAny<Invoice>()), Times.Never());
        }

        [TestMethod]
        public async Task AddPaymentToInvoiceAsync_FullPayment_UpdatesAmountPaidAndStatusToPaid()
        {
            // Arrange
            var patient = _seedPatients.First();
            var invoiceId = Guid.NewGuid();
            var invoice = new Invoice
            {
                Id = invoiceId,
                PatientId = patient.Id,
                TotalAmount = 200.00m,
                AmountPaid = 0m,
                Status = InvoiceStatus.Sent
            };
            _seedInvoices.Add(invoice);
            SetupMockDbSet(_mockInvoiceDbSet, _seedInvoices);

            // Act
            var result = await _invoiceService.AddPaymentToInvoiceAsync(invoiceId, 200.00m, DateTime.Now);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200.00m, invoice.AmountPaid);
            Assert.AreEqual(InvoiceStatus.Paid, invoice.Status);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task AddPaymentToInvoiceAsync_PartialPayment_UpdatesAmountPaidAndStatusToPartiallyPaid()
        {
            // Arrange
            var patient = _seedPatients.First();
            var invoiceId = Guid.NewGuid();
            var invoice = new Invoice
            {
                Id = invoiceId,
                PatientId = patient.Id,
                TotalAmount = 200.00m,
                AmountPaid = 50.00m,
                Status = InvoiceStatus.Sent
            };
            _seedInvoices.Add(invoice);
            SetupMockDbSet(_mockInvoiceDbSet, _seedInvoices);

            // Act
            var result = await _invoiceService.AddPaymentToInvoiceAsync(invoiceId, 50.00m, DateTime.Now);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(100.00m, invoice.AmountPaid);
            Assert.AreEqual(InvoiceStatus.PartiallyPaid, invoice.Status);
        }

        [TestMethod]
        public async Task AddPaymentToInvoiceAsync_InvoiceNotFound_ReturnsNull()
        {
            // Act
            var result = await _invoiceService.AddPaymentToInvoiceAsync(Guid.NewGuid(), 100m, DateTime.Now);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateInvoiceStatusAsync_ValidUpdate_ReturnsTrue()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Draft };
            _seedInvoices.Add(invoice);
            SetupMockDbSet(_mockInvoiceDbSet, _seedInvoices);

            // Act
            var result = await _invoiceService.UpdateInvoiceStatusAsync(invoiceId, InvoiceStatus.Void);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(InvoiceStatus.Void, invoice.Status);
        }
    }
}
