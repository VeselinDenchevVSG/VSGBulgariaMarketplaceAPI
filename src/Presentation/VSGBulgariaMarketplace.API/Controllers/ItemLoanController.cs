﻿namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters.ValidateEmail;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;

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
        // [Route("user-emails-with-lend-items-count")]
        [Route("lentitems")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUserEmailsWithLendItemsCount()
        {
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCount = this.itemLoanService.GetUserEmailsWithLendItemsCount();

            return Ok(emailsWithLendItemsCount);
        }

        [HttpGet]
        //[Route("user-lend-items/{email}")]
        [Route("myloans/{email}/")]
        [ValidateEmailFilter]
        public IActionResult GetUserLendItems([FromRoute] string email)
        {
            UserLendItemDto[] userLendItems = this.itemLoanService.GetUserLendItems(email);

            return Ok(userLendItems);
        }

        [HttpPost]
        //[Route("lend-items/{ItemId}")]
        [Route("inventory/loan/{itemId}")]
        [Authorize(Policy = "Admin")]
        //public IActionResult LendItems([FromRoute] string itemId, [FromForm] LendItemsDto lendItems)
        public IActionResult LendItems([FromRoute] string itemId, [FromBody] LendItemsDto input)
        {
            this.itemLoanService.LendItems(itemId, input);

            return Ok(new { Message = "Item lent successful" });
        }

        [HttpPut]
        //[Route("return-items/{id}")]
        [Route("lentitems/return/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Return([FromRoute] string id)
        {
            this.itemLoanService.Return(id);

            return Ok(new { Message = "Returnment successful" });
        }
    }
}