using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Enums;
using Xunit;
using Newtonsoft.Json;

namespace UniCareERP.Web.Tests.Controllers
{
    public class PatientBillingControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PatientBillingControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ProcessPayment_Should_ReturnSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();
            var paymentDto = new PatientPaymentDto
            {
                PatientId = Guid.NewGuid(),
                PaymentDate = DateTime.UtcNow,
                Amount = 100,
                PaymentMethod = PaymentMethod.Cash
            };
            var content = new StringContent(JsonConvert.SerializeObject(paymentDto), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/patientbilling/process-payment", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
