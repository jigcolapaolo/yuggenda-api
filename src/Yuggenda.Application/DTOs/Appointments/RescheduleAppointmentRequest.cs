using System.ComponentModel.DataAnnotations;

public class RescheduleAppointmentRequest
{
    [Required]
    public DateTimeOffset StartTime { get; set; }
}