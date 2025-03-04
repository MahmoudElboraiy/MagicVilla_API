using System.ComponentModel.DataAnnotations;

namespace Magic_villa_web.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
    }
}