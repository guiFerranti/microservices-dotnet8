﻿using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<IActionResult> ProductIndex()
    {
        var products = await _productService.FindAllProducts("");

        return View(products);
    }

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }

	[Authorize]
	[HttpPost]
    public async Task<IActionResult> ProductCreate(ProductModel productModel)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.CreateProduct(productModel, token);

            if (response != null) return RedirectToAction(nameof(ProductIndex));
        }
        return View(productModel);
    }

    public async Task<IActionResult> ProductUpdate(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await _productService.FindProductById(id, token);

        if (model != null) return View(model);
        else return NotFound();
        
    }

	[Authorize]
	[HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductModel productModel)
    {
        if (ModelState.IsValid)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.UpdateProduct(productModel, token);

            if (response != null) return RedirectToAction(nameof(ProductIndex));
        }

        return View(productModel);
    }

	[Authorize]
	public async Task<IActionResult> ProductDelete(int id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var model = await _productService.FindProductById(id, token);

        if (model != null) return View(model);
        else return NotFound();
        
    }

	[HttpPost]
	[Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> ProductDelete(ProductModel productModel)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.DeleteProductById(productModel.Id, token);
        
        if (response) return RedirectToAction(nameof(ProductIndex));
        else return View(productModel);
    }
    
    
}
