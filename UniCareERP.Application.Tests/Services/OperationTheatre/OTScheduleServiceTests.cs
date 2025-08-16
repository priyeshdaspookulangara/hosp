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
using UniCareERP.Domain.Entities.Patients;

namespace UniCareERP.Application.Tests.Services.OperationTheatre
{
    public class OTScheduleServiceTests
    {
        private readonly DbContextOptions<UniCareDbContext> _dbContextOptions;

        public OTScheduleServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private OTSchedule CreateTestOTSchedule(Guid otId, Guid spId, Guid pId, Guid stId, string notes = "Schedule 1")
        {
            return new OTSchedule
            {
                OperationTheatreId = otId,
                SurgicalProcedureId = spId,
                PatientId = pId,
                SurgicalTeamId = stId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Notes = notes
            };
        }

        [Fact]
        public async Task GetAllOTSchedulesAsync_ShouldReturnAllOTSchedules()
        {
            // Arrange
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var ot = new Domain.Entities.OperationTheatre.OperationTheatre { Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" };
                var sp = new SurgicalProcedure { Name = "Procedure 1", Description = "Desc 1", RequiredEquipment = "Equip 1", DurationMinutes = 60 };
                var p = new Patient { FirstName = "John", LastName = "Doe" };
                var st = new SurgicalTeam { Name = "Team 1" };
                context.OperationTheatres.Add(ot);
                context.SurgicalProcedures.Add(sp);
                context.Patients.Add(p);
                context.SurgicalTeams.Add(st);
                context.OTSchedules.Add(CreateTestOTSchedule(ot.Id, sp.Id, p.Id, st.Id, "Schedule 1"));
                context.OTSchedules.Add(CreateTestOTSchedule(ot.Id, sp.Id, p.Id, st.Id, "Schedule 2"));
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OTScheduleService(context);

                // Act
                var result = await service.GetAllOTSchedulesAsync();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetOTScheduleByIdAsync_ShouldReturnOTSchedule()
        {
            // Arrange
            var id = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var ot = new Domain.Entities.OperationTheatre.OperationTheatre { Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" };
                var sp = new SurgicalProcedure { Name = "Procedure 1", Description = "Desc 1", RequiredEquipment = "Equip 1", DurationMinutes = 60 };
                var p = new Patient { FirstName = "John", LastName = "Doe" };
                var st = new SurgicalTeam { Name = "Team 1" };
                context.OperationTheatres.Add(ot);
                context.SurgicalProcedures.Add(sp);
                context.Patients.Add(p);
                context.SurgicalTeams.Add(st);
                var schedule = CreateTestOTSchedule(ot.Id, sp.Id, p.Id, st.Id);
                schedule.Id = id;
                context.OTSchedules.Add(schedule);
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OTScheduleService(context);

                // Act
                var result = await service.GetOTScheduleByIdAsync(id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Schedule 1", result.Notes);
            }
        }

        [Fact]
        public async Task CreateOTScheduleAsync_ShouldCreateOTSchedule()
        {
            // Arrange
            var otId = Guid.NewGuid();
            var spId = Guid.NewGuid();
            var pId = Guid.NewGuid();
            var stId = Guid.NewGuid();
            var createDto = new CreateOTScheduleDto { OperationTheatreId = otId, SurgicalProcedureId = spId, PatientId = pId, SurgicalTeamId = stId, Notes = "Schedule 1" };
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                context.OperationTheatres.Add(new Domain.Entities.OperationTheatre.OperationTheatre { Id = otId, Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" });
                context.SurgicalProcedures.Add(new SurgicalProcedure { Id = spId, Name = "Procedure 1", Description = "Desc 1", RequiredEquipment = "Equip 1", DurationMinutes = 60 });
                context.Patients.Add(new Patient { Id = pId, FirstName = "John", LastName = "Doe" });
                context.SurgicalTeams.Add(new SurgicalTeam { Id = stId, Name = "Team 1" });
                context.SaveChanges();
            }
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OTScheduleService(context);

                // Act
                var result = await service.CreateOTScheduleAsync(createDto);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Schedule 1", result.Notes);
            }
        }

        [Fact]
        public async Task UpdateOTScheduleAsync_ShouldUpdateOTSchedule()
        {
            // Arrange
            var id = Guid.NewGuid();
            var otId = Guid.NewGuid();
            var spId = Guid.NewGuid();
            var pId = Guid.NewGuid();
            var stId = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                context.OperationTheatres.Add(new Domain.Entities.OperationTheatre.OperationTheatre { Id = otId, Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" });
                context.SurgicalProcedures.Add(new SurgicalProcedure { Id = spId, Name = "Procedure 1", Description = "Desc 1", RequiredEquipment = "Equip 1", DurationMinutes = 60 });
                context.Patients.Add(new Patient { Id = pId, FirstName = "John", LastName = "Doe" });
                context.SurgicalTeams.Add(new SurgicalTeam { Id = stId, Name = "Team 1" });
                var schedule = CreateTestOTSchedule(otId, spId, pId, stId);
                schedule.Id = id;
                context.OTSchedules.Add(schedule);
                context.SaveChanges();
            }
            var updateDto = new UpdateOTScheduleDto { Id = id, OperationTheatreId = otId, SurgicalProcedureId = spId, PatientId = pId, SurgicalTeamId = stId, Notes = "Schedule 1 Updated" };

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OTScheduleService(context);

                // Act
                var result = await service.UpdateOTScheduleAsync(updateDto);

                // Assert
                Assert.True(result);
                var updatedEntity = await context.OTSchedules.FindAsync(id);
                Assert.Equal("Schedule 1 Updated", updatedEntity.Notes);
            }
        }

        [Fact]
        public async Task DeleteOTScheduleAsync_ShouldDeleteOTSchedule()
        {
            // Arrange
            var id = Guid.NewGuid();
            var otId = Guid.NewGuid();
            var spId = Guid.NewGuid();
            var pId = Guid.NewGuid();
            var stId = Guid.NewGuid();
            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                context.OperationTheatres.Add(new Domain.Entities.OperationTheatre.OperationTheatre { Id = otId, Name = "OT 1", RoomNumber = "101", Location = "First Floor", Equipment = "Standard Equipment" });
                context.SurgicalProcedures.Add(new SurgicalProcedure { Id = spId, Name = "Procedure 1", Description = "Desc 1", RequiredEquipment = "Equip 1", DurationMinutes = 60 });
                context.Patients.Add(new Patient { Id = pId, FirstName = "John", LastName = "Doe" });
                context.SurgicalTeams.Add(new SurgicalTeam { Id = stId, Name = "Team 1" });
                var schedule = CreateTestOTSchedule(otId, spId, pId, stId);
                schedule.Id = id;
                context.OTSchedules.Add(schedule);
                context.SaveChanges();
            }

            using (var context = new UniCareDbContext(_dbContextOptions))
            {
                var service = new OTScheduleService(context);

                // Act
                var result = await service.DeleteOTScheduleAsync(id);

                // Assert
                Assert.True(result);
                var deletedEntity = await context.OTSchedules.FindAsync(id);
                Assert.Null(deletedEntity);
            }
        }
    }
}
