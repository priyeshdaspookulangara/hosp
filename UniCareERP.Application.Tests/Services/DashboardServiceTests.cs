using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.Services.Dashboard;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class DashboardServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<ILogger<DashboardService>> _mockLogger;
        private DashboardService _dashboardService;

        private List<Patient> _seedPatients;
        private List<Appointment> _seedAppointments;
        private List<Sale> _seedSales;
        private List<InventoryItem> _seedItems;
        private List<PurchaseOrder> _seedPOs;
        private List<Invoice> _seedInvoices;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockLogger = new Mock<ILogger<DashboardService>>();

            // Seed Data
            _seedPatients = new List<Patient>();
            _seedAppointments = new List<Appointment>();
            _seedSales = new List<Sale>();
            _seedItems = new List<InventoryItem>();
            _seedPOs = new List<PurchaseOrder>();
            _seedInvoices = new List<Invoice>();

            // Setup DbSets
            SetupMockDbSet(new Mock<DbSet<Patient>>(), _seedPatients);
            SetupMockDbSet(new Mock<DbSet<Appointment>>(), _seedAppointments);
            SetupMockDbSet(new Mock<DbSet<Sale>>(), _seedSales);
            SetupMockDbSet(new Mock<DbSet<InventoryItem>>(), _seedItems);
            SetupMockDbSet(new Mock<DbSet<PurchaseOrder>>(), _seedPOs);
            SetupMockDbSet(new Mock<DbSet<Invoice>>(), _seedInvoices);

            _dashboardService = new DashboardService(_mockContext.Object, _mockLogger.Object);
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

            _mockContext.Setup(c => c.Set<TEntity>()).Returns(mockDbSet.Object);
        }

        [TestMethod]
        public async Task GetDashboardDataAsync_CalculatesStatsCorrectly()
        {
            // Arrange
            var today = DateTime.Today;
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today, Status = AppointmentStatus.Scheduled });
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today, Status = AppointmentStatus.Completed });
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today.AddDays(-1), Status = AppointmentStatus.Scheduled }); // Not today

            _seedSales.Add(new Sale { SaleDate = today, TotalAmount = 100 });
            _seedSales.Add(new Sale { SaleDate = today, TotalAmount = 50 });
            _seedSales.Add(new Sale { SaleDate = today.AddDays(-1), TotalAmount = 200 }); // Not today

            _seedItems.Add(new InventoryItem { IsActive = true, QuantityInStock = 5, ReorderLevel = 10 }); // Low stock
            _seedItems.Add(new InventoryItem { IsActive = true, QuantityInStock = 10, ReorderLevel = 10 }); // Low stock
            _seedItems.Add(new InventoryItem { IsActive = true, QuantityInStock = 11, ReorderLevel = 10 }); // Not low stock
            _seedItems.Add(new InventoryItem { IsActive = false, QuantityInStock = 1, ReorderLevel = 10 }); // Not active

            // Act
            var result = await _dashboardService.GetDashboardDataAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Stats.AppointmentsToday);
            Assert.AreEqual(150m, result.Stats.TotalSalesToday);
            Assert.AreEqual(2, result.Stats.LowStockItems);
        }

        [TestMethod]
        public async Task GetDashboardDataAsync_CalculatesWeeklySalesChartCorrectly()
        {
            // Arrange
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            _seedSales.Add(new Sale { SaleDate = startOfWeek, TotalAmount = 10 });
            _seedSales.Add(new Sale { SaleDate = startOfWeek, TotalAmount = 15 }); // 25 total for start of week
            _seedSales.Add(new Sale { SaleDate = startOfWeek.AddDays(2), TotalAmount = 50 });
            _seedSales.Add(new Sale { SaleDate = today.AddDays(-30), TotalAmount = 1000 }); // Not in this week

            // Act
            var result = await _dashboardService.GetDashboardDataAsync();

            // Assert
            Assert.IsNotNull(result.WeeklySalesChart);
            Assert.AreEqual(7, result.WeeklySalesChart.Labels.Count);
            Assert.AreEqual(25m, result.WeeklySalesChart.Data[0]); // Sale on Sunday (first day)
            Assert.AreEqual(0m, result.WeeklySalesChart.Data[1]);   // No sale on Monday
            Assert.AreEqual(50m, result.WeeklySalesChart.Data[2]); // Sale on Tuesday
        }

        [TestMethod]
        public async Task GetDashboardDataAsync_CalculatesAppointmentStatusChartCorrectly()
        {
            // Arrange
            var today = DateTime.Today;
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today, Status = AppointmentStatus.Scheduled });
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today, Status = AppointmentStatus.Scheduled });
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today, Status = AppointmentStatus.Completed });
            _seedAppointments.Add(new Appointment { AppointmentDateTime = today.AddDays(1), Status = AppointmentStatus.Scheduled }); // Not today

            // Act
            var result = await _dashboardService.GetDashboardDataAsync();

            // Assert
            Assert.IsNotNull(result.AppointmentStatusChart);
            var scheduledData = result.AppointmentStatusChart.Data[result.AppointmentStatusChart.Labels.IndexOf("Scheduled")];
            var completedData = result.AppointmentStatusChart.Data[result.AppointmentStatusChart.Labels.IndexOf("Completed")];

            Assert.AreEqual(2, scheduledData);
            Assert.AreEqual(1, completedData);
        }
    }
}
