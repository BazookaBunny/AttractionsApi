
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRestaurantsService
{
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
    Task<Restaurant> GetRestaurantByIdAsync(int id);
    Task AddRestaurantAsync(Restaurant restaurant);
}