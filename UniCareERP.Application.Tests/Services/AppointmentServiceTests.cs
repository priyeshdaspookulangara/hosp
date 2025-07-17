using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Application.Services.Appointments;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Appointment>> _mockAppointmentDbSet;
        private Mock<DbSet<Patient>> _mockPatientDbSet;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<AppointmentService>> _mockLogger;
        private AppointmentService _appointmentService;

        private List<Patient> _seedPatients;
        private List<ApplicationUser> _seedDoctors;
        private List<Appointment> _seedAppointments;

        // Helper to create Mock UserManager
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManager =  new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup FindByIdAsync and IsInRoleAsync
            userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync((string id) => _seedDoctors.FirstOrDefault(d => d.Id == id));
            userManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Doctor"))
                       .ReturnsAsync((ApplicationUser user, string role) => _seedDoctors.Any(d => d.Id == user.Id && role == "Doctor")); // Simple check
            return userManager;
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockAppointmentDbSet = new Mock<DbSet<Appointment>>();
            _mockPatientDbSet = new Mock<DbSet<Patient>>();
            _mockLogger = new Mock<ILogger<AppointmentService>>();

            // Seed Data
            _seedPatients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FirstName = "Pat", LastName = "One", PatientCode = "P00100" },
                new Patient { Id = Guid.NewGuid(), FirstName = "Pat", LastName = "Two", PatientCode = "P00101" }
            };
            _seedDoctors = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "doc1@test.com", FirstName = "Doctor", LastName = "Strange" },
                new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "doc2@test.com", FirstName = "Doctor", LastName = "Who" }
            };
            _seedAppointments = new List<Appointment>(); // Start empty or with some seed

            // Setup DbSets
            SetupMockDbSet(_mockAppointmentDbSet, _seedAppointments);
            SetupMockDbSet(_mockPatientDbSet, _seedPatients);

            _mockContext.Setup(c => c.Appointments).Returns(_mockAppointmentDbSet.Object);
            _mockContext.Setup(c => c.Patients).Returns(_mockPatientDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockUserManager = GetMockUserManager();
            _appointmentService = new AppointmentService(_mockContext.Object, _mockUserManager.Object, _mockLogger.Object);
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
                    if (typeof(TEntity) == typeof(Patient)) return sourceList.FirstOrDefault(e => (e as Patient).Id == (Guid)ids[0]) as TEntity;
                    if (typeof(TEntity) == typeof(Appointment)) return sourceList.FirstOrDefault(e => (e as Appointment).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
            mockDbSet.Setup(d => d.Remove(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Remove(s));
        }


        [TestMethod]
        public async Task ScheduleAppointmentAsync_ValidData_ReturnsAppointmentDto()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var createDto = new CreateAppointmentDto
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Today.AddDays(1).AddHours(10),
                DurationMinutes = 30,
                ServiceType = "Consultation"
            };

            // Act
            var resultDto = await _appointmentService.ScheduleAppointmentAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(patient.Id, resultDto.PatientId);
            Assert.AreEqual(doctor.Id, resultDto.DoctorId);
            Assert.AreEqual(AppointmentStatus.Scheduled, resultDto.Status);
            _mockAppointmentDbSet.Verify(db => db.Add(It.IsAny<Appointment>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task ScheduleAppointmentAsync_PatientNotFound_ReturnsNull()
        {
            // Arrange
            var doctor = _seedDoctors.First();
            var createDto = new CreateAppointmentDto
            {
                PatientId = Guid.NewGuid(), // Non-existent patient
                DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Today.AddDays(1).AddHours(10),
                DurationMinutes = 30
            };

            // Act
            var resultDto = await _appointmentService.ScheduleAppointmentAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            _mockAppointmentDbSet.Verify(db => db.Add(It.IsAny<Appointment>()), Times.Never());
        }

        [TestMethod]
        public async Task ScheduleAppointmentAsync_DoctorNotFound_ReturnsNull()
        {
            // Arrange
            var patient = _seedPatients.First();
            var createDto = new CreateAppointmentDto
            {
                PatientId = patient.Id,
                DoctorId = Guid.NewGuid().ToString(), // Non-existent doctor
                AppointmentDateTime = DateTime.Today.AddDays(1).AddHours(10),
                DurationMinutes = 30
            };

            // Act
            var resultDto = await _appointmentService.ScheduleAppointmentAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
        }

        [TestMethod]
        public async Task ScheduleAppointmentAsync_DoctorNotDoctorRole_ReturnsNull()
        {
            // Arrange
             var patient = _seedPatients.First();
             var nonDoctorUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName="notadoc@test.com"};
             _seedDoctors.Add(nonDoctorUser); // Add to list so FindByIdAsync finds it
             _mockUserManager.Setup(um => um.IsInRoleAsync(nonDoctorUser, "Doctor")).ReturnsAsync(false); // Explicitly not in role


            var createDto = new CreateAppointmentDto
            {
                PatientId = patient.Id,
                DoctorId = nonDoctorUser.Id,
                AppointmentDateTime = DateTime.Today.AddDays(1).AddHours(10),
                DurationMinutes = 30
            };
            // Act
            var resultDto = await _appointmentService.ScheduleAppointmentAsync(createDto);
            // Assert
            Assert.IsNull(resultDto);
        }


        [TestMethod]
        public async Task ScheduleAppointmentAsync_ConflictExists_ReturnsNull()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var existingApptTime = DateTime.Today.AddDays(1).AddHours(10);
            _seedAppointments.Add(new Appointment {
                Id = Guid.NewGuid(),
                PatientId = _seedPatients.Last().Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = existingApptTime,
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled
            });
            SetupMockDbSet(_mockAppointmentDbSet, _seedAppointments); //Re-setup with data

            var createDto = new CreateAppointmentDto
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = existingApptTime.AddMinutes(10), // Overlapping
                DurationMinutes = 30
            };

            // Act
            var resultDto = await _appointmentService.ScheduleAppointmentAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
        }


        [TestMethod]
        public async Task GetAppointmentByIdAsync_Exists_ReturnsDto()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var appointmentId = Guid.NewGuid();
            _seedAppointments.Add(new Appointment {
                Id = appointmentId,
                PatientId = patient.Id, Patient = patient,
                DoctorId = doctor.Id, Doctor = doctor,
                AppointmentDateTime = DateTime.Now, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            });
             SetupMockDbSet(_mockAppointmentDbSet, _seedAppointments);


            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointmentId, result.Id);
        }

        [TestMethod]
        public async Task UpdateAppointmentAsync_ValidUpdate_ReturnsUpdatedDto()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var appointmentId = Guid.NewGuid();
            var originalTime = DateTime.Today.AddDays(2).AddHours(11);
            _seedAppointments.Add(new Appointment {
                Id = appointmentId, PatientId = patient.Id, DoctorId = doctor.Id,
                AppointmentDateTime = originalTime, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            });
            SetupMockDbSet(_mockAppointmentDbSet, _seedAppointments);

            var updateDto = new UpdateAppointmentDto {
                Id = appointmentId, PatientId = patient.Id, DoctorId = doctor.Id,
                AppointmentDateTime = originalTime.AddHours(1), DurationMinutes = 45, Status = AppointmentStatus.Confirmed
            };

            // Act
            var result = await _appointmentService.UpdateAppointmentAsync(updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Confirmed, result.Status);
            Assert.AreEqual(45, result.DurationMinutes);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task CancelAppointmentAsync_Exists_ReturnsTrueAndSetsStatus()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var appointmentId = Guid.NewGuid();
            _seedAppointments.Add(new Appointment {
                Id = appointmentId, PatientId = patient.Id, DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Now, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            });
            SetupMockDbSet(_mockAppointmentDbSet, _seedAppointments);

            // Act
            var result = await _appointmentService.CancelAppointmentAsync(appointmentId, "Patient request", true);

            // Assert
            Assert.IsTrue(result);
            var cancelledAppt = _seedAppointments.First(a => a.Id == appointmentId);
            Assert.AreEqual(AppointmentStatus.CancelledByPatient, cancelledAppt.Status);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }

}
