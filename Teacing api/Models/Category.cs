namespace Teacing_api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // Навигационное свойство: у одной категории список (коллекция) товаров
        public List<Product> Products { get; set; } = new();
    }
}
