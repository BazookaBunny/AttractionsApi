using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=db.bkrksjufthuxzrnjmszh.supabase.co;Database=postgres;Username=postgres;Password=S$tanefAndrei;SSL Mode=Require;Trust Server Certificate=true");

        return new AppDbContext(optionsBuilder.Options);
    }
}