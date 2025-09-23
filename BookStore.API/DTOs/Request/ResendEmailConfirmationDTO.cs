using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class ResendEmailConfirmationDTO
    {
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
