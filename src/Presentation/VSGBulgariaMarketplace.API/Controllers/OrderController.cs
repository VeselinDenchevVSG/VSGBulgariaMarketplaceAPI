namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;

    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        //[Route("pending-orders")]
        [Route("/pendingorders")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetPendingOrders()
        {
            PendingOrderDto[] orders = this.orderService.GetPendingOrders();

            return Ok(orders);
        }

        [HttpGet]
        //[Route("user-orders")]
        [Route("/myorders")]
        public IActionResult GetUserOrders()
        {
            UserOrderDto[] orders = this.orderService.GetUserOrders();

            return Ok(orders);
        }

        [HttpPost]
        //[Route("create")]
        [Route("marketplace/buy")]
        public IActionResult Create([FromBody] CreateOrderDto orderDto)
        {
            this.orderService.Create(orderDto);

            return Ok($"Order was successfully created!");
        }

        //[HttpPatch]
        [HttpPut]
        //[Route("finish/{id}")]
        [Route("/pendingorders/complete/{code}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Finish([FromRoute] string code)
        {
            this.orderService.Finish(code);

            return Ok($"Order has been finished");
        }

        //[HttpDelete]
        [HttpPut]
        [Route("myorders/deleteorder/{code}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Decline([FromRoute] string code)
        {
            this.orderService.Decline(code);

            return Ok("Order has been declined!");
        }
    }
}