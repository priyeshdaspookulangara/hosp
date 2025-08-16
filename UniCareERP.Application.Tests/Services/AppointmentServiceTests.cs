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
using UniCareERP.Application.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private UniCareDbContext _context;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<AppointmentService>> _mockLogger;
        private AppointmentService _appointmentService;

        private List<Patient> _seedPatients;
        private List<ApplicationUser> _seedDoctors;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new UniCareDbContext(options);
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

            _context.Patients.AddRange(_seedPatients);
            _context.Users.AddRange(_seedDoctors);
            _context.SaveChanges();

            _mockUserManager = MockHelpers.GetMockUserManager();
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync((string id) => _context.Users.FirstOrDefault(d => d.Id == id));
            _mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Doctor"))
                       .ReturnsAsync((ApplicationUser user, string role) => _context.Users.Any(d => d.Id == user.Id && role == "Doctor")); // Simple check
            _appointmentService = new AppointmentService(_context, _mockUserManager.Object, _mockLogger.Object);
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
            Assert.AreEqual(1, _context.Appointments.Count());
            Assert.AreEqual(patient.Id, _context.Appointments.First().PatientId);
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
            Assert.AreEqual(0, _context.Appointments.Count());
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
            var existingAppointment = new Appointment {
                Id = Guid.NewGuid(),
                PatientId = _seedPatients.Last().Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = existingApptTime,
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled
            };
            _context.Appointments.Add(existingAppointment);
            _context.SaveChanges();

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
            var appointment = new Appointment {
                Id = appointmentId,
                PatientId = patient.Id, Patient = patient,
                DoctorId = doctor.Id, Doctor = doctor,
                AppointmentDateTime = DateTime.Now, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            };
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

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
            var appointment = new Appointment {
                Id = appointmentId, PatientId = patient.Id, DoctorId = doctor.Id,
                AppointmentDateTime = originalTime, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            };
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

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
        }

        [TestMethod]
        public async Task CancelAppointmentAsync_Exists_ReturnsTrueAndSetsStatus()
        {
            // Arrange
            var patient = _seedPatients.First();
            var doctor = _seedDoctors.First();
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment {
                Id = appointmentId, PatientId = patient.Id, DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Now, DurationMinutes = 30, Status = AppointmentStatus.Scheduled
            };
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            // Act
            var result = await _appointmentService.CancelAppointmentAsync(appointmentId, "Patient request", true);

            // Assert
            Assert.IsTrue(result);
            var cancelledAppt = await _context.Appointments.FindAsync(appointmentId);
            Assert.AreEqual(AppointmentStatus.CancelledByPatient, cancelledAppt.Status);
        }
    }

}
