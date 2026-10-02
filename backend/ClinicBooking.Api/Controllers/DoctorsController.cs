using ClinicBooking.Api.Data;
using ClinicBooking.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly DoctorRepository _repository;

    public DoctorsController(DoctorRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public List<Doctor> GetAll()
    {
        return _repository.GetAll();
    }
}