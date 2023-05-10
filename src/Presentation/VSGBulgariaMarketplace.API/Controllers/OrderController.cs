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

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAll()
        {
            Order[] orders = this.orderRepository.GetAll();

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

        [HttpPut]
        [Route("finish/{id}")]
        public IActionResult Finish([FromRoute] int id)
        {
            try
            {
                this.orderRepository.Finish(id);

                return Ok($"Order with id = {id} is finished");
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                this.orderRepository.Delete(id);

                return Ok("Order has been successfully deleted!");

            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }
    }
}