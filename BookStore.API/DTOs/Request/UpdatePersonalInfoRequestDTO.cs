using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class UpdatePersonalInfoRequestDTO
    {

        [Required]
        public string ApplicationUserId { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string? PhoneNumber { get; set; }
        public string? Addresse { get; set; }
        public string? ProfileImage { get; set; }

    }
}
