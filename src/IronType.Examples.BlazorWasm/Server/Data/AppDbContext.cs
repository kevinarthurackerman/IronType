namespace IronType.Examples.BlazorWasm.Server.Data;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
