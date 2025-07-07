using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Appointments;
using UniCareERP.Application.Services.Appointments;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Enums;
using UniCareERP.Web.Controllers; // Make sure this using is correct for your project structure
using Microsoft.AspNetCore.Http; // Required for Url.Action mocking

namespace UniCareERP.Web.Tests.Controllers
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private Mock<IAppointmentService> _mockAppointmentService;
        private Mock<IPatientService> _mockPatientService;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<AppointmentsController>> _mockLogger;
        private AppointmentsController _controller;

        // Helper to create Mock UserManager
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAppointmentService = new Mock<IAppointmentService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockUserManager = GetMockUserManager();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();

            _controller = new AppointmentsController(
                _mockAppointmentService.Object,
                _mockPatientService.Object,
                _mockUserManager.Object,
                _mockLogger.Object);

            // Mock Url.Action - This is a common way to handle UrlHelper dependencies in controllers
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                         .Returns("/fake/url"); // Return a dummy URL
            _controller.Url = urlHelperMock.Object;
        }

        [TestMethod]
        public async Task GetCalendarEvents_ReturnsCorrectlyFormattedJson()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);
            var appointmentId = Guid.NewGuid();

            var appointmentsDto = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    Id = appointmentId,
                    PatientName = "John Doe",
                    PatientCode = "P001",
                    ServiceType = "Checkup",
                    AppointmentDateTime = startDate.AddHours(10),
                    DurationMinutes = 30, // EndDateTime will be 10:30
                    Status = AppointmentStatus.Scheduled
                }
            };

            _mockAppointmentService.Setup(s => s.GetAppointmentsByDateRangeAsync(startDate, endDate))
                                   .ReturnsAsync(appointmentsDto);

            // Act
            var result = await _controller.GetCalendarEvents(startDate, endDate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult?.Value);

            var events = jsonResult.Value as IEnumerable<object>;
            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Count());

            var eventData = events.First();
            dynamic eventObj = eventData; // Use dynamic to access properties of anonymous type

            Assert.AreEqual(appointmentId.ToString(), eventObj.id);
            Assert.AreEqual("John Doe (P001) - Checkup", eventObj.title);
            Assert.AreEqual(startDate.AddHours(10).ToString("o"), eventObj.start);
            Assert.AreEqual(startDate.AddHours(10).AddMinutes(30).ToString("o"), eventObj.end);
            Assert.AreEqual("/fake/url", eventObj.url); // From mocked Url.Action
            Assert.IsFalse(eventObj.allDay);
        }
    }
}
