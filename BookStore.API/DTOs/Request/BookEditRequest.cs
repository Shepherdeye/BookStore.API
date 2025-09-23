using Microsoft.EntityFrameworkCore;

namespace BookStore.API.DTOs.Request
{
    public class BookEditRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageUrl { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int AutherId { get; set; }
    }
}
