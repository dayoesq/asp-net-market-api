using Market.DataContext;
using Market.Models;

namespace Market.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductService> AddProductToCategory(int productId, int categoryId)
    {
        var productCategory = new ProductCategory
        {
            ProductId = productId,
            CategoryId = categoryId
        };
        _context.ProductCategories.Add(productCategory);
        await _context.SaveChangesAsync();
        return this;
    }


}
