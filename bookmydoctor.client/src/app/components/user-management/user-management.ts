import { Component } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-user-management',
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement {
  displayedColumns: string[] = ['name', 'email', 'role', 'actions'];
  users = [
    { id: 1, name: 'John Doe', email: 'john@example.com', role: 'Patient' },
    { id: 2, name: 'Jane Smith', email: 'jane@example.com', role: 'Doctor' }
  ];

  constructor(private dialog: MatDialog) {}

  openUserDialog(user?: any) {
    // Dialog implementation
  }

  editUser(user: any) {
    this.openUserDialog(user);
  }

  deleteUser(id: number) {
    this.users = this.users.filter(u => u.id !== id);
  }
}
