using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Radiology;
using UniCareERP.Application.Services.Radiology;
using UniCareERP.Application.Services.Patients;
using UniCareERP.Web.Controllers;

namespace UniCareERP.Web.Tests.Controllers
{
    [TestClass]
    public class RadiologyControllerTests
    {
        private Mock<IRISService> _mockRisService;
        private Mock<IPatientService> _mockPatientService;
        private RadiologyController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRisService = new Mock<IRISService>();
            _mockPatientService = new Mock<IPatientService>();
            _controller = new RadiologyController(_mockRisService.Object, _mockPatientService.Object);
        }

        [TestMethod]
        public async Task Index_ReturnsAViewResult_WithAListOfRadiologyOrders()
        {
            // Arrange
            _mockRisService.Setup(service => service.GetOrdersAsync())
                .ReturnsAsync(new List<RadiologyOrderDto>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<RadiologyOrderDto>));
        }

        [TestMethod]
        public async Task Create_ReturnsAViewResult()
        {
            // Arrange
            _mockPatientService.Setup(service => service.GetAllPatientsAsync())
                .ReturnsAsync(new List<Application.DTOs.Patients.PatientDto>());
            _mockRisService.Setup(service => service.GetTestTypesAsync())
                .ReturnsAsync(new List<RadiologyTestDto>());

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
