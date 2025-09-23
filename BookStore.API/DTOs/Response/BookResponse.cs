using Microsoft.EntityFrameworkCore;

namespace BookStore.API.DTOs.Response
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Rate { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int AutherId { get; set; }
        public string AutherName { get; set;} 
   
    }
}
