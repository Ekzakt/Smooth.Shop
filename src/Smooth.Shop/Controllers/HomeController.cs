using Microsoft.AspNetCore.Mvc;
using Smooth.Shop.FakeData;
using Smooth.Shop.Models;
using System.Diagnostics;

namespace Smooth.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ProductData _productData;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ProductData productData)
        {
            _logger = logger;
            _configuration = configuration;
            _productData = productData;
        }


        public IActionResult Index()
        {
            var products = _productData.GenerateRandomProductData();
            return View(products);
        }


        public IActionResult ProductCategories()
        {
            var categories = new ProductCategoriesData().GenerateRandomProductCategoryData();
            return View(categories);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
