using System.ComponentModel.DataAnnotations;

namespace RestaurantsApi.Application.DTOs
{
    public class RestaurantDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}
