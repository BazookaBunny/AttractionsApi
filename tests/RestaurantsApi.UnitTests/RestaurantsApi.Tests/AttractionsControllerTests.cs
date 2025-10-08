using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantsApi.Application.Services;

namespace RestaurantsApi.Tests;

public class AttractionsControllerTests
{

    //     The TDD Cycle

    // Red → Write a failing test (the feature doesn’t exist yet).

    // Green → Write the simplest code to make the test pass.

    // Refactor → Improve the code while keeping tests green.

    [Fact]
    public async Task GetAllRestarants_ShouldReturnOk()
    {
        var mockService = new Mock<IRestaurantsService>();
        var memoryCache = new Mock<IMemoryCache>();

        mockService.Setup(
            service => service.GetAllRestaurantsAsync()
        ).ReturnsAsync(
            new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Testaurant", Category = "Test" }
            }
        );

        //test
        var result = await mockService.Object.GetAllRestaurantsAsync();
        //assert
        var listResult = result.Should().BeAssignableTo<IEnumerable<Restaurant>>();
        listResult.Subject.Should().HaveCount(1);
        //
        result.First().Name.Should().Be("Testaurant");
    }

    [Fact]
    public async Task GetRestaurantById_ShouldReturnOk()
    {
        var mockService = new Mock<IRestaurantsService>();
        var memoryCache = new Mock<IMemoryCache>();

        mockService.Setup(
            service => service.GetRestaurantByIdAsync(1)
        ).ReturnsAsync(
            new Restaurant { Id = 1, Name = "Testaurant", Category = "Test" }
        );

        //test
        var result = await mockService.Object.GetRestaurantByIdAsync(1);
        //assert
        result.Should().BeAssignableTo<Restaurant>();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Testaurant");
    }


}