using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Tasks.Dtos;

public static class TaskMappings
{
    public static TaskEntity ToEntity(
        this CreateTaskCommand command,
        string createdByUserId,
        DateTime createdAtUtc)
    {
        return new TaskEntity
        {
            Title = command.Title,
            Description = command.Description,
            Status = command.Status,
            Priority = command.Priority,
            ProjectId = command.ProjectId,
            AssigneeId = command.AssigneeId,
            CreatedByUserId = createdByUserId,
            DueDate = command.DueDate,
            CreatedAt = createdAtUtc,
            isDeleted = false
        };
    }

    public static TaskDto ToDto(this TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name!,
            CreatedByUserId = task.CreatedByUserId,
            CreatedByName = task.CreatedBy?.UserName!,
            AssigneeId = task.AssigneeId,
            AssigneeName = task.Assignee?.UserName,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
