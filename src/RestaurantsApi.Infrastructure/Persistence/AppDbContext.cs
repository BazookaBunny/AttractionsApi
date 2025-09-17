
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);

    //     // Hash pentru parola "Admin@123!"
    //     // var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123!");

    //     // Folosim un GUID static, nu Guid.NewGuid()
    //     var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    //     modelBuilder.Entity<UserModel>().HasData(
    //         new UserModel("admin", "$2a$10$k4sOQ9MhxNGE4UJh7X0E.Oa0rsXZ4nFyWZs2YqrfUM2bGcOhtO9zC", UserRole.Admin)
    //         {
    //             Id = adminId
    //         }
    //     );
    // }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<UserModel> Users { get; set; }
}