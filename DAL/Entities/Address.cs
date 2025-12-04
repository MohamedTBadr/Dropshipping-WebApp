using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [NotMapped]
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
