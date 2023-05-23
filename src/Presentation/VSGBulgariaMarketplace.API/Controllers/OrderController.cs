namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;

    [Route("api/[controller]")]
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
        [Route("pending-orders")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetPendingOrders()
        {
            PendingOrderDto[] orders = this.orderService.GetPendingOrders();

            return Ok(orders);
        }

        [HttpGet]
        [Route("user-orders")]
        public IActionResult GetUserOrders()
        {
            UserOrderDto[] orders = this.orderService.GetUserOrders();

            return Ok(orders);
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromForm] CreateOrderDto orderDto)
        {
            this.orderService.Create(orderDto);

            return Ok($"Order was successfully created!");
        }

        [HttpPatch]
        [Route("finish/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Finish([FromRoute] int id)
        {
            this.orderService.Finish(id);

            return Ok($"Order with id = {id} is finished");
        }

        [HttpDelete]
        [Route("decline/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Decline([FromRoute] int id)
        {
            this.orderService.Decline(id);

            return Ok("Order has been declined!");
        }
    }
}