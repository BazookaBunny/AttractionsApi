using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantsApi.Infrastructure.Repositories.Implementations;

public interface IRestaurantsRepository
{
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<Restaurant> GetByIdAsync(int id);
    Task AddAsync(Restaurant restaurant);
}