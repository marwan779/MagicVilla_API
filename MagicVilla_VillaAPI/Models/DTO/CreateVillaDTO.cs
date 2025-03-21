﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class CreateVillaDTO
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        [Required]
        public double Rate { get; set; }
        public int Occupancy { get; set; }
        public int Sqft { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public IFormFile? Image {get; set; }
        public string Amenity { get; set; } = string.Empty;

    }
}
