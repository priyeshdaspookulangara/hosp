using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Emergency;
using UniCareERP.Application.Services.Emergency;
using UniCareERP.Domain.Entities.Emergency;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class EmergencyCaseServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<EmergencyCase>> _mockDbSet;
        private Mock<ILogger<EmergencyCaseService>> _mockLogger;
        private EmergencyCaseService _emergencyCaseService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockDbSet = new Mock<DbSet<EmergencyCase>>();
            _mockLogger = new Mock<ILogger<EmergencyCaseService>>();

            var emergencyCases = new List<EmergencyCase>().AsQueryable();
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.Provider).Returns(emergencyCases.Provider);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.Expression).Returns(emergencyCases.Expression);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.ElementType).Returns(emergencyCases.ElementType);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.GetEnumerator()).Returns(emergencyCases.GetEnumerator());

            _mockDbSet.Setup(d => d.Add(It.IsAny<EmergencyCase>())).Callback<EmergencyCase>((s) => emergencyCases.ToList().Add(s));
            _mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => emergencyCases.FirstOrDefault(p => p.Id == (Guid)ids[0]));

            _mockContext.Setup(c => c.EmergencyCases).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _emergencyCaseService = new EmergencyCaseService(_mockContext.Object, _mockLogger.Object);
        }

        private List<Patient> GetSeedPatients()
        {
            return new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
                new Patient { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
            };
        }

        private List<EmergencyCase> GetSeedEmergencyCases(List<Patient> patients)
        {
            return new List<EmergencyCase>
            {
                new EmergencyCase { Id = Guid.NewGuid(), PatientId = patients[0].Id, Patient = patients[0], CaseDescription = "Fever", ReportedAt = DateTime.UtcNow, Status = Domain.Enums.EmergencyCaseStatus.Reported },
                new EmergencyCase { Id = Guid.NewGuid(), PatientId = patients[1].Id, Patient = patients[1], CaseDescription = "Headache", ReportedAt = DateTime.UtcNow, Status = Domain.Enums.EmergencyCaseStatus.InProgress }
            };
        }

        private void SetupMockDbSet(List<EmergencyCase> emergencyCases)
        {
            var queryableEmergencyCases = emergencyCases.AsQueryable();
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.Provider).Returns(queryableEmergencyCases.Provider);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.Expression).Returns(queryableEmergencyCases.Expression);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.ElementType).Returns(queryableEmergencyCases.ElementType);
            _mockDbSet.As<IQueryable<EmergencyCase>>().Setup(m => m.GetEnumerator()).Returns(() => queryableEmergencyCases.GetEnumerator());
            _mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => queryableEmergencyCases.FirstOrDefault(p => p.Id == (Guid)ids[0]));
            _mockDbSet.Setup(m => m.Add(It.IsAny<EmergencyCase>())).Callback<EmergencyCase>(s => emergencyCases.Add(s));
            _mockDbSet.Setup(m => m.Remove(It.IsAny<EmergencyCase>())).Callback<EmergencyCase>(s => emergencyCases.Remove(s));
        }

        [TestMethod]
        public async Task CreateEmergencyCaseAsync_ValidDto_ReturnsEmergencyCaseDtoAndSaves()
        {
            // Arrange
            var patients = GetSeedPatients();
            var emergencyCases = new List<EmergencyCase>();
            SetupMockDbSet(emergencyCases);
            var createDto = new CreateEmergencyCaseDto { PatientId = patients[0].Id, CaseDescription = "New Case" };

            _mockContext.Setup(c => c.Patients).Returns(DbSetMock.Create(patients.AsQueryable()).Object);


            // Act
            var resultDto = await _emergencyCaseService.CreateEmergencyCaseAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("New Case", resultDto.CaseDescription);
            _mockDbSet.Verify(db => db.Add(It.IsAny<EmergencyCase>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task GetEmergencyCaseByIdAsync_CaseExists_ReturnsEmergencyCaseDto()
        {
            // Arrange
            var patients = GetSeedPatients();
            var emergencyCases = GetSeedEmergencyCases(patients);
            var emergencyCaseId = emergencyCases[0].Id;
            SetupMockDbSet(emergencyCases);

            // Act
            var resultDto = await _emergencyCaseService.GetEmergencyCaseByIdAsync(emergencyCaseId);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(emergencyCaseId, resultDto.Id);
        }

        [TestMethod]
        public async Task GetAllEmergencyCasesAsync_ReturnsAllEmergencyCaseDtos()
        {
            // Arrange
            var patients = GetSeedPatients();
            var emergencyCases = GetSeedEmergencyCases(patients);
            SetupMockDbSet(emergencyCases);

            // Act
            var results = await _emergencyCaseService.GetAllEmergencyCasesAsync();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task UpdateEmergencyCaseAsync_CaseExists_ReturnsTrueAndUpdates()
        {
            // Arrange
            var patients = GetSeedPatients();
            var emergencyCases = GetSeedEmergencyCases(patients);
            var emergencyCaseId = emergencyCases[0].Id;
            SetupMockDbSet(emergencyCases);

            var updateDto = new UpdateEmergencyCaseDto { Id = emergencyCaseId, CaseDescription = "Updated Case", Status = Domain.Enums.EmergencyCaseStatus.Resolved };

            // Act
            var result = await _emergencyCaseService.UpdateEmergencyCaseAsync(updateDto);

            // Assert
            Assert.IsNotNull(result);
            var updatedEmergencyCase = emergencyCases.First(p => p.Id == emergencyCaseId);
            Assert.AreEqual("Updated Case", updatedEmergencyCase.CaseDescription);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task DeleteEmergencyCaseAsync_CaseExists_ReturnsTrueAndRemoves()
        {
            // Arrange
            var patients = GetSeedPatients();
            var emergencyCases = GetSeedEmergencyCases(patients);
            var emergencyCaseId = emergencyCases[0].Id;
            SetupMockDbSet(emergencyCases);

            // Act
            var result = await _emergencyCaseService.DeleteEmergencyCaseAsync(emergencyCaseId);

            // Assert
            Assert.IsTrue(result);
            _mockDbSet.Verify(db => db.Remove(It.IsAny<EmergencyCase>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.IsFalse(emergencyCases.Any(p => p.Id == emergencyCaseId));
        }
    }
}
