import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OrgService } from '../../../core/services/org.service';

@Component({
  selector: 'app-accept-invite',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatCardModule, MatProgressSpinnerModule],
  templateUrl: './accept-invite.component.html'
})
export class AcceptInviteComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly orgService = inject(OrgService);

  loading = true;
  accepted = false;
  errorMessage = '';

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.loading = false;
      this.errorMessage = 'This invitation link is incomplete.';
      return;
    }

    this.orgService.acceptInvite(token).subscribe({
      next: () => {
        this.loading = false;
        this.accepted = true;
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error?.error?.detail || 'This invitation is invalid or has expired.';
      }
    });
  }
}
