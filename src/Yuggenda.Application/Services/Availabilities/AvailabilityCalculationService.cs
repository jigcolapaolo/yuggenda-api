using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Availabilities;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.Services.Availabilities;

public class AvailabilityCalculationService
{
    private const int SlotIntervalMinutes = 15;

    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessMemberRepository _businessMemberRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly TimeProvider _timeProvider;

    public AvailabilityCalculationService(
        IAvailabilityRepository availabilityRepository,
        IServiceRepository serviceRepository,
        IBusinessMemberRepository businessMemberRepository,
        IBusinessRepository businessRepository,
        TimeProvider timeProvider
    )
    {
        _availabilityRepository = availabilityRepository;
        _serviceRepository = serviceRepository;
        _businessMemberRepository = businessMemberRepository;
        _businessRepository = businessRepository;
        _timeProvider = timeProvider;
    }

    public async Task<List<AvailabilityInterval>> GetAvailableIntervalsAsync(
        Guid businessMemberId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        var availabilities =
            await _availabilityRepository.GetByBusinessMemberIdAsync(
                businessMemberId,
                cancellationToken
            );

        var weeklyIntervals = availabilities
            .Where(availability =>
                availability.Type == AvailabilityType.Weekly
                && availability.DayOfWeek == date.DayOfWeek
                && availability.IsAvailable
            )
            .Select(availability => new AvailabilityInterval
            {
                StartTime = date.ToDateTime(availability.StartTime),
                EndTime = date.ToDateTime(availability.EndTime)
            });

        var exceptionAvailabilities = availabilities
            .Where(availability =>
                availability.Type == AvailabilityType.Exception
                && availability.SpecificDate == date
            )
            .ToList();

        var exceptionIntervals = exceptionAvailabilities
            .Where(availability => availability.IsAvailable)
            .Select(availability => new AvailabilityInterval
            {
                StartTime = date.ToDateTime(availability.StartTime),
                EndTime = date.ToDateTime(availability.EndTime)
            });

        var blockedIntervals = exceptionAvailabilities
            .Where(availability => !availability.IsAvailable)
            .Select(availability => new AvailabilityInterval
            {
                StartTime = date.ToDateTime(availability.StartTime),
                EndTime = date.ToDateTime(availability.EndTime)
            })
            .ToList();

        var availableIntervals = MergeIntervals(
            weeklyIntervals.Concat(exceptionIntervals)
        );

        return SubtractBlockedIntervals(
            availableIntervals,
            blockedIntervals
        );
    }
 
    public async Task<List<AvailabilitySlotResponse>> GetAvailableSlotsForServiceAsync(
        Guid businessId,
        Guid serviceId,
        DateOnly date,
        CancellationToken cancellationToken,
        Guid? businessMemberId = null
    )
    {
        var business = await _businessRepository.GetByIdAsync(
            businessId,
            cancellationToken
        );

        if (business is null)
        {
            throw new KeyNotFoundException("Business not found.");
        }

        var today = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(
                _timeProvider.GetUtcNow(),
                TimeZoneInfo.FindSystemTimeZoneById(business.Timezone)
            ).DateTime
        );

        if (date < today)
        {
            throw new ArgumentException(
                "The requested date cannot be in the past.",
                nameof(date)
            );
        }

        var maximumBookingDate = today.AddDays(
            BookingPolicy.MaximumBookingHorizonDays
        );

