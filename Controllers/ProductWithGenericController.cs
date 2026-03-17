using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using generic_repo_uow_pattern_api.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace generic_repo_uow_pattern_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductWithGenericController : ControllerBase
    {
        private readonly IRepository<Product> _productrepository;

        public ProductWithGenericController(IRepository<Product> productrepository)
        {
            _productrepository = productrepository;
        }


        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var products = await _productrepository.GetAllAsync();
            return Ok(products);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productrepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }


        [HttpPost]

        public async Task<IActionResult> Post([FromBody] ProductRequest product)
        {
            var productEntity = new Product
            {
                ProductName = product.Name,
                Price = product.Price
            };


            var createdProduct = await _productrepository.AddAsync(productEntity);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Put(int id, [FromBody] ProductRequest product)
        {
            var productEntity = await _productrepository.GetByIdAsync(id);
            if (productEntity == null)
            {
                return NotFound();
            }
            productEntity.ProductName = product.Name;
            productEntity.Price = product.Price;
            await _productrepository.UpdateAsync(productEntity);
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var productEntity = await _productrepository.GetByIdAsync(id);
            if (productEntity == null)
            {
                return NotFound();
            }
            await _productrepository.DeleteAsync(id);
            return NoContent();
        }
    }


}
