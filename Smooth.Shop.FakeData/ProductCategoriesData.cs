using Bogus;

namespace Smooth.Shop.FakeData;

public class ProductCategoriesData
{
    public List<ProductCategory> GenerateRandomProductCategoryData()
    {
        var categories = new Faker<ProductCategory>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
            .RuleFor(c => c.DateCreated, f => f.Date.Past())
            .RuleFor(c => c.Products, f => new ProductData().GenerateRandomProductData());

        return categories.Generate(75);
    }
}
