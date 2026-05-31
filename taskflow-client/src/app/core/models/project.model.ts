export interface ProjectDto {
  id: number;
  name: string;
  description: string;
  createdByUserId: string;
  createdByName: string;
  createdAt: string;
  updatedAt?: string;
  organizationId: number;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  organizationId: number;
}

export interface UpdateProjectRequest {
  id?: number;
  name: string;
  description: string;
  organizationId: number;
}

export interface AddMemberRequest {
  projectId: number;
  userId: string;
}

export interface PromoteManagerRequest {
  userId: string;
  projectId: number;
}
