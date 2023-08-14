using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required] public string KnownAs { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; } // optional to make required work because it has a default value
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        public string Password { get; set; }
    }
}