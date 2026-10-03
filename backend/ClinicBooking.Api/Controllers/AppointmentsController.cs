using ClinicBooking.Api.Models;
using ClinicBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly BookingService _bookingService;

    public AppointmentsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public ActionResult<Appointment> Book(CreateAppointmentRequest request)
    {
        BookingResult result = _bookingService.Book(
            request.DoctorId!.Value,
            request.PatientId!.Value,
            request.StartTime!.Value,
            DateTime.Now);

        if (result.Status == BookingStatus.Success)
        {
            return StatusCode(201, result.Appointment);
        }

        if (result.Status == BookingStatus.DoctorNotFound || result.Status == BookingStatus.PatientNotFound)
        {
            return Problem(detail: result.ErrorMessage, statusCode: 404);
        }

        if (result.Status == BookingStatus.SlotTaken)
        {
            return Problem(detail: result.ErrorMessage, statusCode: 409);
        }

        return Problem(detail: result.ErrorMessage, statusCode: 400);
    }
}