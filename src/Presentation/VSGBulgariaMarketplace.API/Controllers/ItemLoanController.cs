namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;

    //[Route("api/[controller]")]
    [ApiController]
    public class ItemLoanController : ControllerBase
    {
        private IItemLoanService itemLoanService;

        public ItemLoanController(IItemLoanService itemLoanService)
        {
            this.itemLoanService = itemLoanService;
        }

        [HttpGet]
        // [Route("user-emails-with-lend-items-count")]
        [Route("lentitems")]
        public IActionResult GetUserEmailsWithLendItemsCount()
        {
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCount = this.itemLoanService.GetUserEmailWithLendItemsCount();

            return Ok(emailsWithLendItemsCount);
        }

        [HttpGet]
        //[Route("user-lend-items/{email}")]
        [Route("myloans/{email}/")]
        public IActionResult GetUserLendItems([FromRoute] string email)
        {
            UserLendItemDto[] userLendItems = this.itemLoanService.GetUserLendItems(email);

            return Ok(userLendItems);
        }

        [HttpPost]
        //[Route("lend-items/{ItemId}")]
        [Route("inventory/loan/{itemId}")]
        public IActionResult LendItems([FromRoute] string itemId, [FromForm] LendItemsDto lendItems)
        {
            this.itemLoanService.LendItems(itemId, lendItems);

            return Ok(new { Message = "Item lent successful" });
        }

        [HttpPost]
        //[Route("return-items/{id}")]
        [Route("inventory/return/{id}")]
        public IActionResult Return([FromRoute] string id)
        {
            this.itemLoanService.Return(id);

            return Ok(new { Message = "Returnment successful" });
        }
    }
}
