using ClinicBooking.Api.Models;

namespace ClinicBooking.Api.Services;

public enum BookingStatus
{
    Success,
    InvalidTime,
    DoctorNotFound,
    PatientNotFound,
    SlotTaken,
    PatientAlreadyBooked
}

public class BookingResult
{
    public BookingStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public Appointment? Appointment { get; set; }
}