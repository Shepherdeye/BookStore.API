using System.Text.Json.Serialization;

namespace BookStore.API.Models
{
    public class Auther
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //[JsonIgnore]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
