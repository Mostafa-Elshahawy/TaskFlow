using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Infrastructure.Persistence;

internal class ApplicationDBContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    internal DbSet<TaskEntity> Tasks { get; set; }
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Organization> Organizations { get; set; }
    internal DbSet<OrganizationMember> OrganizationMembers { get; set; }
    internal DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Organization>()
       .HasOne(o => o.Owner)
       .WithMany(u => u.OwnedOrganizations)
       .HasForeignKey(o => o.OwnerId)
       .OnDelete(DeleteBehavior.Restrict);

        // OrganizationMember relationships
        modelBuilder.Entity<OrganizationMember>()
            .HasKey(m => new { m.OrganizationId, m.UserId });

        modelBuilder.Entity<OrganizationMember>()
            .HasOne(m => m.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(m => m.OrganizationId);

        modelBuilder.Entity<OrganizationMember>()
            .HasOne(m => m.User)
            .WithMany(u => u.Organizations)
            .HasForeignKey(m => m.UserId);

        // Project relationships
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.Projects)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedBy)
            .WithMany() 
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Managers)
            .WithMany(u => u.ManagedProjects)
            .UsingEntity(j => j.ToTable("ProjectManagers"));

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany(u => u.AssignedProjects)
            .UsingEntity(j => j.ToTable("ProjectMembers"));

        // Task relationships
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrganizationInvitation>()
           .HasOne(i => i.Organization)
           .WithMany(o => o.Invitations)
           .HasForeignKey(i => i.OrganizationId);

        // Configure soft delete query filters
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.isDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(p => !p.isDeleted);
        modelBuilder.Entity<TaskEntity>().HasQueryFilter(t => !t.isDeleted);
    }
}
