import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  TaskDto,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskFilter,
  AssignTaskRequest
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

  create(request: CreateTaskRequest) {
    return this.http.post<number>(this.base, request);
  }

  update(id: number, request: UpdateTaskRequest) {
    return this.http.patch(`${this.base}/${id}`, request);
  }

  delete(id: number) {
    return this.http.delete(`${this.base}/${id}`);
  }

  assign(request: AssignTaskRequest) {
    return this.http.post(`${this.base}/assign`, request);
  }
}
