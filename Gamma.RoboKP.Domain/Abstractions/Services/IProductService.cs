using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Services;

public interface IProductService
{
    Task<long> CreateProduct(ProductEntity product);
    Task<ProductEntity?> GetProduct(long id);
    Task<List<ProductEntity>> GetProducts();
    Task<List<ProductEntity>?> GetProductsByName(string name);
    Task<List<ProductEntity>?> GetProductByPrice(decimal price);
    Task<List<ProductEntity>?> GetProductByCategory(long? categoryId = null, long? subCategoryId = null);
    Task<ProductEntity?> GetProductWithDiscount(long id, string status);
    Task<DiscountEntity?> GetDiscount(string status);
    Task<List<ProductEntity>?> SearchAndFilter(string? name = null, decimal? exactPrice = null,
        decimal? minPrice = null, decimal? maxPrice = null, long? categoryId = null, long? subCategoryId = null,
        int page = 1, int pageSize = 50);
    Task<long> UpdateProductData(long id, string name, string description, decimal price, SubCategoryEntity subCategory);
    Task<long> UpdateProductImage(long id, string imageUrl);
    Task<bool> RemoveProduct(long id);
}