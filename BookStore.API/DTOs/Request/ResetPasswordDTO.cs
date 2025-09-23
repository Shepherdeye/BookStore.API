using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class ResetPasswordDTO
    {
   
        [Required]
        public int OTPNumber { get; set; } 
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
