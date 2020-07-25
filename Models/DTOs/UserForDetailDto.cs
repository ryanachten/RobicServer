using System.Collections.Generic;

namespace RobicServer.Models.DTOs
{
    public class UserForDetailDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<string> Exercises { get; set; }
    }
}