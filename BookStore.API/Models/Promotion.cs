using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }=string.Empty;
        public DateTime ValidTo { get; set; }
        public int NumberOfUse { get; set; }
        public bool Status { get; set; }
        public int Discount { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
