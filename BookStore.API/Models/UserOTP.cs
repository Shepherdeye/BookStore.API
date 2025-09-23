using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class UserOTP
    {
        [Key]
        public int Id { get; set; }       
        public string? ApplicationUserId { get; set; }
        [Required]
        public ApplicationUser? ApplicationUser { get; set; }
        public  int OTPNumber { get; set; }
        public DateTime ValidTo { get; set; }
        
    }
}
