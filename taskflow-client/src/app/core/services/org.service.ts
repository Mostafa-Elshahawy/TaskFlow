import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CreateOrgRequest, InviteMemberRequest, OrganizationDto } from '../models/organization.model';

@Injectable({ providedIn: 'root' })
export class OrgService {
  private readonly base = `${environment.apiUrl}/organization`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<OrganizationDto[]>(this.base);
  }

  create(request: CreateOrgRequest) {
    return this.http.post<number>(this.base, request);
  }

  invite(request: InviteMemberRequest) {
    return this.http.post(`${this.base}/send-invite`, request);
  }

  acceptInvite(token: string) {
    return this.http.get(`${this.base}/accept-invite`, { params: { token } });
  }
}
