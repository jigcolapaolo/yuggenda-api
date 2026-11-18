namespace Yuggenda.Application.DTOs.Appointments;

public class AppointmentListResponse
{
    public List<AppointmentResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}