using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class AutherRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
