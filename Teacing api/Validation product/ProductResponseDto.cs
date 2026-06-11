namespace Teacing_api.Validation_product
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        // Вместо целого объекта категории с кучей лишних данных 
        // мы отдадим просто её понятное текстовое имя!
        public string CategoryName { get; set; } = null!;
    }
}
