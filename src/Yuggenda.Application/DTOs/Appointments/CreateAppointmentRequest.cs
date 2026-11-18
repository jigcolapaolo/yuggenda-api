using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Appointments;

public class CreateAppointmentRequest
{
    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public Guid BusinessMemberId { get; set; }

    [Required]
    public DateTimeOffset StartTime { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }
}