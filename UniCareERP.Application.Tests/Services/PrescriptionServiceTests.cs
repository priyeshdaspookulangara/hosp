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
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class PrescriptionServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Prescription>> _mockPrescriptionDbSet;
        private Mock<DbSet<Patient>> _mockPatientDbSet;
        private Mock<DbSet<InventoryItem>> _mockInventoryItemDbSet;
        private Mock<ILogger<PrescriptionService>> _mockLogger;
        private PrescriptionService _prescriptionService;

        private List<Prescription> _seedPrescriptions;
        private List<Patient> _seedPatients;
        private List<InventoryItem> _seedItems;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockPrescriptionDbSet = new Mock<DbSet<Prescription>>();
            _mockPatientDbSet = new Mock<DbSet<Patient>>();
            _mockInventoryItemDbSet = new Mock<DbSet<InventoryItem>>();
            _mockLogger = new Mock<ILogger<PrescriptionService>>();

            _seedPrescriptions = new List<Prescription>();
            _seedPatients = new List<Patient> { new Patient { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Patient" } };
            _seedItems = new List<InventoryItem> { new InventoryItem { Id = Guid.NewGuid(), Name = "Aspirin" } };

            SetupMockDbSet(_mockPrescriptionDbSet, _seedPrescriptions);
            SetupMockDbSet(_mockPatientDbSet, _seedPatients);
            SetupMockDbSet(_mockInventoryItemDbSet, _seedItems);

            _mockContext.Setup(c => c.Prescriptions).Returns(_mockPrescriptionDbSet.Object);
            _mockContext.Setup(c => c.Patients).Returns(_mockPatientDbSet.Object);
            _mockContext.Setup(c => c.InventoryItems).Returns(_mockInventoryItemDbSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _prescriptionService = new PrescriptionService(_mockContext.Object, _mockLogger.Object);
        }

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
                    if (typeof(TEntity) == typeof(Prescription)) return sourceList.FirstOrDefault(e => (e as Prescription).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreatePrescriptionAsync_ValidData_ReturnsPrescriptionDto()
        {
            // Arrange
            var patient = _seedPatients.First();
            var item = _seedItems.First();
            var doctorId = Guid.NewGuid().ToString();
            var createDto = new CreatePrescriptionDto
            {
                PatientId = patient.Id,
                Items = new List<CreatePrescriptionItemDto>
                {
                    new CreatePrescriptionItemDto { InventoryItemId = item.Id, Dosage = "100mg", Frequency = "Once a day", Duration = "1 week" }
                }
            };

            // Act
            var resultDto = await _prescriptionService.CreatePrescriptionAsync(createDto, doctorId);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(1, resultDto.Items.Count);
            Assert.AreEqual(item.Id, resultDto.Items.First().InventoryItemId);
            Assert.AreEqual(PrescriptionStatus.Active, resultDto.Status);
            _mockPrescriptionDbSet.Verify(db => db.Add(It.IsAny<Prescription>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task CreatePrescriptionAsync_PatientNotFound_ReturnsNull()
        {
            // Arrange
            var createDto = new CreatePrescriptionDto { PatientId = Guid.NewGuid() }; // Non-existent patient

            // Act
            var resultDto = await _prescriptionService.CreatePrescriptionAsync(createDto, Guid.NewGuid().ToString());

            // Assert
            Assert.IsNull(resultDto);
        }

        [TestMethod]
        public async Task CreatePrescriptionAsync_ItemNotFound_ReturnsNull()
        {
            // Arrange
            var patient = _seedPatients.First();
            var createDto = new CreatePrescriptionDto
            {
                PatientId = patient.Id,
                Items = new List<CreatePrescriptionItemDto>
                {
                    new CreatePrescriptionItemDto { InventoryItemId = Guid.NewGuid() } // Non-existent item
                }
            };

            // Act
            var resultDto = await _prescriptionService.CreatePrescriptionAsync(createDto, Guid.NewGuid().ToString());

            // Assert
            Assert.IsNull(resultDto);
        }

        [TestMethod]
        public async Task CancelPrescriptionAsync_OwnedByDoctorAndActive_ReturnsTrue()
        {
            // Arrange
            var doctorId = Guid.NewGuid().ToString();
            var prescriptionId = Guid.NewGuid();
            var prescription = new Prescription { Id = prescriptionId, DoctorId = doctorId, Status = PrescriptionStatus.Active };
            _seedPrescriptions.Add(prescription);
            _mockPrescriptionDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync(prescription);


            // Act
            var result = await _prescriptionService.CancelPrescriptionAsync(prescriptionId, doctorId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(PrescriptionStatus.Cancelled, prescription.Status);
        }

        [TestMethod]
        public async Task CancelPrescriptionAsync_NotOwnedByDoctor_ReturnsFalse()
        {
            // Arrange
            var ownerDoctorId = Guid.NewGuid().ToString();
            var otherDoctorId = Guid.NewGuid().ToString();
            var prescriptionId = Guid.NewGuid();
            var prescription = new Prescription { Id = prescriptionId, DoctorId = ownerDoctorId, Status = PrescriptionStatus.Active };
            _seedPrescriptions.Add(prescription);
            _mockPrescriptionDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync(prescription);

            // Act
            var result = await _prescriptionService.CancelPrescriptionAsync(prescriptionId, otherDoctorId);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(PrescriptionStatus.Active, prescription.Status);
        }

        [TestMethod]
        public async Task CancelPrescriptionAsync_NotActive_ReturnsFalse()
        {
            // Arrange
            var doctorId = Guid.NewGuid().ToString();
            var prescriptionId = Guid.NewGuid();
            var prescription = new Prescription { Id = prescriptionId, DoctorId = doctorId, Status = PrescriptionStatus.Dispensed };
            _seedPrescriptions.Add(prescription);
             _mockPrescriptionDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync(prescription);

            // Act
            var result = await _prescriptionService.CancelPrescriptionAsync(prescriptionId, doctorId);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(PrescriptionStatus.Dispensed, prescription.Status);
        }
    }
}
