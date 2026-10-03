using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Api.Models;

public class CreateAppointmentRequest
{
    [Required(ErrorMessage = "Doctor is required.")]
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Patient is required.")]
    public int? PatientId { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime? StartTime { get; set; }
}