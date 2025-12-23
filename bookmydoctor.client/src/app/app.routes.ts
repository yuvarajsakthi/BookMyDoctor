import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { ForgotPasswordComponent } from './components/auth/forgot-password/forgot-password';
import { AdminDashboardComponent } from './components/dashboard/admin-dashboard/admin-dashboard';
import { DoctorDashboardComponent } from './components/dashboard/doctor-dashboard/doctor-dashboard';
import { PatientDashboardComponent } from './components/dashboard/patient-dashboard/patient-dashboard';
import { PatientManagementComponent } from './components/admin/patient-management/patient-management';
import { DoctorManagementComponent } from './components/admin/doctor-management/doctor-management';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { DoctorGuard } from './core/guards/doctor.guard';
import { PatientGuard } from './core/guards/patient.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent }
    ]
  },
  {
    path: 'dashboard',
    canActivate: [AuthGuard],
    children: [
      { path: 'admin', component: AdminDashboardComponent, canActivate: [AdminGuard] },
      { path: 'doctor', component: DoctorDashboardComponent, canActivate: [DoctorGuard] },
      { path: 'patient', component: PatientDashboardComponent, canActivate: [PatientGuard] }
    ]
  },
  {
    path: 'admin',
    canActivate: [AuthGuard, AdminGuard],
    children: [
      { path: 'patients', component: PatientManagementComponent },
      { path: 'doctors', component: DoctorManagementComponent }
    ]
  },
  { path: '**', redirectTo: '/auth/login' }
];