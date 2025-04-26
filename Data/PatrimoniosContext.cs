using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Patrimonios.Models;

namespace Patrimonios.Data;

public class PatrimoniosContext : IdentityDbContext<PatrimoniosUser>
{
    public DbSet<PatrimoniosUser> Usuarios {  get; set; } 

    public DbSet<PatrimoniosModel> Patrimonios { get; set; }

    public DbSet<LogEntry> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public PatrimoniosContext(DbContextOptions<PatrimoniosContext> options)
        : base(options)
    {
    }
}
