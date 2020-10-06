using System;

namespace RobicServer.Models.DTOs
{
    public class ExerciseDefinitionForListDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime LastActive { get; set; }
    }
}