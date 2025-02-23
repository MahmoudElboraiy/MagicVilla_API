﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
