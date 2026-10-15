using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.BusinessMembers;

public class AddBusinessMemberRequest
{
    [Required]
    public Guid UserId { get; init; }
}