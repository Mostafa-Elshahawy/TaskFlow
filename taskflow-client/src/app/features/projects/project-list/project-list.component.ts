import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProjectService } from '../../../core/services/project.service';
import { OrgService } from '../../../core/services/org.service';
import { ProjectDto } from '../../../core/models/project.model';
import { OrganizationDto } from '../../../core/models/organization.model';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './project-list.component.html'
})
export class ProjectListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private projectService = inject(ProjectService);
  private orgService = inject(OrgService);
  private snack = inject(MatSnackBar);

  dataSource = new MatTableDataSource<ProjectDto>([]);
  orgs: OrganizationDto[] = [];
  displayedColumns = ['name', 'description', 'organization', 'actions'];
  showForm = false;

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    organizationId: [null as number | null, Validators.required]
  });

  ngOnInit() {
    this.load();
    this.orgService.getAll().subscribe({ next: (data) => (this.orgs = data) });
  }

  load() {
    this.projectService.getAll().subscribe({
      next: (data) => (this.dataSource.data = data),
      error: () => this.snack.open('Failed to load projects', 'Close', { duration: 3000 })
    });
  }

  orgName(id: number) {
    return this.orgs.find(o => o.id === id)?.name ?? id;
  }

  submit() {
    if (this.form.invalid) return;
    this.projectService.create(this.form.value as any).subscribe({
      next: () => {
        this.snack.open('Project created', 'Close', { duration: 3000 });
        this.form.reset();
        this.showForm = false;
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  delete(id: number) {
    if (!confirm('Delete this project?')) return;
    this.projectService.delete(id).subscribe({
      next: () => { this.snack.open('Project deleted', 'Close', { duration: 3000 }); this.load(); },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }
}
