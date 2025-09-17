using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantsApi.Application.Services;

namespace RestaurantsApi.Tests;

public class RestaurantsControllerTests
{
    private readonly ILogger<AttractionsController> _logger;

    [Fact]
    public void GetAllRestaurants_ShouldReturnOk_WithList()
    {
        var mockService = new Moq.Mock<RestaurantsService>();
        mockService.Setup(service => service.GetAllRestaurantsAsync())
            .ReturnsAsync(new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Testaurant", Category = "Test" }
            });

        var controller = new AttractionsController(mockService.Object, _logger);

        //test
        var result = controller.GetAllAttractions();
        //assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var restaurants = okResult.Value.Should().BeAssignableTo<IEnumerable<Restaurant>>().Subject;

        restaurants.Should().HaveCount(1);
        restaurants.First().Name.Should().Be("Testaurant");
    }
}