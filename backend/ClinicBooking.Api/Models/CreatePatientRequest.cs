using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Api.Models;

public class CreatePatientRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?[0-9]{8,15}$", ErrorMessage = "Phone number must be 8 to 15 digits and may start with +.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateOnly? DateOfBirth { get; set; }
}