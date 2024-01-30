using AutoMapper;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class CartRepository : ICartRepository
{
    private readonly MySQLContext _context;
    private IMapper _mapper;

    public CartRepository(MySQLContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId  == userId);

        if (cartHeader != null) 
        {
            _context.CartDetails.RemoveRange
                (_context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));
            _context.CartHeaders.Remove(cartHeader);

            await _context.SaveChangesAsync();

            return true;
        }
        return false;
    }

    public async Task<Data.ValueObjects.CartVO> FindCartByUserId(string userId)
    {
        Cart cart = new()
        {
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId),
        };

        cart.CartDetails = _context.CartDetails.Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(c => c.Product);

        return _mapper.Map<Data.ValueObjects.CartVO>(cart);
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        try
        {
            CartDetail cartDetail = await _context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetailsId);

            int total = _context.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();


            _context.CartDetails.Remove(cartDetail);

            if (total == 1)
            {
                var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);

                _context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception) 
        {
            return false;
        }

    }

    public async Task<Data.ValueObjects.CartVO> SaveOrUpdateCart(Data.ValueObjects.CartVO cartVo)
    {
        Cart cart = _mapper.Map<Cart>(cartVo);

        // ver se o produto ja ta salvo na db

        var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == cartVo.CartDetails.FirstOrDefault().ProductId);

        if (product == null)
        {
            _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }

        // procurar o CartHeader

        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader == null)
        {
            // criar o CartHeader e o CartDetails
            _context.CartHeaders.Add(cart.CartHeader);
            await _context.SaveChangesAsync();
            
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.First().Product = null;
            _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());

            await _context.SaveChangesAsync();
        }
        else
        {
            var cartDetail = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId && p.CartHeaderId == cartHeader.Id);
            if (cartDetail == null)
            {
                // Criar CartDetails
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.First().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();

            }
            else
            {
                // Update Product count and CartDetails
                cart.CartDetails.First().Product = null;
                cart.CartDetails.First().Count  += cartDetail.Count;
                cart.CartDetails.First().Id = cartDetail.Id;
                cart.CartDetails.First().CartHeaderId = cartDetail.CartHeaderId;

                _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());

                await _context.SaveChangesAsync();
            }
        }

        return _mapper.Map<Data.ValueObjects.CartVO>(cart);
    }
}
