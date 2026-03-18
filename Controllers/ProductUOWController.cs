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
            var productRepository = _unitOfWork.GetRepository<IProductRepository, Product>();
            var resul = await productRepository.GetAllAsync();

            return Ok(resul);
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetProductsByPaging(
      [FromQuery] decimal minPrice = 0,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string searchTerm = "")
        {
            var result = await _unitOfWork.ProductRepository.GetProductsByPaging(minPrice, pageNumber, pageSize, searchTerm);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            return Ok(new { data = result, pagination = metadata });
        }

        [HttpGet("ProductByName")]
        public async Task<IActionResult> GetByName(string productName)
        {
           var productRepository = _unitOfWork.GetRepository<IProductRepository,Product>();
            var result = await productRepository.GetProductsByName(productName);

            return Ok(result);
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


                var productRepository = _unitOfWork.GetRepository<IProductRepository, Product>();
                var productrestul = await productRepository.AddAsync(productEntity);
               

              
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
