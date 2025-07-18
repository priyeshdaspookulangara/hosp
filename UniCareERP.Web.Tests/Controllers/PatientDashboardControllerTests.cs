using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.PatientDashboard;
using UniCareERP.Application.Services.PatientDashboard;
using UniCareERP.Web.Controllers;
using Xunit;

namespace UniCareERP.Web.Tests.Controllers
{
    public class PatientDashboardControllerTests
    {
        [Fact]
        public async Task Index_ReturnsNotFound_WhenDashboardDataIsNull()
        {
            // Arrange
            var mockService = new Mock<IPatientDashboardService>();
            mockService.Setup(s => s.GetPatientDashboardAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PatientDashboardDto)null);
            var controller = new PatientDashboardController(mockService.Object);

            // Act
            var result = await controller.Index(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithDashboardData()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dashboardData = new PatientDashboardDto();
            var mockService = new Mock<IPatientDashboardService>();
            mockService.Setup(s => s.GetPatientDashboardAsync(patientId))
                .ReturnsAsync(dashboardData);
            var controller = new PatientDashboardController(mockService.Object);

            // Act
            var result = await controller.Index(patientId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(dashboardData, viewResult.Model);
        }
    }
}
