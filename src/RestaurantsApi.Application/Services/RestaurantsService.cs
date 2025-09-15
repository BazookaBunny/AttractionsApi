
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RestaurantsApi.Infrastructure.Repositories.Implementations;

namespace RestaurantsApi.Application.Services;

public class RestaurantsService : IRestaurantsService
{
    private readonly IRestaurantsRepository _repository;
    private readonly IMemoryCache _cache;
    //cache key
    private const string RestaurantsCacheKey = "all_restaurants";

    public RestaurantsService(IRestaurantsRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task AddRestaurantAsync(Restaurant restaurant)
    {
        await _repository.AddAsync(restaurant);
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        //get cached restaurants if exist
        if (_cache.TryGetValue(RestaurantsCacheKey, out IEnumerable<Restaurant> cachedRestaurants))
        {
            return cachedRestaurants;
        }

        //else
        var restaurants = await _repository.GetAllAsync();

        //cache the restaurants for 5 minutes
        _cache.Set(
            RestaurantsCacheKey,
            restaurants,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromSeconds(20) // optional: extend if accessed often
            }
        );

        return restaurants;
    }

    public async Task<Restaurant> GetRestaurantByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}