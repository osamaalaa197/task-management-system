using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Consts;
using TaskManagement.Core.Repositories;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.EF.Repositories
{
    public class UserRepository:BaseRepository<ApplicationUser>,IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(TaskManagementDbContext context,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager) : base(context) 
        {
            _userManager=userManager;
            _roleManager= roleManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUserByRole(string role,bool includeTeamLeader)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var users = new List<ApplicationUser>();
            users.AddRange(usersInRole); // add users with the specified role
            if (includeTeamLeader)
            {
                var teamleader = await _userManager.GetUsersInRoleAsync(Roles.TeamLeader);
                users.AddRange(teamleader);
            }
            return usersInRole;
        }
        public async Task<List<ApplicationUser>> GetUserByTeamIdAndRole(int teamId, string role)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            return await _userManager.Users
                .Where(u => u.TeamId != null && (int)u.TeamId == teamId)
                .Where(u => usersInRole.Contains(u))
                .ToListAsync();
        }
        public async Task<bool> AssignRole(string userId, string newRoleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false; 
            var roleExists = await _roleManager.RoleExistsAsync(newRoleName);
            if (!roleExists)
                await _roleManager.CreateAsync(new IdentityRole(newRoleName));
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles != null && currentRoles.Count > 0)
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, newRoleName);
            return result.Succeeded;
        }

        public async Task<string> GetUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles == null || !currentRoles.Any())
                return "No role assigned";
            return currentRoles.FirstOrDefault();
        }

    }
}
