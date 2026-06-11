namespace Teacing_api.Validation
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

    }
}
