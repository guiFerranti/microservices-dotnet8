using AutoMapper;
using GeekShopping.ProductAPI.Config;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly MySQLContext _context;
    private IMapper _mapper;

    public ProductRepository(MySQLContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<ProductVO>> FindAll()
    {
        List<Product> products = await _context.Products.ToListAsync();

        return _mapper.Map<List<ProductVO>>(products);
    }

    public async Task<ProductVO> FindById(long id)
    {
        Product product = await _context.Products.Where(p => p.Id == id)
            .FirstOrDefaultAsync() ?? new Product();
        
        return _mapper.Map<ProductVO>(product);
    }
        
    public async Task<ProductVO> Create(ProductVO vo)
    {

        Product product = _mapper.Map<Product>(vo);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductVO>(product);
    }

    public async Task<ProductVO> Update(ProductVO vo)
    {
        Product product = _mapper.Map<Product> (vo);

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductVO>(product);
    }

    public async Task<bool> Delete(long id)
    {
        try
        {
            Product product = _context.Products.Where(p => p.Id==id).FirstOrDefault() ?? new Product();

            if (product.Id <= 0) return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;

        }
        catch (Exception e)
        {
            return false;
        }
    }


}
