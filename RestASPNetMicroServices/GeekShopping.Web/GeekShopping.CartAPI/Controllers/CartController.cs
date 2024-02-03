using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeekShopping.CartAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController : ControllerBase
{
    private ICartRepository _cartRepository;
    private ICouponRepository _couponRepository;
    private IRabbitMQMessageSender _rabbitMQSender;

    public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IRabbitMQMessageSender rabbitMQSender)
    {
        _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
        _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        _rabbitMQSender = rabbitMQSender ?? throw new ArgumentNullException(nameof(rabbitMQSender));
    }

    [HttpGet("find-cart/{userId}")]
    public async Task<ActionResult<CartVO>> FindCartById(string userId)
    {
        var cart = await _cartRepository.FindCartByUserId(userId);

        if (cart == null) return NotFound();

        return Ok(cart);
    }

    [HttpPost("add-cart")]
    public async Task<ActionResult<CartVO>> AddCart(CartVO cartVo)
    {
        var cart = await _cartRepository.SaveOrUpdateCart(cartVo);

        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPut("update-cart")]
    public async Task<ActionResult<CartVO>> UpdateCart(CartVO cartVo)
    {
        var cart = await _cartRepository.SaveOrUpdateCart(cartVo);

        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<bool>> RemoveCart(long id)
    {
        var status = await _cartRepository.RemoveFromCart(id);

        if (!status) return BadRequest();

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO cartVo)
    {
        var status = await _cartRepository.ApplyCoupon(cartVo.CartHeader.UserId, cartVo.CartHeader.CouponCode);

        if (!status) return NotFound();
        return Ok(status);
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult<CartVO>> AddCart(string userId)
    {
        var status = await _cartRepository.RemoveCoupon(userId);

        if (!status) return NotFound();
        return Ok(status);
    }
    
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO vo)
    {
        string token = await HttpContext.GetTokenAsync("access_token");

        if (vo?.UserId == null) return BadRequest();

        var cart = await _cartRepository.FindCartByUserId(vo.UserId);

        if (cart == null) return NotFound();
        if (!string.IsNullOrEmpty(vo.CouponCode))
        {
            CouponVO coupon = await _couponRepository.GetCoupon(vo.CouponCode, token);
            if (coupon.DiscountAmount != vo.DiscountAmount)
            {
                return StatusCode(412);
            }
        }


        vo.CartDetails = cart.CartDetails;
        vo.DateTime  = DateTime.UtcNow;

        // RabbitMQ

        _rabbitMQSender.SendMessage(vo, "checkoutqueue");

        await _cartRepository.ClearCart(vo.UserId);

        return Ok(vo);
    }
}
