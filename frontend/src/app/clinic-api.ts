import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Appointment, Doctor, Patient } from './models';

@Injectable({ providedIn: 'root' })
export class ClinicApi {
  private http = inject(HttpClient);

  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>('/api/doctors');
  }

  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>('/api/patients');
  }

  createPatient(fullName: string, phoneNumber: string, dateOfBirth: string): Observable<Patient> {
    return this.http.post<Patient>('/api/patients', { fullName, phoneNumber, dateOfBirth });
  }

  getAppointments(doctorId: number, date: string): Observable<Appointment[]> {
    return this.http.get<Appointment[]>('/api/appointments', { params: { doctorId, date } });
  }

  bookAppointment(doctorId: number, patientId: number, startTime: string): Observable<Appointment> {
    return this.http.post<Appointment>('/api/appointments', { doctorId, patientId, startTime });
  }

  cancelAppointment(id: number): Observable<void> {
    return this.http.delete<void>(`/api/appointments/${id}`);
  }
}