using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Radiology;
using UniCareERP.Domain.Entities.Radiology;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Radiology
{
    public class RISService : IRISService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<RISService> _logger;

        public RISService(UniCareDbContext context, ILogger<RISService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<RadiologyImageDto> AddImageToOrderAsync(RadiologyImageDto imageDto)
        {
            throw new NotImplementedException();
        }

        public async Task<RadiologyOrderDto> CreateOrderAsync(RadiologyOrderDto orderDto)
        {
            var order = new RadiologyOrder
            {
                PatientId = orderDto.PatientId,
                TestTypeId = orderDto.TestTypeId,
                OrderDateTime = DateTime.UtcNow,
                Status = "Scheduled"
            };

            _context.RadiologyOrders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Radiology order created with ID: {order.Id}");

            return await GetOrderAsync(order.Id);
        }

        public Task<RadiologyReportDto> CreateReportAsync(RadiologyReportDto reportDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RadiologyImageDto>> GetImagesForOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<RadiologyOrderDto> GetOrderAsync(Guid orderId)
        {
            var order = await _context.RadiologyOrders
                .Include(o => o.Patient)
                .Include(o => o.TestType)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientId = order.PatientId,
                PatientName = order.Patient.FirstName + " " + order.Patient.LastName,
                TestTypeId = order.TestTypeId,
                TestTypeName = order.TestType.Name,
                OrderDateTime = order.OrderDateTime,
                Status = order.Status
            };
        }

        public async Task<IEnumerable<RadiologyOrderDto>> GetOrdersAsync()
        {
            return await _context.RadiologyOrders
                .Include(o => o.Patient)
                .Include(o => o.TestType)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    PatientName = o.Patient.FirstName + " " + o.Patient.LastName,
                    TestTypeId = o.TestTypeId,
                    TestTypeName = o.TestType.Name,
                    OrderDateTime = o.OrderDateTime,
                    Status = o.Status
                })
                .ToListAsync();
        }

        public Task<RadiologyReportDto> GetReportAsync(Guid reportId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RadiologyReportDto>> GetReportsForOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RadiologyTestDto>> GetTestTypesAsync()
        {
            return await _context.RadiologyTests
                .Select(t => new RadiologyTestDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                })
                .ToListAsync();
        }
    }
}
