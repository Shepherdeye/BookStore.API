using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Request
{
    public class ForgetPasswordDTO
    {
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
