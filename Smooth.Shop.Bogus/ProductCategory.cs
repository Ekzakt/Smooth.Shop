namespace Smooth.Shop.Bogus;

#nullable disable

public class ProductCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public List<Product> Products { get; set; }
}
