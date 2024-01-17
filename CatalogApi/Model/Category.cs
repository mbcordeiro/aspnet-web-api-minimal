using System.Collections.ObjectModel;

namespace CatalogApi.Model
{
    public class Category
    {
        public Category()
        {
            Products = new Collection<Product>();
        }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
