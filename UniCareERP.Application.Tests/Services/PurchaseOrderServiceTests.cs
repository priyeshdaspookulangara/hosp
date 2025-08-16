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
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class PurchaseOrderServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<PurchaseOrder>> _mockPoDbSet;
        private Mock<IInventoryService> _mockInventoryService;
        private Mock<ILogger<PurchaseOrderService>> _mockLogger;
        private PurchaseOrderService _poService;

        private List<PurchaseOrder> _seedPOs;
        private List<InventoryItem> _seedItems;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockPoDbSet = new Mock<DbSet<PurchaseOrder>>();
            _mockInventoryService = new Mock<IInventoryService>();
            _mockLogger = new Mock<ILogger<PurchaseOrderService>>();

            _seedPOs = new List<PurchaseOrder>();
            _seedItems = new List<InventoryItem>
            {
                new InventoryItem { Id = Guid.NewGuid(), Name = "Test Item 1", ItemCode = "ITM001" },
                new InventoryItem { Id = Guid.NewGuid(), Name = "Test Item 2", ItemCode = "ITM002" }
            };

            SetupMockDbSet(_mockPoDbSet, _seedPOs);
            _mockContext.Setup(c => c.PurchaseOrders).Returns(_mockPoDbSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _poService = new PurchaseOrderService(_mockContext.Object, _mockInventoryService.Object, _mockLogger.Object);
        }

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
                .ReturnsAsync((object[] ids) => sourceList.FirstOrDefault(e => (e as PurchaseOrder).Id == (Guid)ids[0]) as TEntity);

            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreatePurchaseOrderAsync_ValidDto_ReturnsPoDto()
        {
            // Arrange
            var createDto = new CreatePurchaseOrderDto
            {
                SupplierInfo = "Test Supplier",
                Items = new List<CreatePurchaseOrderItemDto>
                {
                    new CreatePurchaseOrderItemDto { InventoryItemId = _seedItems[0].Id, QuantityOrdered = 10, UnitPrice = 5.0m },
                    new CreatePurchaseOrderItemDto { InventoryItemId = _seedItems[1].Id, QuantityOrdered = 5, UnitPrice = 10.0m }
                }
            };

            _mockPoDbSet.Setup(m => m.Include(It.IsAny<string>()))
                        .Returns(_mockPoDbSet.Object);

            // Act
            var resultDto = await _poService.CreatePurchaseOrderAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(100.0m, resultDto.TotalAmount); // (10 * 5) + (5 * 10)
            Assert.AreEqual(PurchaseOrderStatus.Pending, resultDto.Status);
            _mockPoDbSet.Verify(db => db.Add(It.IsAny<PurchaseOrder>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task ApprovePurchaseOrderAsync_PendingPo_ReturnsTrueAndSetsStatusToApproved()
        {
            // Arrange
            var poId = Guid.NewGuid();
            var po = new PurchaseOrder { Id = poId, Status = PurchaseOrderStatus.Pending };
            _seedPOs.Add(po);

            // Act
            var result = await _poService.ApprovePurchaseOrderAsync(poId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(PurchaseOrderStatus.Approved, po.Status);
        }

        [TestMethod]
        public async Task ApprovePurchaseOrderAsync_AlreadyApprovedPo_ReturnsFalse()
        {
            // Arrange
            var poId = Guid.NewGuid();
            var po = new PurchaseOrder { Id = poId, Status = PurchaseOrderStatus.Approved };
            _seedPOs.Add(po);

            // Act
            var result = await _poService.ApprovePurchaseOrderAsync(poId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ReceiveGoodsAsync_FullReceipt_UpdatesStockAndCompletesPo()
        {
            // Arrange
            var poId = Guid.NewGuid();
            var poItem1Id = Guid.NewGuid();
            var item1Id = _seedItems[0].Id;
            var po = new PurchaseOrder
            {
                Id = poId,
                Status = PurchaseOrderStatus.Approved,
                Items = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem { Id = poItem1Id, InventoryItemId = item1Id, QuantityOrdered = 10, QuantityReceived = 0 }
                }
            };
            _seedPOs.Add(po);
            _mockInventoryService.Setup(s => s.AdjustStockAsync(item1Id, 10, It.IsAny<string>(), "Purchase")).ReturnsAsync(true);

            var receivedItems = new List<ReceivedItemDto>
            {
                new ReceivedItemDto { PurchaseOrderItemId = poItem1Id, QuantityReceived = 10 }
            };

            // Act
            var result = await _poService.ReceiveGoodsAsync(poId, receivedItems);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PurchaseOrderStatus.Completed, result.Status);
            _mockInventoryService.Verify(s => s.AdjustStockAsync(item1Id, 10, It.IsAny<string>(), "Purchase"), Times.Once);
        }

        [TestMethod]
        public async Task ReceiveGoodsAsync_PartialReceipt_UpdatesStockAndSetsPoToPartiallyReceived()
        {
            // Arrange
            var poId = Guid.NewGuid();
            var poItem1Id = Guid.NewGuid();
            var item1Id = _seedItems[0].Id;
            var po = new PurchaseOrder
            {
                Id = poId,
                Status = PurchaseOrderStatus.Approved,
                Items = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem { Id = poItem1Id, InventoryItemId = item1Id, QuantityOrdered = 10, QuantityReceived = 0 }
                }
            };
            _seedPOs.Add(po);
            _mockInventoryService.Setup(s => s.AdjustStockAsync(item1Id, 5, It.IsAny<string>(), "Purchase")).ReturnsAsync(true);

            var receivedItems = new List<ReceivedItemDto>
            {
                new ReceivedItemDto { PurchaseOrderItemId = poItem1Id, QuantityReceived = 5 }
            };

            // Act
            var result = await _poService.ReceiveGoodsAsync(poId, receivedItems);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PurchaseOrderStatus.PartiallyReceived, result.Status);
            Assert.AreEqual(5, result.Items.First().QuantityReceived);
        }

        [TestMethod]
        public async Task ReceiveGoodsAsync_OverReceipt_ThrowsExceptionAndRollsBack()
        {
            // Arrange
            var poId = Guid.NewGuid();
            var poItem1Id = Guid.NewGuid();
            var item1Id = _seedItems[0].Id;
            var po = new PurchaseOrder
            {
                Id = poId,
                Status = PurchaseOrderStatus.Approved,
                Items = new List<PurchaseOrderItem>
                {
                    new PurchaseOrderItem { Id = poItem1Id, InventoryItemId = item1Id, QuantityOrdered = 10, QuantityReceived = 5 }
                }
            };
            _seedPOs.Add(po);

            var receivedItems = new List<ReceivedItemDto>
            {
                new ReceivedItemDto { PurchaseOrderItemId = poItem1Id, QuantityReceived = 6 } // Trying to receive 6 when only 5 are left
            };

            // Act
            var result = await _poService.ReceiveGoodsAsync(poId, receivedItems);

            // Assert
            Assert.IsNull(result); // Service should return null on failure
            _mockInventoryService.Verify(s => s.AdjustStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
