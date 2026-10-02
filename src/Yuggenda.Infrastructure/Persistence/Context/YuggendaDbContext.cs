using Microsoft.EntityFrameworkCore;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Context;

public class YuggendaDbContext : DbContext
{
    public YuggendaDbContext(DbContextOptions<YuggendaDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessMember> BusinessMembers => Set<BusinessMember>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(YuggendaDbContext).Assembly);
    }
}
