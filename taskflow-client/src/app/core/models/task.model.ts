export enum TaskStatus {
  Open = 0,
  InProgress = 1,
  Completed = 2
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2
}

export interface TaskDto {
  id: number;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  projectId: number;
  projectName: string;
  createdByUserId: string;
  createdByName: string;
  assigneeId?: string;
  assigneeName?: string;
  dueDate: string;
  completedAt?: string;
  createdAt: string;
  updatedAt?: string;
  rowVersion: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  projectId: number;
  assigneeId?: string;
  dueDate: string;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string;
  rowVersion: string;
}

export interface TaskFilter {
  projectId?: number;
  status?: TaskStatus;
  priority?: TaskPriority;
  assigneeId?: string;
  dueBefore?: string;
  dueAfter?: string;
}

export interface AssignTaskRequest {
  taskId: number;
  assigneeId: string;
  rowVersion: string;
}

export interface TaskStatusHistoryDto {
  id: number;
  fromStatus: TaskStatus;
  toStatus: TaskStatus;
  changedByUserId: string;
  changedByName?: string;
  changedAtUtc: string;
}
