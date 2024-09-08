using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Models;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class TaskManagementDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<RawData> RawData { get; set; }

        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assignment>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RawData>(e => { e.HasNoKey().ToView(null); });
            base.OnModelCreating(modelBuilder);

        }
    }
}
