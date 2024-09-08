using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Models;
using TaskManagement.Core.Repositories;
using TaskManagement.Models;

namespace TaskManagement.Core
{
    public interface IUnitOfWork
    {
        IBaseRepository<Assignment> Assignments { get; }
        IBaseRepository<Team> Teams { get; }
        IUserRepository Users { get; }
        IBaseRepository<Attachments> Attachments { get; }
        IBaseRepository<Comments> Comments { get; }
        IBaseRepository<AuditTrail> AuditTrails { get; }

        int Complete();

    }
}
