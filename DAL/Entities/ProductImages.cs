using DAL.Models;

namespace DAL.Models
{
    public class ProductImages:BaseEntity
    {
        public string Image { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}


