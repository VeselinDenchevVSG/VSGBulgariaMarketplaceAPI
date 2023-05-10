namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository orderRepository;
        private IOrderService orderService;

        public OrderController(IOrderRepository orderRepository, IOrderService orderService)
        {
            this.orderRepository = orderRepository;
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

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            Order order = this.orderRepository.GetById(id);

            if (order is null) return NotFound("Order doesn't exist!");

            return Ok(order);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Create([FromForm] OrderDto orderDto)
        {
            Order order = new Order()
            {
                ItemId = orderDto.ItemId,
                Quantity = orderDto.Quantity,
                Email = orderDto.Email,
            };
            Enum.TryParse(orderDto.Status, out OrderStatus status);
            order.Status = status;

            this.orderRepository.Create(order);

            return Ok($"Order with id {order.Id} is successfully added!");
        }

        [HttpPatch]
        [Route("finish/{id}")]
        public IActionResult Finish([FromRoute] int id)
        {
            try
            {
                this.orderService.Finish(id);

                return Ok($"Order with id = {id} is finished");
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }

        [HttpDelete]
        [Route("decline/{id}")]
        public IActionResult Decline([FromRoute] int id)
        {
            try
            {
                this.orderService.Decline(id);

                return Ok("Order has been declined!");

            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }
    }
}