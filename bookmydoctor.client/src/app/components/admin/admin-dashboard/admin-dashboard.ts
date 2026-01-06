import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AdminService } from '../../../core/services/admin.service';
import { DashboardSummaryDto, UserResponseDto } from '../../../core/models/admin.models';

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

  constructor(private router: Router, private adminService: AdminService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  async loadDashboardData() {
    try {
      this.isLoading = true;
      this.error = null;
      
      const summaryResponse = await this.adminService.getDashboardSummary().toPromise();
      
      this.dashboardSummary = summaryResponse?.data || {
        totalPatients: 0,
        totalDoctors: 0,
        totalAppointments: 0,
        totalRevenue: 0,
        pendingDoctors: []
      };
      
      this.pendingDoctors = this.dashboardSummary.pendingDoctors || [];
    } catch (error) {
      console.error('Error loading dashboard data:', error);
      this.error = 'Failed to load dashboard data. Please try again.';
      this.dashboardSummary = {
        totalPatients: 0,
        totalDoctors: 0,
        totalAppointments: 0,
        totalRevenue: 0,
        pendingDoctors: []
      };
    } finally {
      this.isLoading = false;
    }
  }

  loadDashboardSummary() {
    this.adminService.getDashboardSummary().subscribe({
      next: (response) => {
        this.dashboardSummary = response.data || null;
        this.pendingDoctors = this.dashboardSummary?.pendingDoctors || [];
      },
      error: (error) => {
        console.error('Error loading dashboard summary:', error);
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
        console.error('Error loading pending doctors:', error);
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
        }
      },
      error: (error) => {
        console.error('Error approving doctor:', error);
      }
    });
  }

  viewDoctorDetails(doctor: UserResponseDto) {
    // Navigate to doctor details page or open modal
    console.log('View doctor details:', doctor);
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