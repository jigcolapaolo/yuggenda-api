using Microsoft.EntityFrameworkCore;

namespace Yuggenda.Infrastructure.Persistence.Context;

public class YuggendaDbContext : DbContext
{
    public YuggendaDbContext(DbContextOptions<YuggendaDbContext> options): base(options) {}
}