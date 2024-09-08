using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Repositories
{
    public interface IUserRepository: IBaseRepository<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> GetUserByRole(string role, bool includeTeamLeader);
        Task<List<ApplicationUser>> GetUserByTeamIdAndRole(int teamId, string role);
        Task<bool> AssignRole(string userId, string RoleName);
        Task<string> GetUserRole(string userId);
    }
}
