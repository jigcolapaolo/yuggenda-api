using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Services;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Services;

public class ServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ServiceService(
        IServiceRepository serviceRepository,
        IBusinessRepository businessRepository,
        IUnitOfWork unitOfWork
    )
    {
        _serviceRepository = serviceRepository;
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse> CreateAsync(
        Guid businessId,
        CreateServiceRequest request,
        CancellationToken cancellationToken
    )
    {
        var business = await _businessRepository.GetByIdAsync(businessId, cancellationToken);

        if (business is null)
        {
            throw new KeyNotFoundException("Business not found");
        }

        var service = new Service(
            businessId,
            request.Name.Trim(),
            request.DurationMinutes,
            request.Price,
            request.Description?.Trim()
        );

        await _serviceRepository.AddAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(service);
    }

    public async Task<ServiceResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);

        if (service is null)
        {
            throw new KeyNotFoundException("Service not found.");
        }

        return MapToResponse(service);
    }


    public async Task<List<ServiceResponse>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken,
        bool activeOnly = false
    )
    {
        var business = await _businessRepository.GetByIdAsync(
            businessId,
            cancellationToken
        );

        if (business is null)
        {
            throw new KeyNotFoundException("Business not found");
        }

        var services = await _serviceRepository.GetByBusinessIdAsync(
            businessId,
            cancellationToken,
            activeOnly
        );

        return services.Select(MapToResponse).ToList();
    }


    public async Task<ServiceResponse> UpdateAsync(
        Guid id,
        UpdateServiceRequest request,
        CancellationToken cancellationToken
    )
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);

        if (service is null)
        {
            throw new KeyNotFoundException("Service not found.");
        }

        service.Update(
            request.Name?.Trim() ?? service.Name,
            request.Description?.Trim() ?? service.Description,
            request.DurationMinutes ?? service.DurationMinutes,
            request.Price ?? service.Price
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(service);
    }

    private static ServiceResponse MapToResponse(Service service)
    {
        return new ServiceResponse
        {
            Id = service.Id,
            BusinessId = service.BusinessId,
            Name = service.Name,
            Description = service.Description,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}