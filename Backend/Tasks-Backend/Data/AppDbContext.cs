using Microsoft.EntityFrameworkCore;
using Tasks_Backend.Entities;

namespace Tasks_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
    }
}