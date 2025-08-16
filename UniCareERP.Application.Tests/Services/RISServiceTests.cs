using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Radiology;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class RISServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<RadiologyOrder>> _mockDbSet;
        private Mock<ILogger<RISService>> _mockLogger;
        private RISService _risService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockDbSet = new Mock<DbSet<RadiologyOrder>>();
            _mockLogger = new Mock<ILogger<RISService>>();

            var orders = new List<RadiologyOrder>().AsQueryable();
            _mockDbSet.As<IQueryable<RadiologyOrder>>().Setup(m => m.Provider).Returns(orders.Provider);
            _mockDbSet.As<IQueryable<RadiologyOrder>>().Setup(m => m.Expression).Returns(orders.Expression);
            _mockDbSet.As<IQueryable<RadiologyOrder>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            _mockDbSet.As<IQueryable<RadiologyOrder>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _mockDbSet.Setup(d => d.Add(It.IsAny<RadiologyOrder>())).Callback<RadiologyOrder>((s) => orders.ToList().Add(s));
            _mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => orders.FirstOrDefault(p => p.Id == (Guid)ids[0]));

            _mockContext.Setup(c => c.RadiologyOrders).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _risService = new RISService(_mockContext.Object, _mockLogger.Object);
        }

        private List<Patient> GetSeedPatients()
        {
            return new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" }
            };
        }

        private List<RadiologyTest> GetSeedTests()
        {
            return new List<RadiologyTest>
            {
                new RadiologyTest { Id = Guid.NewGuid(), Name = "X-Ray" }
            };
        }

        [TestMethod]
        public async Task CreateOrderAsync_ValidOrder_ReturnsOrderDto()
        {
            // Arrange
            var patients = GetSeedPatients();
            var tests = GetSeedTests();
            var orderDto = new RadiologyOrderDto { PatientId = patients[0].Id, TestTypeId = tests[0].Id };
            _mockContext.Setup(c => c.Patients).Returns(DbSetMock.Create(patients.AsQueryable()).Object);
            _mockContext.Setup(c => c.RadiologyTests).Returns(DbSetMock.Create(tests.AsQueryable()).Object);

            // Act
            var result = await _risService.CreateOrderAsync(orderDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(patients[0].Id, result.PatientId);
            _mockDbSet.Verify(db => db.Add(It.IsAny<RadiologyOrder>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
