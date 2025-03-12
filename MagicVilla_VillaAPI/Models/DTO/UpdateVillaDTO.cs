using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class UpdateVillaDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        [Required]
        public double Rate { get; set; }
        [Required]
        public int Occupancy { get; set; }
        [Required]
        public int Sqft { get; set; }
        [Required]
        public string? ImageUrl { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string? ImageLocalPath { get; set; } = string.Empty;
        public string Amenity { get; set; } = string.Empty;

    }
}
