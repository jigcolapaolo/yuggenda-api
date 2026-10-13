using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.BusinessMembers;

namespace Yuggenda.Application.Services.BusinessMembers;

public class BusinessMemberService
{
    private readonly IBusinessMemberRepository _businessMemberRepository;

    public BusinessMemberService(
        IBusinessMemberRepository businessMemberRepository
    )
    {
        _businessMemberRepository = businessMemberRepository;
    }

    public async Task<List<BusinessMemberResponse>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var members = await _businessMemberRepository.GetByBusinessIdAsync(
            businessId,
            cancellationToken
        );

        return members.Select(member => new BusinessMemberResponse
        {
            Id = member.Id,
            BusinessId = member.BusinessId,
            UserId = member.UserId,
            Role = member.Role.ToString(),
            CreatedAt = member.CreatedAt
        }).ToList();
    }
}