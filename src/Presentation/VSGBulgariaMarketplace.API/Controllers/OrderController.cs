namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        [Route("pending-orders")]
        public IActionResult GetPendingOrders()
        {
            PendingOrderDto[] orders = this.orderService.GetPendingOrders();

            return Ok(orders);
        }

        [HttpGet]
        [Route("orders/{userId}")]
        public IActionResult GetUserOrders([FromRoute]string userId)
        {
            UserOrderDto[] orders = this.orderService.GetUserOrders(userId);

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
        public IActionResult Finish([FromRoute] int id)
        {
            this.orderService.Finish(id);

            return Ok($"Order with id = {id} is finished");
        }

        [HttpDelete]
        [Route("decline/{id}")]
        public IActionResult Decline([FromRoute] int id)
        {
            this.orderService.Decline(id);

            return Ok("Order has been declined!");
        }
    }
}