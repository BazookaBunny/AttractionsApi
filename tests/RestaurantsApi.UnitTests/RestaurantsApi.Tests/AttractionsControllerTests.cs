using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantsApi.Application.Services;

namespace RestaurantsApi.Tests;

public class AttractionsControllerTests
{

    [Fact]
    public async Task GetAllRestaurants_ShouldReturnOk_WithListAsync()
    {
        var mockService = new Mock<RestaurantsService>();
        var loggerMock = new Mock<ILogger<AttractionsController>>();

        mockService.Setup(service => service.GetAllRestaurantsAsync())
            .ReturnsAsync(new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Testaurant", Category = "Test" }
            });

        var controller = new AttractionsController(mockService.Object, loggerMock.Object);

        //test
        var result = await controller.GetAllAttractions();

        //assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var restaurants = okResult.Value.Should().BeAssignableTo<IEnumerable<Restaurant>>().Subject;

        restaurants.Should().HaveCount(1);
        restaurants.First().Name.Should().Be("Testaurant");
    }
}