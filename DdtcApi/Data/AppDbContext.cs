using DdtcApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DdtcApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Translation> Translations { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
    }
}
