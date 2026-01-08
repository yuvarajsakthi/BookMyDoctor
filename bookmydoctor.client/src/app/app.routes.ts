import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { ForgotPasswordComponent } from './components/auth/forgot-password/forgot-password';
import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard';
import { DoctorDashboardComponent } from './components/doctor/doctor-dashboard/doctor-dashboard';
import { PatientDashboardComponent } from './components/patient/patient-dashboard/patient-dashboard';
import { ClinicFormComponent } from './components/admin/clinic-form/clinic-form';
import { ClinicManagementComponent } from './components/admin/clinic-management/clinic-management';
import { UserManagement } from './components/admin/user-management/user-management';
import { UserForm } from './components/admin/user-form/user-form';
import { AppointmentManagementComponent } from './components/admin/appointment-management/appointment-management';
import { DoctorProfileComponent } from './components/doctor/doctor-profile/doctor-profile';
import { AvailabilityComponent } from './components/doctor/availability/availability';
import { DoctorAppointmentsComponent } from './components/doctor/doctor-appointments/doctor-appointments';
import { BookAppointmentComponent } from './components/patient/book-appointment/book-appointment';
import { PatientAppointmentsComponent } from './components/patient/patient-appointments/patient-appointments';
import { NearbyClinicsComponent } from './components/patient/nearby-clinics/nearby-clinics';
import { PatientProfileComponent } from './components/patient/patient-profile/patient-profile';
import { DoctorSearchComponent } from './components/patient/doctor-search/doctor-search';
import { DoctorProfile } from './components/patient/doctor-profile/doctor-profile';
import { RescheduleAppointment } from './components/patient/reschedule-appointment/reschedule-appointment';
import { DoctorNotifications } from './components/doctor/doctor-notifications/doctor-notifications';
import { PaymentHistory } from './components/patient/payment-history/payment-history';
import { Notifications } from './components/patient/notifications/notifications';
import { NotFoundComponent } from './components/shared/not-found/not-found';
import { AuthGuard } from './core/guards/auth.guard';
import { NoAuthGuard } from './core/guards/no-auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { DoctorGuard } from './core/guards/doctor.guard';
import { PatientGuard } from './core/guards/patient.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [NoAuthGuard] },
  { path: 'home', component: HomeComponent, canActivate: [NoAuthGuard] },
  {
    path: 'auth',
    canActivate: [NoAuthGuard],
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
      { path: 'doctors', component: UserManagement },
      { path: 'patients', component: UserManagement },
      { path: 'users', component: UserManagement },
      { path: 'user-form', component: UserForm },
      { path: 'clinics', component: ClinicManagementComponent },
      { path: 'clinics/create', component: ClinicFormComponent },
      { path: 'clinics/edit/:clinicId', component: ClinicFormComponent },
      { path: 'appointments', component: AppointmentManagementComponent }
    ]
  },
  {
    path: 'doctor',
    canActivate: [AuthGuard, DoctorGuard],
    children: [
      { path: 'profile', component: DoctorProfileComponent },
      { path: 'appointments', component: DoctorAppointmentsComponent },
      { path: 'availability', component: AvailabilityComponent },
      { path: 'calendar', component: AvailabilityComponent },
      { path: 'notifications', component: DoctorNotifications }
    ]
  },
  {
    path: 'patient',
    canActivate: [AuthGuard, PatientGuard],
    children: [
      { path: 'profile', component: PatientProfileComponent },
      { path: 'doctor-search', component: DoctorSearchComponent },
      { path: 'doctor-profile/:id', component: DoctorProfile },
      { path: 'book-appointment', component: BookAppointmentComponent },
      { path: 'reschedule-appointment/:id', component: RescheduleAppointment },
      { path: 'appointments', component: PatientAppointmentsComponent },
      { path: 'payment-history', component: PaymentHistory },
      { path: 'nearby-clinics', component: NearbyClinicsComponent },
      { path: 'notifications', component: Notifications }
    ]
  },
  { path: '**', component: NotFoundComponent }
];
