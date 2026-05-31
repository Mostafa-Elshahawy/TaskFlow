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
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { TaskService } from '../../../core/services/task.service';
import { ProjectService } from '../../../core/services/project.service';
import { UserService } from '../../../core/services/user.service';
import { TaskDto, TaskFilter, TaskStatus, TaskPriority } from '../../../core/models/task.model';
import { ProjectDto } from '../../../core/models/project.model';
import { UserDto } from '../../../core/models/user.model';

@Component({
  selector: 'app-task-list',
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
    MatChipsModule,
    MatSnackBarModule,
    MatExpansionModule
  ],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);
  private projectService = inject(ProjectService);
  private userService = inject(UserService);
  private snack = inject(MatSnackBar);

  dataSource = new MatTableDataSource<TaskDto>([]);
  projects: ProjectDto[] = [];
  users: UserDto[] = [];
  displayedColumns = ['title', 'project', 'status', 'priority', 'assignee', 'dueDate', 'actions'];
  showCreateForm = false;
  editingTask?: TaskDto;

  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;

  statusOptions = [
    { label: 'Open', value: TaskStatus.Open },
    { label: 'In Progress', value: TaskStatus.InProgress },
    { label: 'Completed', value: TaskStatus.Completed }
  ];

  priorityOptions = [
    { label: 'Low', value: TaskPriority.Low },
    { label: 'Medium', value: TaskPriority.Medium },
    { label: 'High', value: TaskPriority.High }
  ];

  filterForm = this.fb.group({
    projectId: [null as number | null],
    status: [null],
    priority: [null],
    assigneeId: [null as string | null]
  });

  createForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    projectId: [null as number | null, Validators.required],
    status: [TaskStatus.Open],
    priority: [TaskPriority.Low],
    assigneeId: [null as string | null],
    dueDate: ['', Validators.required]
  });

  editForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    status: [''],
    priority: [''],
    assigneeId: [null as string | null],
    assigneeName: [''],
    dueDate: ['', Validators.required]
  });

  assignForm = this.fb.group({
    taskId: [null as number | null, Validators.required],
    assigneeId: [null as string | null, Validators.required],
    assignedById: [null as string | null, Validators.required]
  });

  ngOnInit() {
    this.load();
    this.projectService.getAll().subscribe({ next: (data) => (this.projects = data) });
    this.userService.getAll().subscribe({ next: (data) => (this.users = data) });
  }

  load() {
    const f = this.filterForm.value;
    const filter: TaskFilter = {};
    if (f.projectId != null) filter.projectId = f.projectId;
    if (f.status != null) filter.status = f.status as TaskStatus;
    if (f.priority != null) filter.priority = f.priority as TaskPriority;
    if (f.assigneeId) filter.assigneeId = f.assigneeId;

    this.taskService.getAll(filter).subscribe({
      next: (data) => (this.dataSource.data = data),
      error: () => this.snack.open('Failed to load tasks', 'Close', { duration: 3000 })
    });
  }

  create() {
    if (this.createForm.invalid) return;
    const v = this.createForm.value;
    this.taskService.create({
      title: v.title!,
      description: v.description!,
      projectId: v.projectId!,
      status: v.status ?? TaskStatus.Open,
      priority: v.priority ?? TaskPriority.Low,
      assigneeId: v.assigneeId ?? undefined,
      dueDate: v.dueDate!
    }).subscribe({
      next: () => {
        this.snack.open('Task created', 'Close', { duration: 3000 });
        this.createForm.reset({ status: TaskStatus.Open, priority: TaskPriority.Low });
        this.showCreateForm = false;
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  startEdit(task: TaskDto) {
    this.editingTask = task;
    this.editForm.patchValue({
      title: task.title,
      description: task.description,
      status: TaskStatus[task.status],
      priority: TaskPriority[task.priority],
      assigneeId: task.assigneeId ?? null,
      assigneeName: task.assigneeName ?? '',
      dueDate: task.dueDate.substring(0, 10)
    });
  }

  saveEdit() {
    if (!this.editingTask || this.editForm.invalid) return;
    this.taskService.update(this.editingTask.id, this.editForm.value as any).subscribe({
      next: () => {
        this.snack.open('Task updated', 'Close', { duration: 3000 });
        this.editingTask = undefined;
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  delete(id: number) {
    if (!confirm('Delete this task?')) return;
    this.taskService.delete(id).subscribe({
      next: () => { this.snack.open('Task deleted', 'Close', { duration: 3000 }); this.load(); },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  assign() {
    if (this.assignForm.invalid) return;
    this.taskService.assign(this.assignForm.value as any).subscribe({
      next: () => {
        this.snack.open('Task assigned', 'Close', { duration: 3000 });
        this.assignForm.reset();
        this.load();
      },
      error: (err) => this.snack.open(err?.error?.detail || 'Failed', 'Close', { duration: 3000 })
    });
  }

  statusLabel(s: TaskStatus) { return TaskStatus[s]; }
  priorityLabel(p: TaskPriority) { return TaskPriority[p]; }
  priorityColor(p: TaskPriority) {
    if (p === TaskPriority.High) return 'warn';
    if (p === TaskPriority.Medium) return 'accent';
    return 'primary';
  }
}
