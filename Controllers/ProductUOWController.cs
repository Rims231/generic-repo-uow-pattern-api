using generic_repo_uow_pattern_api.Entity;
using generic_repo_uow_pattern_api.Repository;
using generic_repo_uow_pattern_api.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace generic_repo_uow_pattern_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductUOWController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductUOWController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
          var resul = await _unitOfWork.GetRepository<Product>().GetAllAsync();
            
            return Ok(resul);
        }




        [HttpGet("ProductByName")]
        public async Task<IActionResult> GetByName(string productName)
        {
            var product = await _unitOfWork.ProductRepository.GetProductsByName(productName);

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductRequest product)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var productEntity = new Product
                {
                    ProductName = product.Name,
                    Price = product.Price
                };

                await _unitOfWork
                    .GetRepository<Product>()
                    .AddAsync(productEntity);

              
                var orderEntity = new Order
                {
                    Product = productEntity,
                    OrderDate = DateTime.UtcNow
                };

                await _unitOfWork
                    .GetRepository<Order>()
                    .AddAsync(orderEntity);

               
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return StatusCode(201, new
                {
                    ProductId = productEntity.Id,
                    OrderId = orderEntity.Id
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }
    }
}
