using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Abstractions.Persistence;

public interface IServiceRepository
{
    Task AddAsync(
        Service service,
        CancellationToken cancellationToken
    );

    Task<Service?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    );


    Task<List<Service>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken,
        bool activeOnly = false
    );



    Task RemoveAsync(
        Service service,
        CancellationToken cancellationToken
    );
}
