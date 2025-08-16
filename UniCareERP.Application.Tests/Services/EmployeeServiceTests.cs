using Microsoft.AspNetCore.Identity;
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
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Infrastructure.Data;
using Microsoft.Extensions.Options;
using UniCareERP.Application.Tests.Helpers;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private UniCareDbContext _context;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<EmployeeService>> _mockLogger;
        private EmployeeService _employeeService;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<UniCareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new UniCareDbContext(options);
            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var mockDatabase = new Mock<DatabaseFacade>(_context);
            mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            var mockContext = new Mock<UniCareDbContext>(options);
            mockContext.Setup(c => c.Database).Returns(mockDatabase.Object);

            _mockUserManager = MockHelpers.GetMockUserManager();
            _mockLogger = new Mock<ILogger<EmployeeService>>();

            _employeeService = new EmployeeService(mockContext.Object, _mockUserManager.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_ValidData_CreatesUserAndEmployee()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Email = "new.employee@test.com",
                Password = "Password123!",
                FirstName = "New",
                LastName = "Employee",
                DateOfBirth = new DateTime(1995, 1, 1),
                Gender = "Female",
                Roles = new List<string> { "Nurse" }
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(createDto.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password))
                            .ReturnsAsync(IdentityResult.Success)
                            .Callback<ApplicationUser, string>((user, pass) => _context.Users.Add(user)); // Simulate user creation
            _mockUserManager.Setup(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createDto.Roles))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var resultDto = await _employeeService.CreateEmployeeAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("New Employee", resultDto.FullName);
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password), Times.Once);
            _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createDto.Roles), Times.Once);
            Assert.AreEqual(1, _context.Employees.Count());
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_UserCreationFails_ReturnsNullAndRollsBack()
        {
            // Arrange
            var createDto = new CreateEmployeeDto { Email = "fail@test.com", Password = "pw", FirstName = "Fail", LastName = "User" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(createDto.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too short" }));

            // Act
            var resultDto = await _employeeService.CreateEmployeeAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            Assert.AreEqual(0, _context.Employees.Count());
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_RoleAssignmentFails_ReturnsNullAndRollsBack()
        {
            // Arrange
            var createDto = new CreateEmployeeDto { Email = "rolefail@test.com", Password = "Password123!", FirstName = "Role", LastName = "Fail", Roles = new List<string>{"InvalidRole"} };
             _mockUserManager.Setup(um => um.FindByEmailAsync(createDto.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createDto.Roles))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role does not exist" }));

            // Act
            var resultDto = await _employeeService.CreateEmployeeAsync(createDto);

            // Assert
            Assert.IsNull(resultDto);
            Assert.AreEqual(0, _context.Employees.Count());
        }

        [TestMethod]
        public async Task DeactivateEmployeeAsync_ValidId_SetsBothEmployeeAndUserToInactive()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var employeeId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, IsActive = true };
            var employee = new Employee { Id = employeeId, ApplicationUserId = userId, IsActive = true, ApplicationUser = user };
            _context.Employees.Add(employee);
            _context.SaveChanges();

            // Act
            var result = await _employeeService.DeactivateEmployeeAsync(employeeId);

            // Assert
            Assert.IsTrue(result);
            var deactivatedEmployee = await _context.Employees.FindAsync(employeeId);
            Assert.IsFalse(deactivatedEmployee.IsActive);
            Assert.IsFalse(deactivatedEmployee.ApplicationUser.IsActive);
        }
    }
}
