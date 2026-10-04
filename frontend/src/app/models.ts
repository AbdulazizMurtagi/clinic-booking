export interface Doctor {
  id: number;
  name: string;
  specialty: string;
}

export interface Patient {
  id: number;
  fullName: string;
  phoneNumber: string;
  dateOfBirth: string;
}

export interface Appointment {
  id: number;
  doctorId: number;
  patientId: number;
  startTime: string;
}