using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UniCareERP.Application.DTOs;
using UniCareERP.Application.Services;
using UniCareERP.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniCareERP.Application.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<ApplicationRole>> _mockRoleManager;
        private UserService _userService;

        // Helper to create Mock UserManager
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        // Helper to create Mock RoleManager
        private Mock<RoleManager<ApplicationRole>> GetMockRoleManager()
        {
            var roleStoreMock = new Mock<IRoleStore<ApplicationRole>>();
            return new Mock<RoleManager<ApplicationRole>>(
                roleStoreMock.Object, null, null, null, null);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _mockUserManager = GetMockUserManager();
            _mockRoleManager = GetMockRoleManager();
            _userService = new UserService(_mockUserManager.Object, _mockRoleManager.Object);
        }

        [TestMethod]
        public async Task CreateUserAsync_SuccessfulCreation_ReturnsSuccess()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                Roles = new List<string> { "Doctor" }
            };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createUserDto.Roles))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var (success, userId, errors) = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(userId); // UserId is generated within the method by Identity
            Assert.IsFalse(errors.Any());
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password), Times.Once);
            _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createUserDto.Roles), Times.Once);
        }

        [TestMethod]
        public async Task CreateUserAsync_UserManagerFails_ReturnsFailureAndErrors()
        {
            // Arrange
            var createUserDto = new CreateUserDto { UserName = "failuser", Email = "fail@example.com", Password = "Password123!" };
            var identityErrors = new List<IdentityError> { new IdentityError { Code = "Error", Description = "User creation failed" } };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var (success, userId, errors) = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNull(userId);
            Assert.IsTrue(errors.Any());
            Assert.AreEqual("User creation failed", errors.First());
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password), Times.Once);
            _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never); // Should not be called if user creation fails
        }

        [TestMethod]
        public async Task CreateUserAsync_AddToRolesFails_ReturnsFailureAndErrors()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                UserName = "rolefailuser",
                Email = "rolefail@example.com",
                Password = "Password123!",
                Roles = new List<string> { "NonExistentRole" }
            };
            var roleErrors = new List<IdentityError> { new IdentityError { Code = "RoleError", Description = "Role assignment failed" } };

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password))
                .ReturnsAsync(IdentityResult.Success); // User creation succeeds

            _mockUserManager.Setup(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createUserDto.Roles))
                .ReturnsAsync(IdentityResult.Failed(roleErrors.ToArray())); // Role assignment fails

            // Act
            var (success, userId, errors) = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNotNull(userId); // User was created, so ID should be there
            Assert.IsTrue(errors.Any());
            Assert.AreEqual("Role assignment failed", errors.First());
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password), Times.Once);
            _mockUserManager.Verify(um => um.AddToRolesAsync(It.IsAny<ApplicationUser>(), createUserDto.Roles), Times.Once);
        }
    }
}
