using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RobicServer.Models.DTOs
{
    public class ExerciseDefinitionForListDto
    {
        // Inherited properties
        public string Id { get; set; }
        public string Title { get; set; }
        public ICollection<string> History { get; set; }

        // DTO-specific computed properties
        public DateTime? LastActive { get; set; }

        [Range(0, 100, ErrorMessage = "Value for {0} must be a percentage between {1} and {2}")]
        public double? LastImprovement { get; set; }
    }
}