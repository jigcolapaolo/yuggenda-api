using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.BusinessMembers;
using Yuggenda.Application.Exceptions;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.Services.BusinessMembers;

public class BusinessMemberService
{
    private readonly IBusinessMemberRepository _businessMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public BusinessMemberService(
        IBusinessMemberRepository businessMemberRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _businessMemberRepository = businessMemberRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
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
            FirstName = member.User.FirstName,
            LastName = member.User.LastName,
            Email = member.User.Email,
            Role = member.Role.ToString(),
            CreatedAt = member.CreatedAt
        }).ToList();
    }

    public async Task<BusinessMemberResponse> AddAsync(
        Guid businessId,
        AddBusinessMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId must not be empty.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found,");
        }

        var existingMember = await _businessMemberRepository.GetByBusinessAndUserIdAsync(
            businessId,
            request.UserId,
            cancellationToken
        );

        if (existingMember is not null)
        {
            throw new ConflictException(
                "The user is already a member of this business"
            );
        }

        var member = new BusinessMember(
            businessId,
            request.UserId,
            BusinessRole.Staff
        );

        await _businessMemberRepository.AddAsync(member, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BusinessMemberResponse
        {
            Id = member.Id,
            BusinessId = member.BusinessId,
            UserId = member.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = member.Role.ToString(),
            CreatedAt = member.CreatedAt
        };
    }

    public async Task<BusinessMemberResponse> UpdateRoleAsync(
        Guid businessId,
        Guid memberId,
        UpdateBusinessMemberRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!Enum.IsDefined(request.Role))
        {
            throw new ArgumentException("Invalid business member role.");
        }

        if (request.Role == BusinessRole.Owner)
        {
            throw new ArgumentException(
                "The Owner role cannot be assigned through this endpoint."
            );
        }

        var member = await _businessMemberRepository.GetByIdAsync(
            memberId,
            cancellationToken
        );

        if (member is null || member.BusinessId != businessId)
        {
            throw new KeyNotFoundException("Business member not found.");
        }

        if (member.Role == BusinessRole.Owner)
        {
            throw new ConflictException(
                "The business owner role cannot be changed directly. Transfer ownership first."
            );
        }

        member.ChangeRole(request.Role);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BusinessMemberResponse
        {
            Id = member.Id,
            BusinessId = member.BusinessId,
            UserId = member.UserId,
            FirstName = member.User.FirstName,
            LastName = member.User.LastName,
            Email = member.User.Email,
            Role = member.Role.ToString(),
            CreatedAt = member.CreatedAt
        };
    }

    public async Task RemoveAsync(
        Guid businessId,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        var member = await _businessMemberRepository.GetByIdAsync(
            memberId,
            cancellationToken
        );

        if (member is null || member.BusinessId != businessId)
        {
            throw new KeyNotFoundException("Business member not found");
        }

        if (member.Role == BusinessRole.Owner)
        {
            throw new ConflictException(
                "The business owner cannot be removed"
            );
        }

        await _businessMemberRepository.RemoveAsync(member, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task LeaveAsync(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.UserId;

        var member = await _businessMemberRepository.GetByBusinessAndUserIdAsync(
            businessId,
            userId,
            cancellationToken
        );

        if (member is null)
        {
            throw new KeyNotFoundException(
                "You are not a member of this business"
            );
        }

        if (member.Role == BusinessRole.Owner)
        {
            throw new ConflictException(
                "The business owner must transfer ownership before leaving"
            );
        }

        await _businessMemberRepository.RemoveAsync(member, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task TransferOwnershipAsync(
        Guid businessId,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        var currentOwner = await _businessMemberRepository.GetByBusinessAndUserIdAsync(
            businessId,
            _currentUser.UserId,
            cancellationToken
        );

        if (currentOwner is null || currentOwner.Role != BusinessRole.Owner)
        {
            throw new UnauthorizedAccessException(
                "Only the current business owner can transfer ownership"
            );
        }

        var newOwner = await _businessMemberRepository.GetByIdAsync(memberId, cancellationToken);

        if (newOwner is null || newOwner.BusinessId != businessId)
        {
            throw new KeyNotFoundException(
                "Business member not found"
            );
        }

        if (newOwner.Id == currentOwner.Id)
        {
            throw new ConflictException(
                "The current owner already owns this business"
            );
        }

        currentOwner.ChangeRole(BusinessRole.Admin);
        newOwner.ChangeRole(BusinessRole.Owner);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}