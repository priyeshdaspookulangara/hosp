using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.HR;
using UniCareERP.Application.Services.HR;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Domain.Enums;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class LeaveRequestServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<LeaveRequest>> _mockLeaveDbSet;
        private Mock<ILogger<LeaveRequestService>> _mockLogger;
        private LeaveRequestService _leaveRequestService;

        private List<LeaveRequest> _seedLeaveRequests;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockLeaveDbSet = new Mock<DbSet<LeaveRequest>>();
            _mockLogger = new Mock<ILogger<LeaveRequestService>>();

            _seedLeaveRequests = new List<LeaveRequest>();

            SetupMockDbSet(_mockLeaveDbSet, _seedLeaveRequests);
            _mockContext.Setup(c => c.LeaveRequests).Returns(_mockLeaveDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _leaveRequestService = new LeaveRequestService(_mockContext.Object, _mockLogger.Object);
        }

        private void SetupMockDbSet<TEntity>(Mock<DbSet<TEntity>> mockDbSet, List<TEntity> sourceList) where TEntity : class
        {
            var queryableList = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => {
                    if (typeof(TEntity) == typeof(LeaveRequest)) return sourceList.FirstOrDefault(e => (e as LeaveRequest).Id == (Guid)ids[0]) as TEntity;
                    return null;
                });
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
        }

        [TestMethod]
        public async Task CreateLeaveRequestAsync_ValidDto_ReturnsLeaveRequestDto()
        {
            // Arrange
            var createDto = new CreateLeaveRequestDto
            {
                EmployeeId = Guid.NewGuid(),
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(7),
                LeaveType = LeaveType.Annual,
                Reason = "Vacation"
            };

            // Act
            var resultDto = await _leaveRequestService.CreateLeaveRequestAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(LeaveRequestStatus.Pending, resultDto.Status);
            Assert.AreEqual("Vacation", resultDto.Reason);
            _mockLeaveDbSet.Verify(db => db.Add(It.IsAny<LeaveRequest>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateLeaveRequestAsync_StartDateAfterEndDate_ReturnsNull()
        {
            // Arrange
            var createDto = new CreateLeaveRequestDto
            {
                EmployeeId = Guid.NewGuid(),
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(5), // Invalid dates
                LeaveType = LeaveType.Annual,
                Reason = "Invalid Vacation"
            };

            // Act
            var resultDto = await _leaveRequestService.CreateLeaveRequestAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            _mockLeaveDbSet.Verify(db => db.Add(It.IsAny<LeaveRequest>()), Times.Never);
        }

        [TestMethod]
        public async Task ApproveLeaveRequestAsync_PendingRequest_ReturnsTrueAndSetsStatusToApproved()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var approverId = Guid.NewGuid().ToString();
            var request = new LeaveRequest { Id = requestId, Status = LeaveRequestStatus.Pending };
            _seedLeaveRequests.Add(request);

            // Act
            var result = await _leaveRequestService.ApproveLeaveRequestAsync(requestId, approverId, "Approved");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(LeaveRequestStatus.Approved, request.Status);
            Assert.AreEqual(approverId, request.ApprovedByUserId);
            Assert.IsNotNull(request.ActionDate);
        }

        [TestMethod]
        public async Task ApproveLeaveRequestAsync_AlreadyApprovedRequest_ReturnsFalse()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var approverId = Guid.NewGuid().ToString();
            var request = new LeaveRequest { Id = requestId, Status = LeaveRequestStatus.Approved };
            _seedLeaveRequests.Add(request);

            // Act
            var result = await _leaveRequestService.ApproveLeaveRequestAsync(requestId, approverId, "Approved again");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RejectLeaveRequestAsync_PendingRequest_ReturnsTrueAndSetsStatusToRejected()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var approverId = Guid.NewGuid().ToString();
            var request = new LeaveRequest { Id = requestId, Status = LeaveRequestStatus.Pending };
            _seedLeaveRequests.Add(request);

            // Act
            var result = await _leaveRequestService.RejectLeaveRequestAsync(requestId, approverId, "Not enough staff");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(LeaveRequestStatus.Rejected, request.Status);
            Assert.AreEqual("Not enough staff", request.ApproverComments);
        }

        [TestMethod]
        public async Task CancelLeaveRequestAsync_OwnedByEmployeeAndPending_ReturnsTrue()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new LeaveRequest { Id = requestId, EmployeeId = employeeId, Status = LeaveRequestStatus.Pending };
            _seedLeaveRequests.Add(request);

            // Act
            var result = await _leaveRequestService.CancelLeaveRequestAsync(requestId, employeeId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(LeaveRequestStatus.Cancelled, request.Status);
        }

        [TestMethod]
        public async Task CancelLeaveRequestAsync_NotOwnedByEmployee_ReturnsFalse()
        {
            // Arrange
            var ownerEmployeeId = Guid.NewGuid();
            var otherEmployeeId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var request = new LeaveRequest { Id = requestId, EmployeeId = ownerEmployeeId, Status = LeaveRequestStatus.Pending };
            _seedLeaveRequests.Add(request);

            // Act
            var result = await _leaveRequestService.CancelLeaveRequestAsync(requestId, otherEmployeeId);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(LeaveRequestStatus.Pending, request.Status);
        }
    }
}
