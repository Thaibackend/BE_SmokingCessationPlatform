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
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PermissionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsAsync()
        {
            var permissions = await _context.Permissions
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }

        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {id} not found");

            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionRequest request)
        {
            // Check if permission with the same name already exists
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == request.Name);

            if (existingPermission != null)
                throw new InvalidOperationException($"Permission with name '{request.Name}' already exists");

            var permission = new Permission
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<PermissionDto> UpdatePermissionAsync(int id, UpdatePermissionRequest request)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {id} not found");

            permission.Description = request.Description;

            await _context.SaveChangesAsync();

            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task DeletePermissionAsync(int id)
        {
            var permission = await _context.Permissions
                .Include(p => p.RolePermissions)
                .FirstOrDefaultAsync(p => p.PermissionId == id);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {id} not found");

            // Check if permission is in use
            if (permission.RolePermissions.Any())
                throw new InvalidOperationException("Cannot delete a permission that is assigned to roles");

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoleDto>> GetPermissionRolesAsync(int permissionId)
        {
            var permission = await _context.Permissions
                .Include(p => p.RolePermissions)
                    .ThenInclude(rp => rp.Role)
                .FirstOrDefaultAsync(p => p.PermissionId == permissionId);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {permissionId} not found");

            var roles = permission.RolePermissions
                .Select(rp => rp.Role)
                .ToList();

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
    }
} 