using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Role;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmokingQuitSupportAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found");

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request)
        {
            // Check if role with the same name already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.Name);

            if (existingRole != null)
                throw new InvalidOperationException($"Role with name '{request.Name}' already exists");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return await GetRoleByIdAsync(role.RoleId);
        }

        public async Task<RoleDto> UpdateRoleAsync(int id, UpdateRoleRequest request)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found");

            // Check if another role with the same name already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.Name && r.RoleId != id);

            if (existingRole != null)
                throw new InvalidOperationException($"Another role with name '{request.Name}' already exists");

            role.Name = request.Name;
            role.Description = request.Description;

            await _context.SaveChangesAsync();

            return await GetRoleByIdAsync(id);
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found");

            // Check if role is in use
            if (role.UserRoles.Any())
                throw new InvalidOperationException("Cannot delete a role that is assigned to users");

            // Remove role permissions
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // Remove role
            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();
        }

        public async Task<RoleDto> AssignPermissionsToRoleAsync(int roleId, AssignPermissionsRequest request)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found");

            // Verify all permissions exist
            var permissions = await _context.Permissions
                .Where(p => request.PermissionIds.Contains(p.PermissionId))
                .ToListAsync();

            if (permissions.Count != request.PermissionIds.Count())
                throw new KeyNotFoundException("One or more permissions not found");

            // Remove existing permissions
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // Add new permissions
            foreach (var permissionId in request.PermissionIds)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return await GetRoleByIdAsync(roleId);
        }

        public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found");

            var permissions = role.RolePermissions
                .Select(rp => rp.Permission)
                .ToList();

            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var roles = user.UserRoles
                .Select(ur => ur.Role)
                .ToList();

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task AssignRolesToUserAsync(int userId, IEnumerable<int> roleIds)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            // Verify all roles exist
            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.RoleId))
                .ToListAsync();

            if (roles.Count != roleIds.Count())
                throw new KeyNotFoundException("One or more roles not found");

            // Remove existing roles
            _context.UserRoles.RemoveRange(user.UserRoles);

            // Add new roles
            foreach (var roleId in roleIds)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
} 