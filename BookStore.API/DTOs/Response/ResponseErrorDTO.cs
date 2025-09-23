namespace BookStore.API.DTOs.Response
{
    public class ResponseErrorDTO
    {
        public string Msg { get; set; }
        public string TraceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
