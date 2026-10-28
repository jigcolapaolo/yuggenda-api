using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Availabilities;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Application.Services.Availabilities;

public class AvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IBusinessMemberRepository _businessMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AvailabilityService(
        IAvailabilityRepository availabilityRepository,
        IBusinessMemberRepository businessMemberRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _availabilityRepository = availabilityRepository;
        _businessMemberRepository = businessMemberRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<AvailabilityResponse> CreateAsync(
        Guid businessId,
        Guid businessMemberId,
        CreateAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var businessMember = await _businessMemberRepository.GetByIdAsync(
            businessMemberId,
            cancellationToken
        );

        if (businessMember is null || businessMember.BusinessId != businessId)
        {
            throw new KeyNotFoundException(
                "Business member not found in the specified business"
            );
        }

        var currentMember =
            await _businessMemberRepository.GetByBusinessAndUserIdAsync(
                businessId,
                _currentUser.UserId,
                cancellationToken
            );

        if (currentMember is null)
        {
            throw new UnauthorizedAccessException(
                "You are not a member of this business."
            );
        }

        if (
            currentMember.Role == BusinessRole.Staff
            && currentMember.Id != businessMember.Id
        )
        {
            throw new UnauthorizedAccessException(
                "Staff members can only manage their own availability."
            );
        }

        var existingAvailabilities =
            await _availabilityRepository.GetByBusinessMemberAndTypeAsync(
                businessMemberId,
                request.Type,
                cancellationToken
            );
        
        var hasOverlap = existingAvailabilities.Any(existing =>
        {
            var samePeriod = request.Type switch
            {
                AvailabilityType.Weekly => 
                    existing.DayOfWeek == request.DayOfWeek,

                AvailabilityType.Exception =>
                    existing.SpecificDate == request.SpecificDate,

                _ => false
            };

            var overlaps =
                existing.StartTime < request.EndTime &&
                request.StartTime < existing.EndTime;

            return samePeriod && overlaps;
        });

        if (hasOverlap)
        {
            throw new DomainException(
                "The availability interval overlaps an existing interval."
            );
        }

        var availability = new Availability(
            businessMemberId,
            request.Type,
            request.StartTime,
            request.EndTime,
            request.IsAvailable,
            request.DayOfWeek,
            request.SpecificDate
        );

        await _availabilityRepository.AddAsync(
            availability,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(availability);
    }

    public async Task<List<AvailabilityResponse>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken
    )
    {
        var availabilities =
            await _availabilityRepository.GetByBusinessIdAsync(
                businessId,
                cancellationToken
            );

        return availabilities
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AvailabilityResponse> UpdateAsync(
        Guid id,
        UpdateAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var availability = await _availabilityRepository.GetByIdAsync(
            id,
            cancellationToken
        );

        if (availability is null)
        {
            throw new KeyNotFoundException("Availability not found.");
        }

        var businessMember = await _businessMemberRepository.GetByIdAsync(
            availability.BusinessMemberId,
            cancellationToken
        );

        if (businessMember is null)
        {
            throw new KeyNotFoundException("Business member not found.");
        }

        var currentMember =
            await _businessMemberRepository.GetByBusinessAndUserIdAsync(
                businessMember.BusinessId,
                _currentUser.UserId,
                cancellationToken
            );

        if (currentMember is null)
        {
            throw new UnauthorizedAccessException(
                "You are not a member of this business."
            );
        }

        if (
            currentMember.Role == BusinessRole.Staff
            && currentMember.Id != availability.BusinessMemberId
        )
        {
            throw new UnauthorizedAccessException(
                "Staff members can only manage their own availability."
            );
        }

        var dayOfWeek = request.DayOfWeek ?? availability.DayOfWeek;
        var specificDate = request.SpecificDate ?? availability.SpecificDate;
        var startTime = request.StartTime ?? availability.StartTime;
        var endTime = request.EndTime ?? availability.EndTime;
        var isAvailable = request.IsAvailable ?? availability.IsAvailable;

        var type = availability.Type;

        var existingAvailabilities =
            await _availabilityRepository.GetByBusinessMemberAndTypeAsync(
                availability.BusinessMemberId,
                type,
                cancellationToken
            );

        var hasOverlap = existingAvailabilities.Any(existing =>
        {
            if (existing.Id == availability.Id)
            {
                return false;
            }

            var samePeriod = type switch
            {
                AvailabilityType.Weekly =>
                    existing.DayOfWeek == dayOfWeek,

                AvailabilityType.Exception =>
                    existing.SpecificDate == specificDate,

                _ => false
            };

            var overlaps =
                existing.StartTime < endTime &&
                startTime < existing.EndTime;

            return samePeriod && overlaps;
        });

        if (hasOverlap)
        {
            throw new DomainException(
                "The availability interval overlaps an existing interval."
            );
        }

        availability.Update(
            startTime,
            endTime,
            isAvailable,
            dayOfWeek,
            specificDate
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(availability);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var availability = await _availabilityRepository.GetByIdAsync(
            id,
            cancellationToken
        );

        if (availability is null)
        {
            throw new KeyNotFoundException("Availability not found.");
        }

        var businessMember = await _businessMemberRepository.GetByIdAsync(
            availability.BusinessMemberId,
            cancellationToken
        );

        if (businessMember is null)
        {
            throw new KeyNotFoundException("Business member not found.");
        }

        var currentMember =
            await _businessMemberRepository.GetByBusinessAndUserIdAsync(
                businessMember.BusinessId,
                _currentUser.UserId,
                cancellationToken
            );

        if (currentMember is null)
        {
            throw new UnauthorizedAccessException(
                "You are not a member of this business."
            );
        }

        if (
            currentMember.Role == BusinessRole.Staff
            && currentMember.Id != availability.BusinessMemberId
        )
        {
            throw new UnauthorizedAccessException(
                "Staff members can only manage their own availability."
            );
        }

        await _availabilityRepository.RemoveAsync(
            availability,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static AvailabilityResponse MapToResponse(Availability availability)
    {
        return new AvailabilityResponse
        {
            Id = availability.Id,
            BusinessMemberId = availability.BusinessMemberId,
            Type = availability.Type,
            DayOfWeek = availability.DayOfWeek,
            SpecificDate = availability.SpecificDate,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            IsAvailable = availability.IsAvailable
        };
    }
}