        if (date > maximumBookingDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(date),
                "The requested date exceeds the booking horizon."
            );
        }

        var service = await _serviceRepository.GetByIdWithMembersAsync(
            serviceId,
            cancellationToken
        );

        if (service is null)
        {
            throw new KeyNotFoundException("Service not found.");
        }

        if (service.BusinessId != businessId)
        {
            throw new InvalidOperationException(
                "The service does not belong to this business."
            );
        }

        if (!service.IsActive)
        {
            throw new InvalidOperationException("Service is inactive.");
        }

        var assignedMembers = service.Members
            .Where(member =>
                !businessMemberId.HasValue
                || member.Id == businessMemberId.Value
            )
            .ToList();

        if (
            businessMemberId.HasValue
            && assignedMembers.Count == 0
        )
        {
            throw new InvalidOperationException(
                "The business member is not assigned to this service."
            );
        }

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(business.Timezone);

        var now = TimeZoneInfo.ConvertTime(
            _timeProvider.GetUtcNow(),
            timeZone
        );

        var earliestStart = now.DateTime.AddMinutes(
            BookingPolicy.MinimumAdvanceMinutes
        );

        var latestStart = now.DateTime.AddDays(
            BookingPolicy.MaximumBookingHorizonDays
        );

        var slots = new List<AvailabilitySlotResponse>();

        foreach (var member in assignedMembers)
        {
            var availableIntervals = await GetAvailableIntervalsAsync(
                member.Id,
                date,
                cancellationToken
            );

            foreach (var interval in availableIntervals)
            {
                var minutesSinceMidnight =
                    interval.StartTime.Hour * 60
                    + interval.StartTime.Minute;

                var remainder = minutesSinceMidnight % SlotIntervalMinutes;

                var minutesToAdd = remainder == 0
                    ? 0
                    : SlotIntervalMinutes - remainder;

                var startTime = interval.StartTime.AddMinutes(minutesToAdd);

                while (
                    startTime.AddMinutes(service.DurationMinutes)
                    <= interval.EndTime
                )
                {
                    if (startTime <= earliestStart || startTime > latestStart)
                    {
                        startTime = startTime.AddMinutes(SlotIntervalMinutes);
                        continue;
                    }

                    var endTime = startTime.AddMinutes(service.DurationMinutes);

                    if (
                        !IsValidLocalTime(startTime, timeZone)
                        || !IsValidLocalTime(endTime, timeZone)
                    )
                    {
                        startTime = startTime.AddMinutes(SlotIntervalMinutes);
                        continue;
                    }

                    slots.Add(new AvailabilitySlotResponse
                    {
                        BusinessMemberId = member.Id,
                        ServiceId = service.Id,
                        StartTime = startTime,
                        EndTime = endTime
                    });

                    startTime = startTime.AddMinutes(SlotIntervalMinutes);
                }
            }
        }

        return slots
            .OrderBy(slot => slot.StartTime)
            .ThenBy(slot => slot.BusinessMemberId)
            .ToList();
    }


    private static List<AvailabilityInterval> SubtractBlockedIntervals(
        List<AvailabilityInterval> availableIntervals,
        List<AvailabilityInterval> blockedIntervals
    )
    {
        var result = new List<AvailabilityInterval>();

        foreach (var available in availableIntervals)
        {
            var remaining = new List<AvailabilityInterval>
            {
                available
            };

            foreach (var blocked in blockedIntervals)
            {
                var next = new List<AvailabilityInterval>();

                foreach (var interval in remaining)
                {
                    if (
                        blocked.EndTime <= interval.StartTime
                        || blocked.StartTime >= interval.EndTime
                    )
                    {
                        next.Add(interval);
                        continue;
                    }

                    if (blocked.StartTime > interval.StartTime)
                    {
                        next.Add(new AvailabilityInterval
                        {
                            StartTime = interval.StartTime,
                            EndTime = blocked.StartTime
                        });
                    }

                    if (blocked.EndTime < interval.EndTime)
                    {
                        next.Add(new AvailabilityInterval
                        {
                            StartTime = blocked.EndTime,
                            EndTime = interval.EndTime
                        });
                    }
                }

                remaining = next;
            }

            result.AddRange(remaining);
        }

        return result
            .Where(interval => interval.StartTime < interval.EndTime)
            .OrderBy(interval => interval.StartTime)
            .ToList();
    }

    private static List<AvailabilityInterval> MergeIntervals(
        IEnumerable<AvailabilityInterval> intervals
    )
    {
        var orderedIntervals = intervals
            .OrderBy(interval => interval.StartTime)
            .ThenBy(interval => interval.EndTime)
            .ToList();

        var mergedIntervals = new List<AvailabilityInterval>();

        foreach (var interval in orderedIntervals)
        {
            if (mergedIntervals.Count == 0)
            {
                mergedIntervals.Add(new AvailabilityInterval
                {
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime
                });

                continue;
            }

            var last = mergedIntervals[^1];

            if (interval.StartTime <= last.EndTime)
            {
                if (interval.EndTime > last.EndTime)
                {
                    mergedIntervals[^1] = new AvailabilityInterval
                    {
                        StartTime = last.StartTime,
                        EndTime = interval.EndTime
                    };
                }
            }
            else
            {
                mergedIntervals.Add(new AvailabilityInterval
                {
                    StartTime = interval.StartTime,
                    EndTime = interval.EndTime
                });
            }
        }

        return mergedIntervals;
    }

    private static bool IsValidLocalTime(
        DateTime localDateTime,
        TimeZoneInfo timeZone
    )
    {
        return !timeZone.IsInvalidTime(localDateTime)
            && !timeZone.IsAmbiguousTime(localDateTime);
    }
}