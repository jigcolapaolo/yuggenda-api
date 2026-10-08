using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Businesses;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.Services.Businesses;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMemberRepository _businessMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public BusinessService(
        IBusinessRepository businessRepository,
        IBusinessMemberRepository businessMemberRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _businessRepository = businessRepository;
        _businessMemberRepository = businessMemberRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<BusinessResponse> CreateAsync(
        CreateBusinessRequest request,
        CancellationToken cancellationToken
    )
    {
        var business = new Business(
            request.Name,
            request.Timezone,
            request.Description,
            request.Email,
            request.Phone
        );

        var owner = new BusinessMember(
            business.Id,
            _currentUser.UserId,
            BusinessRole.Owner
        );

        await _businessRepository.AddAsync(business, cancellationToken);
        await _businessMemberRepository.AddAsync(owner, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BusinessResponse
        {
            Id = business.Id,
            Name = business.Name,
            Description = business.Description,
            Email = business.Email,
            Phone = business.Phone,
            Timezone = business.Timezone,
            CreatedAt = business.CreatedAt,
            UpdatedAt = business.UpdatedAt
        };
    }
}