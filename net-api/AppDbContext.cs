using Microsoft.EntityFrameworkCore;
using net_api.Types;

namespace net_api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
}