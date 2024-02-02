using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace GeekShopping.CouponAPI.Model.Context;

public class MySQLContext : DbContext
{
    public MySQLContext() {}
    
    
    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 1,
            CouponCode = "COUPON_2024_10",
            DiscountAmount = 10
        });
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 2,
            CouponCode = "COUPON_2024_15",
            DiscountAmount = 15
        });
    }

}
