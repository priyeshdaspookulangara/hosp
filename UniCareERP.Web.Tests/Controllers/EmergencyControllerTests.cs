using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Emergency;
using UniCareERP.Application.Services.Emergency;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Web.Controllers;

namespace UniCareERP.Web.Tests.Controllers
{
    [TestClass]
    public class EmergencyControllerTests
    {
        private Mock<IEmergencyCaseService> _mockEmergencyCaseService;
        private Mock<IPatientService> _mockPatientService;
        private EmergencyController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockEmergencyCaseService = new Mock<IEmergencyCaseService>();
            _mockPatientService = new Mock<IPatientService>();
            _controller = new EmergencyController(_mockEmergencyCaseService.Object, _mockPatientService.Object);
        }

        [TestMethod]
        public async Task Index_ReturnsAViewResult_WithAListOfEmergencyCases()
        {
            // Arrange
            _mockEmergencyCaseService.Setup(service => service.GetAllEmergencyCasesAsync())
                .ReturnsAsync(new List<EmergencyCaseDto>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<EmergencyCaseDto>));
        }

        [TestMethod]
        public async Task Create_ReturnsAViewResult()
        {
            // Arrange
            _mockPatientService.Setup(service => service.GetAllPatientsAsync())
                .ReturnsAsync(new List<Application.DTOs.Patients.PatientDto>());

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFoundResult_WhenCaseNotFound()
        {
            // Arrange
            _mockEmergencyCaseService.Setup(service => service.GetEmergencyCaseByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((EmergencyCaseDto)null);

            // Act
            var result = await _controller.Edit(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsAViewResult_WithEmergencyCase()
        {
            // Arrange
            var emergencyCaseId = Guid.NewGuid();
            _mockEmergencyCaseService.Setup(service => service.GetEmergencyCaseByIdAsync(emergencyCaseId))
                .ReturnsAsync(new EmergencyCaseDto { Id = emergencyCaseId });

            // Act
            var result = await _controller.Details(emergencyCaseId);

            // Assert
            var viewResult = Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EmergencyCaseDto));
            Assert.AreEqual(emergencyCaseId, model.Id);
        }
    }
}
