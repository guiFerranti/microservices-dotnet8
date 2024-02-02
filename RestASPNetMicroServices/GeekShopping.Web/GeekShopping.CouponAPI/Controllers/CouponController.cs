using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{
    private ICouponRepository _repository;

    public CouponController(ICouponRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{couponCode}")]
    [Authorize]
    public async Task<ActionResult<CouponVO>> GetCouponByCouponCode(string couponCode)
    {
        CouponVO coupon = await _repository.GetCouponByCouponCode(couponCode);

        if (coupon == null) return NotFound();
        return Ok(coupon);

    }

}
