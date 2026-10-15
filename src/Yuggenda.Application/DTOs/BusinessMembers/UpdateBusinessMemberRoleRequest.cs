using System.ComponentModel.DataAnnotations;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.DTOs.BusinessMembers;

public class UpdateBusinessMemberRoleRequest
{
    [Required]
    public BusinessRole Role { get; init; }
}