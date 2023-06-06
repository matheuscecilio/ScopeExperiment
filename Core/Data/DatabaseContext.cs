using Core.Data.Configurations;
using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new PersonConfiguration().Configure(modelBuilder.Entity<Person>());

        base.OnModelCreating(modelBuilder);
    }
}
