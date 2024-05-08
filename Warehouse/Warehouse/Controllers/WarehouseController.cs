using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YourNamespace;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly ILogger<WarehouseController> _logger;
        private readonly IDatabaseHelper _databaseHelper;

        public WarehouseController(ILogger<WarehouseController> logger, IDatabaseHelper databaseHelper)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] WarehouseRequestModel request)
        {
            try
            {

                var productExists = await _databaseHelper.CheckIfProductExists(request.IdProduct);
                if (!productExists)
                    return BadRequest("Product with the specified Id does not exist.");
                
                var warehouseExists = await _databaseHelper.CheckIfWarehouseExists(request.IdWarehouse);
                if (!warehouseExists)
                    return BadRequest("Warehouse with the specified Id does not exist.");
                
                if (request.Amount <= 0)
                    return BadRequest("Amount must be greater than 0.");

                var orderExists = await _databaseHelper.CheckIfOrderExists(request.IdProduct, request.Amount, request.CreatedAt);
                if (!orderExists)
                    return BadRequest("Order for the specified product with the specified amount and creation date does not exist.");


                var orderFulfilled = await _databaseHelper.CheckIfOrderFulfilled();
                if (orderFulfilled)
                    return BadRequest("Order has already been fulfilled.");


                await _databaseHelper.UpdateOrderFulfilledDate();

       
                var insertedId = await _databaseHelper.InsertIntoProductWarehouse(request.IdProduct, request.IdWarehouse, request.Amount, request.CreatedAt);
                if (insertedId == -1)
                    throw new Exception("Failed to insert record into Product_Warehouse table.");

       
                return Ok(insertedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product to warehouse.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request.");
            }
        }
    }

    public class WarehouseRequestModel
    {
        public int IdProduct { get; set; }
        public int IdWarehouse { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
