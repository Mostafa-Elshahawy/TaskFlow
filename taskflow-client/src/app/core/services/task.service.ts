import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  TaskDto,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskFilter,
  AssignTaskRequest,
  TaskStatusHistoryDto
} from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly base = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getAll(filter: TaskFilter = {}) {
    let params = new HttpParams();
    if (filter.projectId != null) params = params.set('projectId', filter.projectId);
    if (filter.status != null) params = params.set('status', filter.status);
    if (filter.priority != null) params = params.set('priority', filter.priority);
    if (filter.assigneeId) params = params.set('assigneeId', filter.assigneeId);
    if (filter.dueBefore) params = params.set('dueBefore', filter.dueBefore);
    if (filter.dueAfter) params = params.set('dueAfter', filter.dueAfter);
    return this.http.get<TaskDto[]>(this.base, { params });
  }

  getById(id: number) {
    return this.http.get<TaskDto>(`${this.base}/${id}`);
  }

  getHistory(id: number) {
    return this.http.get<TaskStatusHistoryDto[]>(`${this.base}/${id}/history`);
  }

  create(request: CreateTaskRequest) {
    return this.http.post<number>(this.base, request);
  }

  update(id: number, request: UpdateTaskRequest) {
    return this.http.patch(`${this.base}/${id}`, request);
  }

  delete(id: number, rowVersion: string) {
    const params = new HttpParams().set('rowVersion', rowVersion);
    return this.http.delete(`${this.base}/${id}`, { params });
  }

  assign(request: AssignTaskRequest) {
    return this.http.post(`${this.base}/assign`, request);
  }
}
