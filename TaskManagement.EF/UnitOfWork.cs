using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core;
using TaskManagement.Core.Models;
using TaskManagement.Core.Repositories;
using TaskManagement.Data;
using TaskManagement.EF.Repositories;
using TaskManagement.Models;

namespace TaskManagement.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagementDbContext _context;
        public IBaseRepository<Assignment> Assignments { get; private set; }
        public IBaseRepository<Team> Teams { get; private set; }
        public IUserRepository Users { get; private set; }
        public IBaseRepository<Comments> Comments { get; private set; }
        public IBaseRepository<Attachments> Attachments { get; private set; }
        public IUserRepository User { get; private set; }

        public IBaseRepository<AuditTrail> AuditTrails { get; private set; }
        public UnitOfWork(TaskManagementDbContext dbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _context= dbContext;
            Assignments = new BaseRepository<Assignment>(_context);  
            Teams=new BaseRepository<Team>(_context);
            Users=new UserRepository(_context,userManager,roleManager);
            Comments=new BaseRepository<Comments>(_context);
            Attachments=new BaseRepository<Attachments>(_context);
            AuditTrails=new BaseRepository<AuditTrail>(_context);
        }

        public int Complete() => _context.SaveChanges();
    }
}
