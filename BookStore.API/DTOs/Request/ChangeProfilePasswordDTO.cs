using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class ChangeProfilePasswordDTO
    {

        public string ApplicationUserId { get; set; } = null!;
        [Required]
        public string CurrentPassword { get; set; }=string.Empty;
        [Required]
        public string Password { get; set; }=string.Empty;

    }
}
