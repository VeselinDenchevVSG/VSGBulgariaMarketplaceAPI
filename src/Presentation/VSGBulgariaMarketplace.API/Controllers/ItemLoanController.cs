namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters.ValidateEmail;
    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters.Validation;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;

    using static VSGBulgariaMarketplace.API.Constants.ControllerConstant;
    using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemLoanController : ControllerBase
    {
        private IItemLoanService itemLoanService;

        public ItemLoanController(IItemLoanService itemLoanService)
        {
            this.itemLoanService = itemLoanService;
        }

        [HttpGet]
        // [Route(GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_ROUTE)]
        [Route(GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult GetUserEmailsWithLendItemsCount()
        {
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCount = this.itemLoanService.GetUserEmailsWithLendItemsCount();

            return Ok(emailsWithLendItemsCount);
        }

        [HttpGet]
        //[Route(GET_USER_LENT_ITEMS_ROUTE)]
        [Route(GET_USER_LENT_ITEMS_ROUTE_SPARTAK)]
        [ValidateEmailFilter]
        public IActionResult GetUserLentItems([FromRoute] string email)
        {
            UserLendItemDto[] userLendItems = this.itemLoanService.GetUserLendItems(email);

            return Ok(userLendItems);
        }

        [HttpPost]
        //[Route(LEND_ITEMS_ROUTE)]
        [Route(LEND_ITEMS_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        [FormatValidationErrorMessagesFilter]
        //public IActionResult LendItems([FromRoute] string itemId, [FromForm] LendItemsDto lendItems)
        public IActionResult LendItems([FromRoute] string itemId, [FromBody] LendItemsDto input)
        {
            this.itemLoanService.LendItems(itemId, input);

            return Ok(new { Message = ITEMS_LENT_SUCCESSFULLY_MESSAGE });
        }

        [HttpPut]
        //[Route(RETURN_LENT_ITEMS_ROUTE)]
        [Route(RETURN_LENT_ITEMS_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult Return([FromRoute] string id)
        {
            this.itemLoanService.Return(id);

            return Ok(new { Message = ITEMS_RETURNED_SUCCESSFULLY_MESSAGE });
        }
    }
}