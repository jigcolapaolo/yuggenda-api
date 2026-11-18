using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Appointments;
using Yuggenda.Application.Services.Appointments;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(appointment);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<AppointmentResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.CancelAsync(
            id,
            cancellationToken);

        return Ok(appointment);
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<ActionResult<AppointmentResponse>> Confirm(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.ConfirmAsync(
            id, cancellationToken);

        return Ok(appointment);
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<AppointmentResponse>> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.CompleteAsync(
            id, cancellationToken);

        return Ok(appointment);
    }

    [HttpPatch("{id:guid}/reschedule")]
    public async Task<ActionResult<AppointmentResponse>> Reschedule(
        Guid id,
        [FromBody] RescheduleAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.RescheduleAsync(
            id,
            request,
            cancellationToken);

        return Ok(appointment);
    }
}