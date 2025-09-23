using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Response
{
    public class UpdatePersonalInfoResponseDTO
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string? PhoneNumber { get; set; }
        public string? Addresse { get; set; }  
        public string? ProfileImage { get; set; }

    }
}
