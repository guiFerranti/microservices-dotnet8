using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GeekShopping.ProductAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private IProductRepository _repository;

    public ProductController(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
	public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll ()
    {
        var products = await _repository.FindAll();

        return Ok(products);
    }

    [HttpGet("{id}")]
	[Authorize]
	public async Task<ActionResult<ProductVO>> FindById (long id)
    {
        var product = await _repository.FindById(id);

        if (product.Id <= 0) return NotFound(); 

        return Ok(product);
    }

    [HttpPost]
	[Authorize]
	public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO vo)
    {
        if (vo == null) return BadRequest();

        ProductVO product = await _repository.Create(vo);

        return Ok(product);
    }

    [HttpPut]
	[Authorize]
	public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO vo)
    {
        if (vo == null) return BadRequest();

        ProductVO product = await _repository.Update(vo);

        return Ok(product);
    }

    [HttpDelete("{id}")]
	[Authorize(Roles = Role.Admin)]
	public async Task<ActionResult> Delete(long id)
    {
        bool success = await _repository.Delete(id);
        if (!success) return BadRequest();
        return Ok(true);
    }
        
}
