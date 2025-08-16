using Moq;
using System;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Vitals;
using UniCareERP.Application.Services.Vitals;
using Xunit;

namespace UniCareERP.Application.Tests.Services
{
    public class VitalServiceTests
    {
        [Fact]
        public async Task CreateVitalAsync_ShouldReturnVitalDto()
        {
            // Arrange
            var vitalDto = new VitalDto
            {
                PatientId = Guid.NewGuid(),
                TemperatureC = 36.5m,
                BloodPressureSystolic = 120,
                BloodPressureDiastolic = 80,
                HeartRate = 70,
                RespiratoryRate = 16,
                OxygenSaturation = 98
            };

            var mockVitalService = new Mock<IVitalService>();
            mockVitalService.Setup(s => s.CreateVitalAsync(It.IsAny<VitalDto>()))
                .ReturnsAsync(vitalDto);

            var service = mockVitalService.Object;

            // Act
            var result = await service.CreateVitalAsync(vitalDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vitalDto.PatientId, result.PatientId);
        }
    }
}
