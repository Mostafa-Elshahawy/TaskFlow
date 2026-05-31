import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProjectService } from '../../../core/services/project.service';
import { ProjectDto } from '../../../core/models/project.model';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule
  ],
  templateUrl: './project-detail.component.html'
})
export class ProjectDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private projectService = inject(ProjectService);
  private snack = inject(MatSnackBar);

  project?: ProjectDto;
  projectId!: number;

  addMemberForm = this.fb.group({ userId: ['', Validators.required] });
  promoteForm = this.fb.group({ userId: ['', Validators.required] });

  ngOnInit() {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    this.projectService.getById(this.projectId).subscribe({
      next: (p) => (this.project = p),
      error: () => this.snack.open('Project not found', 'Close', { duration: 3000 })
    });
  }

  addMember() {
    if (this.addMemberForm.invalid) return;
    this.projectService
      .addMember({ projectId: this.projectId, userId: this.addMemberForm.value.userId! })
      .subscribe({
        next: () => {
          this.snack.open('Member added', 'Close', { duration: 3000 });
          this.addMemberForm.reset();
        },
        error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
      });
  }

  promote() {
    if (this.promoteForm.invalid) return;
    this.projectService
      .promoteToManager({ userId: this.promoteForm.value.userId!, projectId: this.projectId })
      .subscribe({
        next: () => {
          this.snack.open('User promoted to manager', 'Close', { duration: 3000 });
          this.promoteForm.reset();
        },
        error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
      });
  }
}
