namespace BssRms.Domain.Entities;

public class Food
{
    public int FoodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DiscountType { get; set; }
    public decimal Discount { get; set; }
    public string Image { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
