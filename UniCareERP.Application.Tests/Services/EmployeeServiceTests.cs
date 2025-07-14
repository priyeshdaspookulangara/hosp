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

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private Mock<UniCareDbContext> _mockContext;
        private Mock<DbSet<Employee>> _mockEmployeeDbSet;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<EmployeeService>> _mockLogger;
        private EmployeeService _employeeService;

        private List<Employee> _seedEmployees;
        private List<ApplicationUser> _seedUsers;

        // Helper to create Mock UserManager
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            return userManager;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockContext = new Mock<UniCareDbContext>(new DbContextOptions<UniCareDbContext>());
            _mockEmployeeDbSet = new Mock<DbSet<Employee>>();
            _mockUserManager = GetMockUserManager();
            _mockLogger = new Mock<ILogger<EmployeeService>>();

            _seedEmployees = new List<Employee>();
            _seedUsers = new List<ApplicationUser>();

            SetupMockDbSet(_mockEmployeeDbSet, _seedEmployees);
            _mockContext.Setup(c => c.Employees).Returns(_mockEmployeeDbSet.Object);

            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            _mockContext.Setup(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockTransaction.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _employeeService = new EmployeeService(_mockContext.Object, _mockUserManager.Object, _mockLogger.Object);
        }

        private void SetupMockDbSet<TEntity>(Mock<DbSet<TEntity>> mockDbSet, List<TEntity> sourceList) where TEntity : class
        {
            var queryableList = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
            mockDbSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(s => sourceList.Add(s));
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
                            .Callback<ApplicationUser, string>((user, pass) => _seedUsers.Add(user)); // Simulate user creation
            _mockUserManager.Setup(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createDto.Roles))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var resultDto = await _employeeService.CreateEmployeeAsync(createDto);

            // Assert
            Assert.IsNotNull(resultDto);
            Assert.AreEqual("New Employee", resultDto.FullName);
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createDto.Password), Times.Once);
            _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createDto.Roles), Times.Once);
            _mockEmployeeDbSet.Verify(db => db.Add(It.IsAny<Employee>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once); // Called once by EmployeeService
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
            _mockEmployeeDbSet.Verify(db => db.Add(It.IsAny<Employee>()), Times.Never);
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
            _mockEmployeeDbSet.Verify(db => db.Add(It.IsAny<Employee>()), Times.Never);
        }

        [TestMethod]
        public async Task DeactivateEmployeeAsync_ValidId_SetsBothEmployeeAndUserToInactive()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var employeeId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, IsActive = true };
            var employee = new Employee { Id = employeeId, ApplicationUserId = userId, IsActive = true, ApplicationUser = user };
            _seedEmployees.Add(employee);

            // Mock FindAsync for Employee
             var queryableList = _seedEmployees.AsQueryable();
            _mockEmployeeDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            _mockEmployeeDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            _mockEmployeeDbSet.As<IQueryable<Employee>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            _mockEmployeeDbSet.As<IQueryable<Employee>>().Setup(m => m.GetEnumerator()).Returns(() => queryableList.GetEnumerator());

            // Act
            var result = await _employeeService.DeactivateEmployeeAsync(employeeId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(employee.IsActive);
            Assert.IsFalse(user.IsActive);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
    }
}
