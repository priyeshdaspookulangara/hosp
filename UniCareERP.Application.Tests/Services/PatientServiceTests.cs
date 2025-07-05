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

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class PatientServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Patient>> _mockDbSet;
        private Mock<ILogger<PatientService>> _mockLogger;
        private PatientService _patientService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockDbSet = new Mock<DbSet<Patient>>();
            _mockLogger = new Mock<ILogger<PatientService>>();

            // Setup DbSet properties and methods
            var patients = new List<Patient>().AsQueryable();
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patients.Provider);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patients.Expression);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patients.ElementType);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patients.GetEnumerator());

            _mockDbSet.Setup(d => d.Add(It.IsAny<Patient>())).Callback<Patient>((s) => patients.ToList().Add(s)); // Simulate Add
            _mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => patients.FirstOrDefault(p => p.Id == (Guid)ids[0]));


            _mockContext.Setup(c => c.Patients).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1); // Simulate successful save

            _patientService = new PatientService(_mockContext.Object, _mockLogger.Object);
        }

        private List<Patient> GetSeedPatients()
        {
            return new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", PatientCode = "P00001", DateOfBirth = new DateTime(1980,1,1), Gender="Male" },
                new Patient { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", PatientCode = "P00002", DateOfBirth = new DateTime(1990,5,5), Gender="Female" }
            };
        }

        private void SetupMockDbSet(List<Patient> patients)
        {
            var queryablePatients = patients.AsQueryable();
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(queryablePatients.Provider);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(queryablePatients.Expression);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(queryablePatients.ElementType);
            _mockDbSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(() => queryablePatients.GetEnumerator());
            _mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => queryablePatients.FirstOrDefault(p => p.Id == (Guid)ids[0]));
            _mockDbSet.Setup(m => m.Add(It.IsAny<Patient>())).Callback<Patient>(s => patients.Add(s));
            _mockDbSet.Setup(m => m.Remove(It.IsAny<Patient>())).Callback<Patient>(s => patients.Remove(s));
        }


        [TestMethod]
        public async Task GenerateNextPatientCodeAsync_NoExistingPatients_ReturnsP00001()
        {
            // Arrange
            SetupMockDbSet(new List<Patient>()); // No patients

            // Act
            var code = await _patientService.GenerateNextPatientCodeAsync();

            // Assert
            Assert.AreEqual("P00001", code);
        }

        [TestMethod]
        public async Task GenerateNextPatientCodeAsync_WithExistingPatients_ReturnsCorrectNextCode()
        {
            // Arrange
             var patients = new List<Patient> { new Patient { PatientCode = "P00001" }, new Patient { PatientCode = "P00002" } };
            SetupMockDbSet(patients);
             _mockDbSet.As<IAsyncEnumerable<Patient>>() // For OrderByDescending(...).FirstOrDefaultAsync()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Patient>(patients.OrderByDescending(p=>p.PatientCode).AsQueryable().GetEnumerator()));


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
            SetupMockDbSet(new List<Patient>());


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
            var patients = new List<Patient> { new Patient { Id = patientId, FirstName = "Test", LastName = "User", PatientCode="P00001", DateOfBirth = DateTime.Now, Gender="Test" } };
            SetupMockDbSet(patients);

            // Act
            var resultDto = await _patientService.GetPatientByIdAsync(patientId);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(patientId, resultDto.Id);
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_PatientDoesNotExist_ReturnsNull()
        {
            // Arrange
            SetupMockDbSet(new List<Patient>());

            // Act
            var resultDto = await _patientService.GetPatientByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(resultDto);
        }

        [TestMethod]
        public async Task GetAllPatientsAsync_ReturnsAllPatientDtos()
        {
            // Arrange
            var seedPatients = GetSeedPatients();
            SetupMockDbSet(seedPatients);
             _mockDbSet.As<IAsyncEnumerable<Patient>>() // For ToListAsync()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Patient>(seedPatients.AsQueryable().GetEnumerator()));


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
            var patients = new List<Patient> { new Patient { Id = patientId, FirstName = "OldName", LastName="OldLast", PatientCode="P00001", DateOfBirth=DateTime.Now, Gender="Old" } };
            SetupMockDbSet(patients);

            var updateDto = new UpdatePatientDto { Id = patientId, FirstName = "NewName", LastName="NewLast", DateOfBirth = DateTime.Now.AddYears(-20), Gender="New" };

            // Act
            var result = await _patientService.UpdatePatientAsync(updateDto);

            // Assert
            Assert.IsTrue(result);
            var updatedPatient = patients.First(p => p.Id == patientId);
            Assert.AreEqual("NewName", updatedPatient.FirstName);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task UpdatePatientAsync_PatientDoesNotExist_ReturnsFalse()
        {
            // Arrange
            SetupMockDbSet(new List<Patient>());
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
            var patients = new List<Patient> { new Patient { Id = patientId, FirstName = "Test" } };
            SetupMockDbSet(patients);

            // Act
            var result = await _patientService.DeletePatientAsync(patientId);

            // Assert
            Assert.IsTrue(result);
            _mockDbSet.Verify(db => db.Remove(It.IsAny<Patient>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.IsFalse(patients.Any(p => p.Id == patientId)); // Check if removed from the list used by mock
        }

        [TestMethod]
        public async Task DeletePatientAsync_PatientDoesNotExist_ReturnsFalse()
        {
            // Arrange
            SetupMockDbSet(new List<Patient>());

            // Act
            var result = await _patientService.DeletePatientAsync(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
    }

    // Helper class for mocking IAsyncEnumerable for EF Core operations like ToListAsync, FirstOrDefaultAsync
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        public T Current => _enumerator.Current;
        public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;
        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
    }
}
