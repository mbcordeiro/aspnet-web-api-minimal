using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
