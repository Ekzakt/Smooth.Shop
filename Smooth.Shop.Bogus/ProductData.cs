using Bogus;

namespace Smooth.Shop.Bogus;

public class ProductData
{
    public List<Product> GenerateRandomProductData()
    {
        var products = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.IndexFaker)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
            .RuleFor(p => p.Quantity, f => f.Random.Int(1, 1000))
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl());

        return products.Generate(26);
    }
}
