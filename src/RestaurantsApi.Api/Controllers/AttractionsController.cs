using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantsApi.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AttractionsController : ControllerBase
{
    private readonly IRestaurantsService _service;
    private readonly ILogger<AttractionsController> _logger;

    public AttractionsController(IRestaurantsService service, ILogger<AttractionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET: api/attractions
    //USE RESPONSE CACHING FOR SETTING UP HTTP RESPONSE HEADERS
    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetAllAttractions()
    {
        try
        {
            var restaurants = await _service.GetAllRestaurantsAsync();
            return Ok(restaurants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get attractions");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    //get attraction by name
    // GET: api/attractions/byname?name=
    [HttpGet("byname")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        var allAttractions = await _service.GetAllRestaurantsAsync();
        var filteredAttractions = allAttractions.Where(a =>
        a.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(filteredAttractions);
    }


    // GET: api/attractions/bycategory?category=
    [HttpGet("bycategory")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetByCategory([FromQuery] string category)
    {
        var allRestaurants = await _service.GetAllRestaurantsAsync();
        var filtered = allRestaurants
            .Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return Ok(filtered);
    }


    // POST: api/attractions
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAttraction([FromBody] RestaurantDto restaurant)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddRestaurantAsync(new Restaurant
        {
            Name = restaurant.Name,
            Address = restaurant.Address,
            Category = restaurant.Category,
            Latitude = restaurant.Latitude,
            Longitude = restaurant.Longitude,
            ImageUrl = restaurant.ImageUrl
        });

        return Ok();
    }
}
