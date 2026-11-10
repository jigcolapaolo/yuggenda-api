using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Appointments;
using Yuggenda.Application.Services.Appointments;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("businesses/{businessId:guid}/appointments")]
public class BusinessAppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public BusinessAppointmentsController(
        AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create(
        Guid businessId,
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.CreateAsync(
            businessId,
            request,
            cancellationToken
        );

        return Created(
            $"/businesses/{businessId}/appointments/{appointment.Id}",
            appointment
        );
    }
}