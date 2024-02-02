﻿using AutoMapper;
using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model;
using GeekShopping.CouponAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly MySQLContext _context;
    private IMapper _mapper;

    public CouponRepository(MySQLContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CouponVO> GetCouponByCouponCode(string couponCode)
    {
        Coupon coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

        return _mapper.Map<CouponVO>(coupon);       
    }
}
