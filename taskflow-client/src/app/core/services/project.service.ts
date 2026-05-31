import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  ProjectDto,
  CreateProjectRequest,
  UpdateProjectRequest,
  AddMemberRequest,
  PromoteManagerRequest
} from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly base = `${environment.apiUrl}/projects`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<ProjectDto[]>(this.base);
  }

  getById(id: number) {
    return this.http.get<ProjectDto>(`${this.base}/${id}`);
  }

  create(request: CreateProjectRequest) {
    return this.http.post<number>(this.base, request);
  }

  update(id: number, request: UpdateProjectRequest) {
    return this.http.patch(`${this.base}/${id}`, request);
  }

  delete(id: number) {
    return this.http.delete(`${this.base}/${id}`);
  }

  addMember(request: AddMemberRequest) {
    return this.http.post(`${this.base}/members`, request);
  }

  promoteToManager(request: PromoteManagerRequest) {
    return this.http.put(`${environment.apiUrl}/identity/promote`, request);
  }
}
