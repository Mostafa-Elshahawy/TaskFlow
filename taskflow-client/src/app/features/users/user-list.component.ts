import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { UserService } from '../../core/services/user.service';
import { UserDto } from '../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatTableModule, MatSnackBarModule],
  templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private snack = inject(MatSnackBar);

  dataSource = new MatTableDataSource<UserDto>([]);
  displayedColumns = ['email', 'userName', 'id'];

  ngOnInit() {
    this.userService.getAll().subscribe({
      next: (data) => (this.dataSource.data = data),
      error: () => this.snack.open('Failed to load users', 'Close', { duration: 3000 })
    });
  }
}
