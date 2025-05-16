using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using Task = TaskFlow.Domain.Entites.Task;

namespace TaskFlow.Infrastructure.Persistence;

internal class ApplicationDBContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    internal DbSet<Task> Tasks { get; set; }
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Organization>()
       .HasOne(o => o.Owner)
       .WithMany(u => u.OwnedOrganizations)
       .HasForeignKey(o => o.OwnerId)
       .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithMany(u => u.MemberOrganizations)
            .UsingEntity(j => j.ToTable("OrganizationMembers"));

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
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure soft delete query filters
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.isDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(p => !p.isDeleted);
        modelBuilder.Entity<Task>().HasQueryFilter(t => !t.isDeleted);
    }
}
