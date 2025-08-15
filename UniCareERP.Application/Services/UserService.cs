using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Application.DTOs;
using UniCareERP.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniCareERP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(bool Success, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new ApplicationUser
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                EmployeeId = createUserDto.EmployeeId,
                IsActive = createUserDto.IsActive,
                EmailConfirmed = true, // Assuming email confirmation is not strictly required initially or handled elsewhere
                Password = createUserDto.Password
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return (false, null, result.Errors.Select(e => e.Description));
            }

            if (createUserDto.Roles != null && createUserDto.Roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(user, createUserDto.Roles);
                if (!roleResult.Succeeded)
                {
                    // Optionally delete user if role assignment fails, or handle as partial success
                    // await _userManager.DeleteAsync(user);
                    return (false, user.Id, roleResult.Errors.Select(e => e.Description));
                }
            }
            return (true, user.Id, Enumerable.Empty<string>());
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return (false, new[] { "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "N/A",
                    Email = user.Email ?? "N/A",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeId = user.EmployeeId,
                    IsActive = user.IsActive,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "N/A",
                Email = user.Email ?? "N/A",
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                IsActive = user.IsActive,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<UpdateUserDto?> GetUserForUpdateAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return new UpdateUserDto
            {
                Id = user.Id,
                Email = user.Email ?? "N/A",
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                IsActive = user.IsActive,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }


        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "N/A",
                Email = user.Email ?? "N/A",
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                IsActive = user.IsActive,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                return (false, new[] { "User not found." });
            }

            user.Email = updateUserDto.Email;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.EmployeeId = updateUserDto.EmployeeId;
            user.IsActive = updateUserDto.IsActive;
            // UserName is typically not updated directly this way, or requires special handling

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(updateUserDto.Roles).ToList();
            var rolesToAdd = updateUserDto.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    return (false, removeResult.Errors.Select(e => e.Description));
                }
            }

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    return (false, addResult.Errors.Select(e => e.Description));
                }
            }

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> AddUserToRolesAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, new[] { "User not found." });

            var result = await _userManager.AddToRolesAsync(user, roles);
            return (result.Succeeded, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RemoveUserFromRolesAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, new[] { "User not found." });

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            return (result.Succeeded, result.Succeeded ? Enumerable.Empty<string>() : result.Errors.Select(e => e.Description));
        }
    }
}
