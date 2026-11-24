using TaskFlow.Application.Orgs.Dtos;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Application.Projects.Dtos;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Entites;
using Xunit;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Tests.Application;

public class ExplicitMappingTests
{
    [Fact]
    public void TaskCreationMapping_UsesCommandFieldsAndServerOwnedValues()
    {
        var createdAt = new DateTime(2026, 11, 24, 8, 30, 0, DateTimeKind.Utc);
        var dueDate = createdAt.AddDays(5);
        var command = new CreateTaskCommand
        {
            Title = "Prepare release",
            Description = "Create the release checklist",
            Status = TaskStatus.InProgress,
            Priority = TaskPriority.High,
            ProjectId = 42,
            AssigneeId = "assignee-1",
            DueDate = dueDate
        };

        var task = command.ToEntity("creator-1", createdAt);

        Assert.Equal("Prepare release", task.Title);
        Assert.Equal("Create the release checklist", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(42, task.ProjectId);
        Assert.Equal("assignee-1", task.AssigneeId);
        Assert.Equal("creator-1", task.CreatedByUserId);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(createdAt, task.CreatedAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.UpdatedAt);
        Assert.False(task.isDeleted);
    }

    [Fact]
    public void TaskDtoMapping_MapsNavigationNames()
    {
        var createdAt = new DateTime(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);
        var updatedAt = createdAt.AddHours(2);
        var completedAt = createdAt.AddHours(3);
        var task = new TaskEntity
        {
            Id = 7,
            Title = "Review pull request",
            Description = "Review access-control changes",
            Status = TaskStatus.Completed,
            Priority = TaskPriority.Medium,
            ProjectId = 9,
            Project = new Project { Name = "TaskFlow" },
            CreatedByUserId = "creator-2",
            CreatedBy = new ApplicationUser { UserName = "creator@example.com" },
            AssigneeId = "assignee-2",
            Assignee = new ApplicationUser { UserName = "assignee@example.com" },
            DueDate = createdAt.AddDays(1),
            CompletedAt = completedAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var dto = task.ToDto();

        Assert.Equal(7, dto.Id);
        Assert.Equal("Review pull request", dto.Title);
        Assert.Equal("Review access-control changes", dto.Description);
        Assert.Equal(TaskStatus.Completed, dto.Status);
        Assert.Equal(TaskPriority.Medium, dto.Priority);
        Assert.Equal(9, dto.ProjectId);
        Assert.Equal("TaskFlow", dto.ProjectName);
        Assert.Equal("creator-2", dto.CreatedByUserId);
        Assert.Equal("creator@example.com", dto.CreatedByName);
        Assert.Equal("assignee-2", dto.AssigneeId);
        Assert.Equal("assignee@example.com", dto.AssigneeName);
        Assert.Equal(task.DueDate, dto.DueDate);
        Assert.Equal(completedAt, dto.CompletedAt);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(updatedAt, dto.UpdatedAt);
    }

    [Fact]
    public void DtoMappings_PreserveNullNavigationNames()
    {
        var task = new TaskEntity
        {
            Title = "Unloaded task",
            Description = "Navigation properties are not loaded",
            CreatedByUserId = "creator-5"
        };
        var project = new Project
        {
            Name = "Unloaded project",
            Description = "Creator navigation is not loaded",
            CreatedByUserId = "creator-6"
        };

        var taskDto = task.ToDto();
        var projectDto = project.ToDto();

        Assert.Null(taskDto.ProjectName);
        Assert.Null(taskDto.CreatedByName);
        Assert.Null(taskDto.AssigneeName);
        Assert.Null(projectDto.CreatedByName);
    }

    [Fact]
    public void ProjectCreationMapping_IgnoresClientOwnedAuditAndDeletionValues()
    {
        var clientTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var serverTime = new DateTime(2026, 11, 24, 9, 0, 0, DateTimeKind.Utc);
        var command = new CreateProjectCommand
        {
            Name = "Platform",
            Description = "Platform improvements",
            OrganizationId = 15,
            CreatedAt = clientTime,
            UpdatedAt = clientTime,
            IsDeleted = true
        };

        var project = command.ToEntity("creator-3", serverTime);

        Assert.Equal("Platform", project.Name);
        Assert.Equal("Platform improvements", project.Description);
        Assert.Equal(15, project.OrganizationId);
        Assert.Equal("creator-3", project.CreatedByUserId);
        Assert.Equal(serverTime, project.CreatedAt);
        Assert.Null(project.UpdatedAt);
        Assert.False(project.isDeleted);
    }

    [Fact]
    public void ProjectDtoMapping_MapsEveryPublishedField()
    {
        var createdAt = new DateTime(2026, 11, 21, 12, 0, 0, DateTimeKind.Utc);
        var updatedAt = createdAt.AddDays(1);
        var project = new Project
        {
            Id = 12,
            Name = "API",
            Description = "API hardening",
            CreatedByUserId = "creator-4",
            CreatedBy = new ApplicationUser { UserName = "owner@example.com" },
            OrganizationId = 3,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            isDeleted = true
        };

        var dto = project.ToDto();

        Assert.Equal(12, dto.Id);
        Assert.Equal("API", dto.Name);
        Assert.Equal("API hardening", dto.Description);
        Assert.Equal("creator-4", dto.CreatedByUserId);
        Assert.Equal("owner@example.com", dto.CreatedByName);
        Assert.Equal(3, dto.OrganizationId);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(updatedAt, dto.UpdatedAt);
        Assert.True(dto.IsDeleted);
    }

    [Fact]
    public void OrganizationDtoMapping_MapsEveryPublishedField()
    {
        var createdAt = new DateTime(2026, 11, 22, 14, 0, 0, DateTimeKind.Utc);
        var organization = new Organization
        {
            Id = 5,
            Name = "Engineering",
            Description = "Engineering organization",
            OwnerId = "owner-1",
            CreatedAt = createdAt
        };

        var dto = organization.ToDto();

        Assert.Equal(5, dto.Id);
        Assert.Equal("Engineering", dto.Name);
        Assert.Equal("Engineering organization", dto.Description);
        Assert.Equal("owner-1", dto.OwnerId);
        Assert.Equal(createdAt, dto.CreatedAt);
    }
}
