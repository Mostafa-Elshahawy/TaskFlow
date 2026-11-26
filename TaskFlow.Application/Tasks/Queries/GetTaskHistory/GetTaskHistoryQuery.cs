using MediatR;
using TaskFlow.Application.Tasks.Dtos;

namespace TaskFlow.Application.Tasks.Queries.GetTaskHistory;

public record GetTaskHistoryQuery(int TaskId) : IRequest<IReadOnlyList<TaskStatusHistoryDto>>;
