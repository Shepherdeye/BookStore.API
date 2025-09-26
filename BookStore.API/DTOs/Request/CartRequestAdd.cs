namespace BookStore.API.DTOs.Request
{
    public class CartRequestAdd
    {
        public int BookId { get; set; }
        public int Count { get; set; }
    }
}
