export interface OrganizationDto {
  id: number;
  name: string;
  description: string;
  ownerId: string;
  createdAt: string;
}

export interface CreateOrgRequest {
  name: string;
  description: string;
  adminEmail: string;
  adminPassword: string;
}

export interface InviteMemberRequest {
  organizationId: number;
  email: string;
}
