namespace Smooth.Shop.FakeData;

#nullable disable

public class ProductCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<Product> Products { get; set; }

    public DateTime DateCreated { get; set; }

    public bool IsNew => DateTime.Now.Subtract(DateCreated).TotalDays <= 7;
}
