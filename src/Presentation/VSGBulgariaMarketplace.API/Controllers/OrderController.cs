﻿namespace VSGBulgariaMarketplace.API.Controllers
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
            try
            {
                this.orderService.Create(orderDto);

                return Ok($"Order was successfully created!");
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }


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