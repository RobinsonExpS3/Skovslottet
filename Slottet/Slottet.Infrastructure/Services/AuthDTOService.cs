using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class AuthDTOService : IAuthDTOService
    {
        private static readonly string[] AllowedRoles = ["Admin", "Employee", "Storskaerm"];

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthDTOService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)   
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> DeleteStaffUserAsync(Guid id)
        {
            var user = await FindByUserIdOrStaffIdAsync(id);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<CreateUserForStaffDTO?> GetStaffUserAsync(Guid userId)
        {
            var user = await FindByUserIdOrStaffIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return await MapToDtoAsync(user);
        }

        public async Task<CreateUserForStaffDTO?> PostStaffUserAsync(CreateUserForStaffDTO dto)
        {
            var authRole = NormalizeRole(dto.AuthRole);

            if (dto.StaffID == Guid.Empty ||
                string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                authRole == null)
            {
                return null;
            }

            if (!await EnsureRoleExistsAsync(authRole))
            {
                return null;
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName.Trim(),
                StaffID = dto.StaffID
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                return null;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, authRole);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return null;
            }

            return await MapToDtoAsync(user);
        }

        public async Task<bool> PutStaffUserAsync(Guid id, CreateUserForStaffDTO dto)
        {
            var user = await FindByUserIdOrStaffIdAsync(id);

            if (user == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                user.UserName = dto.UserName.Trim();
            }

            if (dto.StaffID != Guid.Empty)
            {
                user.StaffID = dto.StaffID;
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                if (!passwordResult.Succeeded)
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.AuthRole))
            {
                var authRole = NormalizeRole(dto.AuthRole);

                if (authRole == null || !await EnsureRoleExistsAsync(authRole))
                {
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return false;
                }

                var addResult = await _userManager.AddToRoleAsync(user, authRole);

                if (!addResult.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<ApplicationUser?> FindByUserIdOrStaffIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            return user ?? await _userManager.Users.FirstOrDefaultAsync(u => u.StaffID == id);
        }

        private async Task<CreateUserForStaffDTO> MapToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new CreateUserForStaffDTO
            {
                StaffID = user.StaffID ?? Guid.Empty,
                UserName = user.UserName,
                Password = string.Empty,
                AuthRole = roles.FirstOrDefault() ?? "Employee"
            };
        }

        private async Task<bool> EnsureRoleExistsAsync(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                return true;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            return result.Succeeded;
        }

        private static string? NormalizeRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return "Employee";
            }

            return AllowedRoles.FirstOrDefault(allowedRole =>
                string.Equals(allowedRole, role.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
