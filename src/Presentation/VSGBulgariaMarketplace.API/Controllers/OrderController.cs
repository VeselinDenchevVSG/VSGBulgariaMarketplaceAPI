namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters.Validation;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;

    using static VSGBulgariaMarketplace.API.Constants.ControllerConstant;
    using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

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
        //[Route(GET_PENDING_ORDERS_ROUTE)]
        [Route(GET_PENDING_ORDERS_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult GetPendingOrders()
        {
            PendingOrderDto[] orders = this.orderService.GetPendingOrders();

            return Ok(orders);
        }

        [HttpGet]
        //[Route(GET_USER_ORDERS_ROUTE)]
        [Route(GET_USER_ORDERS_ROUTE_SPARTAK)]
        public IActionResult GetUserOrders()
        {
            UserOrderDto[] orders = this.orderService.GetUserOrders();

            return Ok(orders);
        }

        [HttpPost]
        //[Route(CREATE_ORDER_ROUTE)]
        [Route(CREATE_ORDER_ROUTE_SPARTAK)]
        [FormatValidationErrorMessagesFilter]
        public IActionResult Create([FromBody] CreateOrderDto orderDto)
        {
            this.orderService.Create(orderDto);

            return Ok(new { Message = ORDER_CREATED_SUCCESSFULLY_MESSAGE });
        }

        //[HttpPatch]
        [HttpPut]
        //[Route(FINISH_ORDER_ROUTE)]
        [Route(FINISH_ORDER_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult Finish([FromRoute] string code)
        {
            this.orderService.Finish(code);

            return Ok(new { Message = ORDER_FINISHED_SUCCESSFULLY_MESSAGE });
        }

        //[HttpDelete]
        [HttpPut]
        //[Route(ontrollerConstant.DECLINE_ORDER_ROUTE)]
        [Route(DECLINE_ORDER_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult Decline([FromRoute] string code)
        {
            this.orderService.Decline(code);

            return Ok(new { Message = ORDER_DECLINED_SUCCESSFULLY_MESSAGE });
        }
    }
}