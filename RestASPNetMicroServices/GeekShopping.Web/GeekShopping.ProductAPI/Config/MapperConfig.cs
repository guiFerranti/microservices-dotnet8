using AutoMapper;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using System.Net.NetworkInformation;

namespace GeekShopping.ProductAPI.Config;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductVO, Product>();
            config.CreateMap<Product, ProductVO>();
        });

        return mappingConfig;
    }
}
