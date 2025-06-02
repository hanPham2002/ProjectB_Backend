﻿using Microsoft.AspNetCore.Mvc;
using Project_B.DTOs;
using Project_B.Interface;
using Project_B.Models;

namespace Project_B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleService;

        public RoleController(IRoleRepository roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var roleDTOs = roles.Select(r => new RoleDTO
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Status = r.Status
            });

            return Ok(roleDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            var roleDTO = new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status
            };

            return Ok(roleDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleDTO roleDTO)
        {
            var role = new Role
            {
                RoleName = roleDTO.RoleName,
                Status = roleDTO.Status,
                CreatedDate = DateTime.UtcNow
            };

            await _roleService.AddRoleAsync(role);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, roleDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO roleDTO)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            role.RoleName = roleDTO.RoleName;
            role.Status = roleDTO.Status;

            await _roleService.UpdateRoleAsync(role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            await _roleService.DeleteRoleAsync(id);
            return NoContent();
        }

        //List Manager
        [HttpGet("{roleId}/Users")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            var users = await _roleService.GetUsersByRoleAsync(roleId);
            return Ok(users.Select(u => new {
                u.UserId,
                u.Name,
                u.Email
            }));
        }

        [HttpPost("{roleId}/AssignUser")]
        public async Task<IActionResult> AssignUserToRole(int roleId, int userId)
        {
            var result = await _roleService.AssignUserToRoleAsync(roleId, userId);
            if (!result) return BadRequest("User already has this role.");
            return Ok("User assigned successfully.");
        }

        [HttpDelete("{roleId}/RemoveUser/{userId}")]
        public async Task<IActionResult> RemoveUserFromRole(int roleId, int userId)
        {
            var result = await _roleService.RemoveUserFromRoleAsync(roleId, userId);
            if (!result) return NotFound("User-role relation not found.");
            return Ok("User removed from role.");
        }

        [HttpGet("{roleId}/UnassignedUsers")]
        public async Task<IActionResult> GetUnassignedUsers(int roleId)
        {
            var users = await _roleService.GetUnassignedUsersAsync(roleId);

            return Ok(users.Select(u => new
            {
                u.UserId,
                u.Name,
                u.Email,
                u.Avatar
            }));
        }

    }
}
