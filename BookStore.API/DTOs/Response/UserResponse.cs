using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Response
{
    public class UserResponse
    {
        [Required]
        public string Id { get; set; }
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; }=string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
