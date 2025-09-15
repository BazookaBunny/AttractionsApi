using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RestaurantsApi.Infrastructure.Repositories.Implementations;

public class RestaurantsRepository : IRestaurantsRepository
{
    private readonly AppDbContext _context;
    public RestaurantsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync() => await _context.Restaurants.ToListAsync();

    public async Task<Restaurant?> GetByIdAsync(int id) => await _context.Restaurants.FindAsync(id);

    public async Task AddAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();
    }


}