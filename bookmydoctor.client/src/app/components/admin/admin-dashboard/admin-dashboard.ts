import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AdminService } from '../../../core/services/admin.service';
import { DashboardSummaryDto, UserResponseDto } from '../../../core/models/admin.models';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboardComponent implements OnInit {
  dashboardSummary: DashboardSummaryDto | null = null;
  pendingDoctors: UserResponseDto[] = [];
  isLoading = true;
  error: string | null = null;

  constructor(
    private router: Router, 
    private adminService: AdminService, 
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  async loadDashboardData() {
    this.isLoading = true;
    this.error = null;
    this.cdr.detectChanges();
    
    this.adminService.getDashboardSummary().pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (response) => {
        this.toastr.success('Dashboard data loaded successfully');
        this.dashboardSummary = response?.data || {
          totalPatients: 0,
          totalDoctors: 0,
          totalAppointments: 0,
          totalRevenue: 0,
          pendingDoctors: []
        };
        this.pendingDoctors = this.dashboardSummary.pendingDoctors || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load dashboard data. Please try again.');
        this.error = 'Failed to load dashboard data. Please try again.';
        this.dashboardSummary = {
          totalPatients: 0,
          totalDoctors: 0,
          totalAppointments: 0,
          totalRevenue: 0,
          pendingDoctors: []
        };
      }
    });
  }

  loadDashboardSummary() {
    this.adminService.getDashboardSummary().subscribe({
      next: (response) => {
        this.dashboardSummary = response.data || null;
        this.pendingDoctors = this.dashboardSummary?.pendingDoctors || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load dashboard summary');
        this.error = 'Failed to load dashboard summary';
      }
    });
  }

  loadPendingDoctors() {
    this.adminService.getPendingDoctors().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.pendingDoctors = response.data;
        }
      },
      error: (error) => {
        this.toastr.error('Failed to load pending doctors');
      }
    });
  }

  getDoctorInitials(name: string): string {
    if (!name) return 'D';
    const names = name.split(' ');
    if (names.length >= 2) {
      return (names[0][0] + names[1][0]).toUpperCase();
    }
    return name[0].toUpperCase();
  }

  approveDoctor(doctor: UserResponseDto) {
    this.adminService.approveRejectDoctor(doctor.userId, true).subscribe({
      next: (response: any) => {
        if (response.success && this.dashboardSummary) {
          this.dashboardSummary.pendingDoctors = this.dashboardSummary.pendingDoctors.filter(d => d.userId !== doctor.userId);
          this.loadDashboardSummary();
          this.toastr.success('Doctor approved successfully');
        }
      },
      error: (error) => {
        this.toastr.error('Failed to approve doctor');
      }
    });
  }

  viewDoctorDetails(doctor: UserResponseDto) {
    this.toastr.info(`Viewing details for Dr. ${doctor.userName}`);
  }

  viewUsers() {
    this.router.navigate(['/admin/patients']);
  }

  viewDoctors() {
    this.router.navigate(['/admin/doctors']);
  }

  viewClinics() {
    this.router.navigate(['/admin/clinics']);
  }

  viewInvoices() {
    this.router.navigate(['/admin/invoices']);
  }

  viewPayments() {
    this.router.navigate(['/admin/payments']);
  }

  viewAppointments() {
    this.router.navigate(['/admin/appointments']);
  }

  viewReports() {
    this.router.navigate(['/admin/reports']);
  }

  refreshDashboard() {
    this.loadDashboardData();
  }

  logout() {
    sessionStorage.removeItem('token');
    this.router.navigate(['/auth/login']);
  }
}