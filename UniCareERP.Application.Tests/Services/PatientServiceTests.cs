using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Patients;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class PatientServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Patient>> _mockDbSet;
        private Mock<ILogger<PatientService>> _mockLogger;
        private PatientService _patientService;

        private List<Patient> _seedPatients;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockDbSet = new Mock<DbSet<Patient>>();
            _mockLogger = new Mock<ILogger<PatientService>>();

            _seedPatients = new List<Patient>();

            // Setup DbSets
            SetupMockDbSet(_mockDbSet, _seedPatients);

            _mockContext.Setup(c => c.Patients).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1); // Simulate successful save

            _patientService = new PatientService(_mockContext.Object, _mockLogger.Object);
        }

        // Generic DbSet Mock Setup
        private void SetupMockDbSet<TEntity>(Mock<DbSet<TEntity>> mockDbSet, List<TEntity> sourceList) where TEntity : class
        {
            var queryableList = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TEntity>(queryableList.Provider));
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(queryableList.GetEnumerator()));

            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => {
                    if (typeof(TEntity) == typeof(Patient)) return sourceList.FirstOrDefault(e => (e as Patient).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
            mockDbSet.Setup(d => d.Remove(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Remove(s));
        }


        [TestMethod]
        public async Task GenerateNextPatientCodeAsync_NoExistingPatients_ReturnsP00001()
        {
            // Act
            var code = await _patientService.GenerateNextPatientCodeAsync();

            // Assert
            Assert.AreEqual("P00001", code);
        }

        [TestMethod]
        public async Task GenerateNextPatientCodeAsync_WithExistingPatients_ReturnsCorrectNextCode()
        {
            // Arrange
            _seedPatients.Add(new Patient { PatientCode = "P00001" });
            _seedPatients.Add(new Patient { PatientCode = "P00002" });

            // Act
            var code = await _patientService.GenerateNextPatientCodeAsync();

            // Assert
            Assert.AreEqual("P00003", code);
        }


        [TestMethod]
        public async Task CreatePatientAsync_ValidDto_ReturnsPatientDtoAndSaves()
        {
            // Arrange
            var createDto = new CreatePatientDto { FirstName = "New", LastName = "Patient", DateOfBirth = new DateTime(2000,1,1), Gender="Male" };

            // Act
            var resultDto = await _patientService.CreatePatientAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("New", resultDto.FirstName);
            Assert.AreEqual("P00001", resultDto.PatientCode); // Assuming GenerateNextPatientCodeAsync works as tested
            _mockDbSet.Verify(db => db.Add(It.IsAny<Patient>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_PatientExists_ReturnsPatientDto()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _seedPatients.Add(new Patient { Id = patientId, FirstName = "Test", LastName = "User", PatientCode="P00001", DateOfBirth = DateTime.Now, Gender="Test" });

            // Act
            var resultDto = await _patientService.GetPatientByIdAsync(patientId);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(patientId, resultDto.Id);
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_PatientDoesNotExist_ReturnsNull()
        {
            // Act
            var resultDto = await _patientService.GetPatientByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(resultDto);
        }

        [TestMethod]
        public async Task GetAllPatientsAsync_ReturnsAllPatientDtos()
        {
            // Arrange
            _seedPatients.Add(new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", PatientCode = "P00001", DateOfBirth = new DateTime(1980,1,1), Gender="Male" });
            _seedPatients.Add(new Patient { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", PatientCode = "P00002", DateOfBirth = new DateTime(1990,5,5), Gender="Female" });

            // Act
            var results = await _patientService.GetAllPatientsAsync();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task UpdatePatientAsync_PatientExists_ReturnsTrueAndUpdates()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _seedPatients.Add(new Patient { Id = patientId, FirstName = "OldName", LastName="OldLast", PatientCode="P00001", DateOfBirth=DateTime.Now, Gender="Old" });

            var updateDto = new UpdatePatientDto { Id = patientId, FirstName = "NewName", LastName="NewLast", DateOfBirth = DateTime.Now.AddYears(-20), Gender="New" };

            // Act
            var result = await _patientService.UpdatePatientAsync(updateDto);

            // Assert
            Assert.IsTrue(result);
            var updatedPatient = _seedPatients.First(p => p.Id == patientId);
            Assert.AreEqual("NewName", updatedPatient.FirstName);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task UpdatePatientAsync_PatientDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updateDto = new UpdatePatientDto { Id = Guid.NewGuid(), FirstName = "Test" };

            // Act
            var result = await _patientService.UpdatePatientAsync(updateDto);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public async Task DeletePatientAsync_PatientExists_ReturnsTrueAndRemoves()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _seedPatients.Add(new Patient { Id = patientId, FirstName = "Test" });

            // Act
            var result = await _patientService.DeletePatientAsync(patientId);

            // Assert
            Assert.IsTrue(result);
            _mockDbSet.Verify(db => db.Remove(It.IsAny<Patient>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.IsFalse(_seedPatients.Any(p => p.Id == patientId)); // Check if removed from the list used by mock
        }

        [TestMethod]
        public async Task DeletePatientAsync_PatientDoesNotExist_ReturnsFalse()
        {
            // Act
            var result = await _patientService.DeletePatientAsync(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
    }

}
