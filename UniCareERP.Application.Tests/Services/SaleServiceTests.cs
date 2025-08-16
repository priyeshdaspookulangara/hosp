using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Finance;
using UniCareERP.Application.Services.Inventory;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class SaleServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Sale>> _mockSaleDbSet;
        private Mock<IInventoryService> _mockInventoryService;
        private Mock<IInvoiceService> _mockInvoiceService;
        private Mock<ILogger<SaleService>> _mockLogger;
        private SaleService _saleService;

        private List<Sale> _seedSales;
        private List<InventoryItem> _seedItems;
        private List<Patient> _seedPatients;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockSaleDbSet = new Mock<DbSet<Sale>>();
            _mockInventoryService = new Mock<IInventoryService>();
            _mockInvoiceService = new Mock<IInvoiceService>();
            _mockLogger = new Mock<ILogger<SaleService>>();

            // Seed Data
            _seedSales = new List<Sale>();
            _seedItems = new List<InventoryItem>
            {
                new InventoryItem { Id = Guid.NewGuid(), Name = "Ibuprofen", QuantityInStock = 100 },
                new InventoryItem { Id = Guid.NewGuid(), Name = "Band-Aid", QuantityInStock = 200 }
            };
            _seedPatients = new List<Patient> { new Patient { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Patient" } };

            // Setup DbSets
            SetupMockDbSet(_mockSaleDbSet, _seedSales);
            _mockContext.Setup(c => c.Sales).Returns(_mockSaleDbSet.Object);
            // Mock Find on InventoryItems for the invoice creation part
            _mockContext.Setup(c => c.InventoryItems.Find(It.IsAny<object[]>()))
                        .Returns((object[] ids) => _seedItems.FirstOrDefault(i => i.Id == (Guid)ids[0]));


            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _saleService = new SaleService(_mockContext.Object, _mockInventoryService.Object, _mockInvoiceService.Object, _mockLogger.Object);
        }

        private void SetupMockDbSet<TEntity>(Mock<DbSet<TEntity>> mockDbSet, List<TEntity> sourceList) where TEntity : class
        {
            var queryableList = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreateSaleAsync_CashPayment_AdjustsStockAndReturnsSaleDto()
        {
            // Arrange
            var itemToSell = _seedItems.First();
            var createDto = new CreateSaleDto
            {
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto { InventoryItemId = itemToSell.Id, Quantity = 5, UnitPrice = 10.0m }
                }
            };
            _mockInventoryService.Setup(s => s.AdjustStockAsync(itemToSell.Id, -5, It.IsAny<string>(), "Sale")).ReturnsAsync(true);

            // Act
            var resultDto = await _saleService.CreateSaleAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(50.0m, resultDto.TotalAmount);
            Assert.AreEqual(PaymentMethod.Cash, resultDto.PaymentMethod);
            _mockInventoryService.Verify(s => s.AdjustStockAsync(itemToSell.Id, -5, It.IsAny<string>(), "Sale"), Times.Once);
            _mockInvoiceService.Verify(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceDto>()), Times.Never); // Should not be called for cash
            _mockSaleDbSet.Verify(db => db.Add(It.IsAny<Sale>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateSaleAsync_InsufficientStock_ReturnsNullAndRollsBack()
        {
            // Arrange
            var itemToSell = _seedItems.First();
            var createDto = new CreateSaleDto
            {
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto { InventoryItemId = itemToSell.Id, Quantity = 5, UnitPrice = 10.0m }
                }
            };
            // Simulate stock adjustment failure
            _mockInventoryService.Setup(s => s.AdjustStockAsync(itemToSell.Id, -5, It.IsAny<string>(), "Sale")).ReturnsAsync(false);

            // Act
            var resultDto = await _saleService.CreateSaleAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            _mockSaleDbSet.Verify(db => db.Add(It.IsAny<Sale>()), Times.Never); // Should not be added if transaction fails
        }

        [TestMethod]
        public async Task CreateSaleAsync_AddToPatientBill_AdjustsStockAndCreatesInvoice()
        {
            // Arrange
            var itemToSell = _seedItems.First();
            var patient = _seedPatients.First();
            var createDto = new CreateSaleDto
            {
                PatientId = patient.Id,
                PaymentMethod = PaymentMethod.AddToPatientBill,
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto { InventoryItemId = itemToSell.Id, Quantity = 2, UnitPrice = 15.0m }
                }
            };
            _mockInventoryService.Setup(s => s.AdjustStockAsync(itemToSell.Id, -2, It.IsAny<string>(), "Sale")).ReturnsAsync(true);
            _mockInvoiceService.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceDto>()))
                               .ReturnsAsync(new InvoiceDto { InvoiceNumber = "INV-TEST-001" }); // Simulate successful invoice creation

            // Act
            var resultDto = await _saleService.CreateSaleAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(PaymentMethod.AddToPatientBill, resultDto.PaymentMethod);
            _mockInventoryService.Verify(s => s.AdjustStockAsync(itemToSell.Id, -2, It.IsAny<string>(), "Sale"), Times.Once);
            _mockInvoiceService.Verify(s => s.CreateInvoiceAsync(It.Is<CreateInvoiceDto>(dto => dto.PatientId == patient.Id)), Times.Once);
        }

        [TestMethod]
        public async Task CreateSaleAsync_AddToBillFails_ReturnsNullAndRollsBack()
        {
            // Arrange
            var itemToSell = _seedItems.First();
            var patient = _seedPatients.First();
            var createDto = new CreateSaleDto
            {
                PatientId = patient.Id,
                PaymentMethod = PaymentMethod.AddToPatientBill,
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto { InventoryItemId = itemToSell.Id, Quantity = 2, UnitPrice = 15.0m }
                }
            };
            _mockInventoryService.Setup(s => s.AdjustStockAsync(itemToSell.Id, -2, It.IsAny<string>(), "Sale")).ReturnsAsync(true);
            // Simulate invoice creation failure
            _mockInvoiceService.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceDto>())).ReturnsAsync((InvoiceDto?)null);

            // Act
            var resultDto = await _saleService.CreateSaleAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            // We expect AdjustStock to have been called, but the transaction should roll back.
            // Testing the rollback itself is hard with mocks, but we can verify the service call was attempted.
            _mockInventoryService.Verify(s => s.AdjustStockAsync(itemToSell.Id, -2, It.IsAny<string>(), "Sale"), Times.Once);
        }
    }
}
