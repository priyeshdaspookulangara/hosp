using UniCareERP.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniCareERP.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<(bool Success, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto createUserDto);
        Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<(bool Success, IEnumerable<string> Errors)> DeleteUserAsync(string id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<(bool Success, IEnumerable<string> Errors)> AddUserToRolesAsync(string userId, IEnumerable<string> roles);
        Task<(bool Success, IEnumerable<string> Errors)> RemoveUserFromRolesAsync(string userId, IEnumerable<string> roles);
        Task<UpdateUserDto?> GetUserForUpdateAsync(string id);

    }
}
