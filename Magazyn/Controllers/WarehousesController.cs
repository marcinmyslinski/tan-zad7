using Magazyn.Models;
using Magazyn.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Magazyn.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehousesController : Controller
    {
        private readonly IDatabaseService _dbService;

        public WarehousesController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("product")]
        public async Task<IActionResult> GetProduct(int IdProd)
        {
            return Ok(await _dbService.GetProduct(IdProd));
        }
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            return Ok(_dbService.GetProducts());
        }

        //[HttpPost]
        //public async Task<IActionResult> SellProduct(int IdProd, int IdWare, int ammount)
        //{

        //    return Ok(await _dbService.Sell(IdProd, IdWare, ammount));
        //}
        [HttpPost]
        public async Task<IActionResult> BuyProduct(int IdProd, int IdWare, int Amount)
        {

            var res = Ok(await _dbService.Buy(IdProd, IdWare, Amount));
            if (res.Value.Equals(1))
            {

                return StatusCode(200);

            }
            else if (res.Value.Equals(0))
            {
                return Content("404 " + (HttpStatusCode)404 + " : Invalid parameter: Provided IdProduct does not exist");
            }
            else if (res.Value.Equals(-1))
            {
                return Content("404 " + (HttpStatusCode)404 + " : Invalid parameter: There is no order to fullfill");
            }
            else if (res.Value.Equals(-2))
            {
                return Content("404 " + (HttpStatusCode)404 + " : Invalid parameter: Provided IdWarehouse does not exist");
            }
            else if (res.Value.Equals(-3))
            {
                return Content("400 " + (HttpStatusCode)404 + " : Filled bad data or data not entered");
            }
            else
            {
                return Content("404 "+(HttpStatusCode)404 + " : Error");
            }
        }
        [HttpPost("addproduct")]
        public async Task<IActionResult> AddProduct(Models.Product p)
        {

            return Ok(await _dbService.Add(p));
        }
 
    }
}
