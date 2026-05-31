import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { OrgService } from '../../../core/services/org.service';
import { OrganizationDto } from '../../../core/models/organization.model';

@Component({
  selector: 'app-org-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './org-list.component.html'
})
export class OrgListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private orgService = inject(OrgService);
  private snack = inject(MatSnackBar);

  dataSource = new MatTableDataSource<OrganizationDto>([]);
  displayedColumns = ['name', 'description', 'createdAt'];
  showCreateForm = false;

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    adminEmail: ['', [Validators.required, Validators.email]],
    adminPassword: ['', [Validators.required, Validators.minLength(6)]]
  });

  inviteForm = this.fb.group({
    organizationId: [null as number | null, Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  acceptForm = this.fb.group({
    token: ['', Validators.required]
  });

  ngOnInit() {
    this.load();
  }

  load() {
    this.orgService.getAll().subscribe({
      next: (data) => (this.dataSource.data = data),
      error: () => this.snack.open('Failed to load organizations', 'Close', { duration: 3000 })
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.orgService.create(this.form.value as any).subscribe({
      next: (id) => {
        this.snack.open(`Organization created (ID: ${id})`, 'Close', { duration: 4000 });
        this.form.reset();
        this.showCreateForm = false;
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  sendInvite() {
    if (this.inviteForm.invalid) return;
    this.orgService.invite(this.inviteForm.value as any).subscribe({
      next: () => {
        this.snack.open('Invitation sent', 'Close', { duration: 3000 });
        this.inviteForm.reset();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  acceptInvite() {
    if (this.acceptForm.invalid) return;
    this.orgService.acceptInvite(this.acceptForm.value.token!).subscribe({
      next: () => {
        this.snack.open('Invitation accepted', 'Close', { duration: 3000 });
        this.acceptForm.reset();
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }
}
