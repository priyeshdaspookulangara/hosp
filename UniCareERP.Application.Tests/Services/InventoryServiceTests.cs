using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Inventory;
using UniCareERP.Application.Services.Inventory;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class InventoryServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<InventoryItem>> _mockItemDbSet;
        private Mock<DbSet<StockTransaction>> _mockTransactionDbSet;
        private Mock<ILogger<InventoryService>> _mockLogger;
        private InventoryService _inventoryService;

        private List<InventoryItem> _seedItems;
        private List<StockTransaction> _seedTransactions;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockItemDbSet = new Mock<DbSet<InventoryItem>>();
            _mockTransactionDbSet = new Mock<DbSet<StockTransaction>>();
            _mockLogger = new Mock<ILogger<InventoryService>>();

            // Seed Data
            _seedItems = new List<InventoryItem>();
            _seedTransactions = new List<StockTransaction>();

            // Setup DbSets
            SetupMockDbSet(_mockItemDbSet, _seedItems);
            SetupMockDbSet(_mockTransactionDbSet, _seedTransactions);

            _mockContext.Setup(c => c.InventoryItems).Returns(_mockItemDbSet.Object);
            _mockContext.Setup(c => c.StockTransactions).Returns(_mockTransactionDbSet.Object);

            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            _mockContext.Setup(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockTransaction.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _inventoryService = new InventoryService(_mockContext.Object, _mockLogger.Object);
        }

        // Generic DbSet Mock Setup
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
                    if (typeof(TEntity) == typeof(InventoryItem)) return sourceList.FirstOrDefault(e => (e as InventoryItem).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreateItemAsync_ValidDto_ReturnsItemDtoWithZeroStock()
        {
            // Arrange
            var createDto = new CreateInventoryItemDto { Name = "Test Item", Category = "Medicine", UnitOfMeasure = "Pcs", UnitPrice = 10, CostPrice = 5, ReorderLevel = 10 };

            // Act
            var resultDto = await _inventoryService.CreateItemAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("Test Item", resultDto.Name);
            Assert.AreEqual(0, resultDto.QuantityInStock); // Must be created with 0 stock
            Assert.IsTrue(resultDto.IsActive);
            _mockItemDbSet.Verify(db => db.Add(It.IsAny<InventoryItem>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task GenerateNextItemCodeAsync_FirstItemInCategory_ReturnsCodeWith0001()
        {
            // Arrange
            string category = "Supply";

            // Act
            var itemCode = await _inventoryService.GenerateNextItemCodeAsync(category);

            // Assert
            Assert.AreEqual("SUP0001", itemCode);
        }

        [TestMethod]
        public async Task GenerateNextItemCodeAsync_ExistingItems_ReturnsNextCode()
        {
            // Arrange
            _seedItems.Add(new InventoryItem { ItemCode = "MED0001" });
            _seedItems.Add(new InventoryItem { ItemCode = "MED0002" });
            SetupMockDbSet(_mockItemDbSet, _seedItems);

            // Act
            var itemCode = await _inventoryService.GenerateNextItemCodeAsync("Medicine");

            // Assert
            Assert.AreEqual("MED0003", itemCode);
        }

        [TestMethod]
        public async Task DeleteItemAsync_SoftDeletesItem_ReturnsTrue()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, Name = "Item to Delete", IsActive = true };
            _seedItems.Add(item);
            SetupMockDbSet(_mockItemDbSet, _seedItems);

            // Act
            var result = await _inventoryService.DeleteItemAsync(itemId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(item.IsActive); // Verify the flag is set to false
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task AdjustStockAsync_PositiveAdjustment_IncreasesStockAndCreatesTransaction()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, Name = "Test Item", QuantityInStock = 10 };
            _seedItems.Add(item);
            SetupMockDbSet(_mockItemDbSet, _seedItems);

            // Act
            var result = await _inventoryService.AdjustStockAsync(itemId, 5, "Initial stock count");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(15, item.QuantityInStock);
            Assert.AreEqual(1, _seedTransactions.Count);
            Assert.AreEqual(5, _seedTransactions.First().QuantityChanged);
            _mockTransactionDbSet.Verify(db => db.Add(It.IsAny<StockTransaction>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task AdjustStockAsync_NegativeAdjustment_DecreasesStock()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, Name = "Test Item", QuantityInStock = 20 };
            _seedItems.Add(item);
            SetupMockDbSet(_mockItemDbSet, _seedItems);

            // Act
            var result = await _inventoryService.AdjustStockAsync(itemId, -5, "Sale");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(15, item.QuantityInStock);
        }

        [TestMethod]
        public async Task AdjustStockAsync_InsufficientStock_ReturnsFalseAndDoesNotChangeStock()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, Name = "Test Item", QuantityInStock = 5 };
            _seedItems.Add(item);
            SetupMockDbSet(_mockItemDbSet, _seedItems);

            // Act
            var result = await _inventoryService.AdjustStockAsync(itemId, -10, "Sale Error");

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(5, item.QuantityInStock); // Stock should not have changed
            _mockTransactionDbSet.Verify(db => db.Add(It.IsAny<StockTransaction>()), Times.Never());
        }
    }
}
