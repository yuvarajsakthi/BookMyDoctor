import { Component, OnInit } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MaterialModule } from '../../../shared/material.module';
import { AdminService } from '../../../core/services/admin.service';
import { UserResponseDto } from '../../../core/models/admin.models';
import { BloodGroupUtil } from '../../../core/utils/blood-group.util';

@Component({
  selector: 'app-user-management',
  imports: [
    MaterialModule
],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement implements OnInit {
  doctors: UserResponseDto[] = [];
  patients: UserResponseDto[] = [];
  isLoading = false;
  selectedTab = 0;
  pageTitle = 'User Management';

  doctorColumns = ['userName', 'email', 'phone', 'specialty', 'experienceYears', 'consultationFee', 'bio', 'status', 'actions'];
  patientColumns = ['userName', 'email', 'phone', 'bloodGroup', 'emergencyContact', 'dateOfBirth', 'status', 'actions'];

  constructor(
    private adminService: AdminService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    // Set tab and title based on route
    const url = this.router.url;
    if (url.includes('/doctors')) {
      this.selectedTab = 0; // Doctors tab
      this.pageTitle = 'Doctor Management';
    } else if (url.includes('/patients')) {
      this.selectedTab = 1; // Patients tab
      this.pageTitle = 'Patient Management';
    }
    
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    const url = this.router.url;
    
    if (url.includes('/doctors')) {
      // Load only doctors
      this.adminService.getAllDoctors().subscribe({
        next: (doctorResponse) => {
          this.isLoading = false;
          if (doctorResponse?.success && doctorResponse.data) {
            this.doctors = doctorResponse.data;
          }
        },
        error: () => {
          this.isLoading = false;
          this.toastr.error('Failed to load doctors');
        }
      });
    } else if (url.includes('/patients')) {
      // Load only patients
      this.adminService.getAllPatients().subscribe({
        next: (patientResponse) => {
          this.isLoading = false;
          if (patientResponse?.success && patientResponse.data) {
            this.patients = patientResponse.data;
          }
        },
        error: () => {
          this.isLoading = false;
          this.toastr.error('Failed to load patients');
        }
      });
    } else {
      // Load both (fallback)
      Promise.all([
        this.adminService.getAllDoctors().toPromise(),
        this.adminService.getAllPatients().toPromise()
      ]).then(([doctorResponse, patientResponse]) => {
        this.isLoading = false;
        if (doctorResponse?.success && doctorResponse.data) {
          this.doctors = doctorResponse.data;
        }
        if (patientResponse?.success && patientResponse.data) {
          this.patients = patientResponse.data;
        }
      }).catch(() => {
        this.isLoading = false;
        this.toastr.error('Failed to load users');
      });
    }
  }

  addUser() {
    const userType = this.selectedTab === 0 ? 'doctor' : 'patient';
    const currentRoute = this.router.url;
    let redirectRoute = '/admin/doctors';
    
    if (currentRoute.includes('/patients')) {
      redirectRoute = '/admin/patients';
    }
    
    this.router.navigate(['/admin/user-form'], { 
      queryParams: { type: userType, returnUrl: redirectRoute } 
    });
  }

  editUser(user: UserResponseDto) {
    const userType = (user.userRole === 1) ? 'doctor' : 'patient';
    const currentRoute = this.router.url;
    let redirectRoute = '/admin/doctors';
    
    if (currentRoute.includes('/patients')) {
      redirectRoute = '/admin/patients';
    }
    
    this.router.navigate(['/admin/user-form'], { 
      queryParams: { type: userType, id: user.userId, returnUrl: redirectRoute } 
    });
  }

  toggleUserStatus(user: UserResponseDto) {
    this.adminService.updateUserStatus(user.userId, !user.isActive).subscribe({
      next: (response) => {
        if (response.success) {
          user.isActive = !user.isActive;
          const userType = this.router.url.includes('/doctors') ? 'Doctor' : 'Patient';
          this.toastr.success(`${userType} ${user.isActive ? 'activated' : 'deactivated'} successfully`);
        } else {
          this.toastr.error(response.message);
        }
      },
      error: () => {
        this.toastr.error('Failed to update user status');
      }
    });
  }

  approveDoctor(doctor: UserResponseDto) {
    this.adminService.approveRejectDoctor(doctor.userId, true).subscribe({
      next: (response) => {
        if (response.success) {
          doctor.isApproved = true;
          this.toastr.success('Doctor approved successfully');
        } else {
          this.toastr.error(response.message);
        }
      },
      error: () => {
        this.toastr.error('Failed to approve doctor');
      }
    });
  }

  deleteUser(user: UserResponseDto) {
    const userType = user.userRole === 1 ? 'doctor' : 'patient';
    if (confirm(`Are you sure you want to delete this ${userType}?`)) {
      this.adminService.deleteUser(user.userId).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadUsers();
            this.toastr.success(`${userType} deleted successfully`);
          } else {
            this.toastr.error(response.message);
          }
        },
        error: () => {
          this.toastr.error(`Failed to delete ${userType}`);
        }
      });
    }
  }

  getUserInitials(userName: string): string {
    if (!userName) return 'U';
    const names = userName.split(' ');
    if (names.length >= 2) {
      return (names[0][0] + names[1][0]).toUpperCase();
    }
    return userName.substring(0, 2).toUpperCase();
  }

  getGenderDisplay(gender: any): string {
    if (gender === null || gender === undefined) return 'N/A';
    const genderMap: { [key: number]: string } = {
      0: 'Male', 1: 'Female', 2: 'Other'
    };
    return genderMap[gender] || 'N/A';
  }

  getBloodGroupDisplay(bloodGroup: any): string {
    return BloodGroupUtil.getDisplayValue(bloodGroup);
  }

  getStatusClass(user: UserResponseDto): string {
    if (!user.isActive) return 'inactive';
    if (user.userRole === 1 && !user.isApproved) return 'pending';
    return 'active';
  }

  getStatusText(user: UserResponseDto): string {
    if (!user.isActive) return 'Inactive';
    if (user.userRole === 1 && !user.isApproved) return 'Pending';
    return 'Active';
  }

  goBack() {
    this.router.navigate(['/dashboard/admin']);
  }

  getDoctorStats() {
    return {
      total: this.doctors.length,
      active: this.doctors.filter(d => d.isActive && d.isApproved).length,
      pending: this.doctors.filter(d => !d.isApproved).length,
      inactive: this.doctors.filter(d => !d.isActive).length
    };
  }

  getPatientStats() {
    return {
      total: this.patients.length,
      active: this.patients.filter(p => p.isActive).length,
      inactive: this.patients.filter(p => !p.isActive).length
    };
  }

  isDoctorOnlyPage(): boolean {
    return this.router.url.includes('/doctors');
  }

  formatExperience(years: number | null | undefined): string {
    if (!years && years !== 0) return 'N/A';
    return years === 1 ? '1 year' : `${years} years`;
  }

  formatFee(fee: number | null | undefined): string {
    if (!fee && fee !== 0) return 'N/A';
    return `₹${fee.toLocaleString()}`;
  }

  truncateBio(bio: string | null | undefined): string {
    if (!bio) return 'N/A';
    return bio.length > 30 ? bio.substring(0, 30) + '...' : bio;
  }

  formatDate(dateString: string | null | undefined): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  isPatientOnlyPage(): boolean {
    return this.router.url.includes('/patients');
  }

  exportData() {
    const currentData = this.isDoctorOnlyPage() || (!this.isPatientOnlyPage() && this.selectedTab === 0) ? this.doctors : this.patients;
    const dataType = this.isDoctorOnlyPage() || (!this.isPatientOnlyPage() && this.selectedTab === 0) ? 'doctors' : 'patients';
    
    if (currentData.length === 0) {
      this.toastr.warning(`No ${dataType} data to export`);
      return;
    }

    const csvData = this.convertToCSV(currentData, dataType);
    const blob = new Blob([csvData], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${dataType}_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
    this.toastr.success(`${dataType} data exported successfully`);
  }

  private convertToCSV(data: UserResponseDto[], type: string): string {
    if (type === 'doctors') {
      const headers = ['Name', 'Email', 'Phone', 'Specialty', 'Experience', 'Fee', 'Bio', 'Status'];
      const rows = data.map(doctor => [
        doctor.userName,
        doctor.email,
        doctor.phone || 'N/A',
        doctor.specialty || 'N/A',
        this.formatExperience(doctor.experienceYears),
        this.formatFee(doctor.consultationFee),
        doctor.bio || 'N/A',
        this.getStatusText(doctor)
      ]);
      return [headers, ...rows].map(row => row.map(cell => `"${cell}"`).join(',')).join('\n');
    } else {
      const headers = ['Name', 'Email', 'Phone', 'Blood Group', 'Emergency Contact', 'Date of Birth', 'Status'];
      const rows = data.map(patient => [
        patient.userName,
        patient.email,
        patient.phone || 'N/A',
        this.getBloodGroupDisplay(patient.bloodGroup),
        patient.emergencyContact || 'N/A',
        this.formatDate(patient.dateOfBirth),
        this.getStatusText(patient)
      ]);
      return [headers, ...rows].map(row => row.map(cell => `"${cell}"`).join(',')).join('\n');
    }
  }
}