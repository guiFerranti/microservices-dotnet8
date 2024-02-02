using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController : ControllerBase
{
    private ICartRepository _repository;
    IMapper _mapper;

    public CartController(ICartRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("find-cart/{userId}")]
    public async Task<ActionResult<CartVO>> FindCartById(string userId)
    {
        var cart = await _repository.FindCartByUserId(userId);

        if (cart == null) return NotFound();

        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<ActionResult<CartVO>> AddCart(CartVO cartVo)
    {
        var cart = await _repository.SaveOrUpdateCart(cartVo);

        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<ActionResult<CartVO>> UpdateCart(CartVO cartVo)
    {
        var cart = await _repository.SaveOrUpdateCart(cartVo);

        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<bool>> RemoveCart(long id)
    {
        var status = await _repository.RemoveFromCart(id);

        if (!status) return BadRequest();

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO cartVo)
    {
        var status = await _repository.ApplyCoupon(cartVo.CartHeader.UserId, cartVo.CartHeader.CouponCode);

        if (!status) return NotFound();
        return Ok(status);
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult<CartVO>> AddCart(string userId)
    {
        var status = await _repository.RemoveCoupon(userId);

        if (!status) return NotFound();
        return Ok(status);
    }
}
