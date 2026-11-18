using System.ComponentModel.DataAnnotations;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.DTOs.Appointments;

public class AppointmentQueryRequest
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }

    public AppointmentStatus? Status { get; set; }

    public Guid? BusinessMemberId { get; set; }
    public Guid? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}