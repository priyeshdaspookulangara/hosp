using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using UniCareERP.Application.DTOs.OperationTheatre;
using UniCareERP.Application.Services.OperationTheatre;
using UniCareERP.Domain.Entities.OperationTheatre;
using UniCareERP.Infrastructure.Data;
using Xunit;

namespace UniCareERP.Application.Tests.Services.OperationTheatre
{
    public class OperationTheatreServiceTests
    {
        private readonly DbContextOptions<UniCareDbContext> _dbContextOptions;

        public OperationTheatreServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Domain.Entities.OperationTheatre.OperationTheatre CreateTestOperationTheatre(string name = "OT 1")
        {
            return new Domain.Entities.OperationTheatre.OperationTheatre
            {
                Name = name,
                RoomNumber = "101",
                Location = "First Floor",
                Equipment = "Standard Equipment"
            };
        }

        [Fact]
        public async Task GetAllOperationTheatresAsync_ShouldReturnAllOperationTheatres()
        {
            // Arrange
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                context.OperationTheatres.Add(CreateTestOperationTheatre("OT 1"));
                context.OperationTheatres.Add(CreateTestOperationTheatre("OT 2"));
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OperationTheatreService(context);

                // Act
                var result = await service.GetAllOperationTheatresAsync();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetOperationTheatreByIdAsync_ShouldReturnOperationTheatre()
        {
            // Arrange
            var id = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var ot = CreateTestOperationTheatre();
                ot.Id = id;
                context.OperationTheatres.Add(ot);
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OperationTheatreService(context);

                // Act
                var result = await service.GetOperationTheatreByIdAsync(id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("OT 1", result.Name);
            }
        }

        [Fact]
        public async Task CreateOperationTheatreAsync_ShouldCreateOperationTheatre()
        {
            // Arrange
            var createDto = new CreateOperationTheatreDto { Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" };
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OperationTheatreService(context);

                // Act
                var result = await service.CreateOperationTheatreAsync(createDto);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("OT 1", result.Name);
            }
        }

        [Fact]
        public async Task UpdateOperationTheatreAsync_ShouldUpdateOperationTheatre()
        {
            // Arrange
            var id = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var ot = CreateTestOperationTheatre();
                ot.Id = id;
                context.OperationTheatres.Add(ot);
                context.SaveChanges();
            }
            var updateDto = new UpdateOperationTheatreDto { Id = id, Name = "OT 1 Updated", RoomNumber = "102", Location = "Second Floor", Equipment = "Advanced Equipment" };

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OperationTheatreService(context);

                // Act
                var result = await service.UpdateOperationTheatreAsync(updateDto);

                // Assert
                Assert.True(result);
                var updatedEntity = await context.OperationTheatres.FindAsync(id);
                Assert.Equal("OT 1 Updated", updatedEntity.Name);
            }
        }

        [Fact]
        public async Task DeleteOperationTheatreAsync_ShouldDeleteOperationTheatre()
        {
            // Arrange
            var id = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var ot = CreateTestOperationTheatre();
                ot.Id = id;
                context.OperationTheatres.Add(ot);
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OperationTheatreService(context);

                // Act
                var result = await service.DeleteOperationTheatreAsync(id);

                // Assert
                Assert.True(result);
                var deletedEntity = await context.OperationTheatres.FindAsync(id);
                Assert.Null(deletedEntity);
            }
        }
    }
}
