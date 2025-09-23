using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class ChangePhotoRequest
    {
        [Required]
        public string ApplicationUserId { get; set; }=string.Empty;
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
