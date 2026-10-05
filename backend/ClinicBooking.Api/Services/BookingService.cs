using ClinicBooking.Api.Data;
using ClinicBooking.Api.Models;

namespace ClinicBooking.Api.Services;

public class BookingService
{
    private const int OpeningHour = 8;
    private const int ClosingHour = 16;

    private readonly DoctorRepository _doctors;
    private readonly PatientRepository _patients;
    private readonly AppointmentRepository _appointments;

    public BookingService(DoctorRepository doctors, PatientRepository patients, AppointmentRepository appointments)
    {
        _doctors = doctors;
        _patients = patients;
        _appointments = appointments;
    }

    public BookingResult Book(int doctorId, int patientId, DateTime start, DateTime now)
    {
        if (_doctors.GetById(doctorId) == null)
        {
            return new BookingResult { Status = BookingStatus.DoctorNotFound, ErrorMessage = "Doctor not found." };
        }

        if (_patients.GetById(patientId) == null)
        {
            return new BookingResult { Status = BookingStatus.PatientNotFound, ErrorMessage = "Patient not found." };
        }

        string? timeError = CheckTimeRules(start, now);
        if (timeError != null)
        {
            return new BookingResult { Status = BookingStatus.InvalidTime, ErrorMessage = timeError };
        }

        if (_appointments.IsSlotTaken(doctorId, start))
        {
            return new BookingResult { Status = BookingStatus.SlotTaken, ErrorMessage = "This slot is already booked." };
        }

        if (_appointments.HasUpcomingAppointment(patientId, now))
        {
            return new BookingResult { Status = BookingStatus.PatientAlreadyBooked, ErrorMessage = "This patient already has an upcoming appointment. Cancel it before booking another." };
        }
        
        Appointment appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            StartTime = start
        };

        Appointment created = _appointments.Add(appointment);
        return new BookingResult { Status = BookingStatus.Success, Appointment = created };
    }

    public string? CheckTimeRules(DateTime start, DateTime now)
    {
        if ((start.Minute != 0 && start.Minute != 30) || start.Second != 0)
        {
            return "Appointments must start on the hour or the half hour.";
        }

        if (start.DayOfWeek == DayOfWeek.Friday || start.DayOfWeek == DayOfWeek.Saturday)
        {
            return "The clinic is closed on Fridays and Saturdays.";
        }

        if (start.Hour < OpeningHour || start.Hour >= ClosingHour)
        {
            return "Appointments must be between 8:00 and 16:00.";
        }

        if (start < now)
        {
            return "Appointments can't be booked in the past.";
        }

        return null;
    }
}