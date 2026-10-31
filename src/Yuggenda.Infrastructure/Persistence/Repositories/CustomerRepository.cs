using Microsoft.EntityFrameworkCore;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Domain.Entities;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly YuggendaDbContext _context;

    public CustomerRepository(YuggendaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);
    }

    public async Task<Customer?> GetByIdAndBusinessIdAsync(
        Guid id,
        Guid businessId,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(
                customer => customer.Id == id
                    && customer.BusinessId == businessId,
                cancellationToken);
    }

    public async Task<List<Customer>> GetByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Where(customer => customer.BusinessId == businessId)
            .OrderBy(customer => customer.LastName)
            .ThenBy(customer => customer.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForUserAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AnyAsync(
                customer => customer.BusinessId == businessId
                    && customer.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AnyAsync(
                customer => customer.Id == id,
                cancellationToken);
    }
}