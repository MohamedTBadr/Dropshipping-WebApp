using DAL.Models;

namespace DAL.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public decimal Price { get; set; }
        public int ModelYear { get; set; }

        public ICollection<ProductImages> Images { get; set; }
    }
}


