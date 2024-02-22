namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters.Validation;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Enums;

    using static VSGBulgariaMarketplace.API.Constants.ControllerConstant;
    using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private IItemService itemService;

        public ItemController(IItemService itemService)
        {
            this.itemService = itemService;
        }

        [HttpGet]
        [Route(MARKETPLACE_ROUTE)]
        public IActionResult GetMarketplace()
        {
            MarketplaceItemDto[] items = this.itemService.GetMarketplace();

            return Ok(items);
        }


        [HttpGet]
        //[Route(GET_CATEGORIES_ROUTE)]
        [Route(GET_CATEGORIES_ROUTE_SPARTAK)]
        public IActionResult GetCategories()
        {
            List<string> categories = EnumService.GetAll<Category>();

            return Ok(categories);
        }


        [HttpGet]
        [Route(INVENTORY_ROUTE)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult GetInventory()
        {
            InventoryItemDto[] items = this.itemService.GetInventory();

            return Ok(items);
        }

        [HttpGet]
        //[Route(GET_ITEM_BY_ID_ROUTE)]
        [Route(GET_ITEM_BY_ID_ROUTE_SPARTAK)]
        public IActionResult GetById([FromRoute] string id)
        {
            ItemDetailsDto item = this.itemService.GetById(id);

            return Ok(item);
        }

        [HttpPost]
        //[Route(CREATE_ITEM_ASYNC_ROUTE)]
        [Route(CREATE_ITEM_ASYNC_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        [FormatValidationErrorMessagesFilter]
        public async Task<IActionResult> CreateAsync([FromForm] CreateItemDto itemDto, CancellationToken cancellationToken)
        {
            await this.itemService.CreateAsync(itemDto, cancellationToken);

            return Ok(new { Message = string.Format(ITEM_SUCCESSFULLY_CREATED_MESSAGE_TEMPLATE, itemDto.Name) } );
        }

        [HttpPut]
        //[Route(UPDATE_ITEM_ASYNC_ROUTE)]
        [Route(UPDATE_ITEM_ASYNC_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        [FormatValidationErrorMessagesFilter]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromForm] UpdateItemDto itemDto, CancellationToken cancellationToken)
        {
            await this.itemService.UpdateAsync(id, itemDto, cancellationToken);

            return Ok(new { Message = string.Format(ITEM_SUCCESSFULLY_UPDATED_MESSAGE_TEMPLATE, itemDto.Name) } );
        }

        [HttpDelete]
        //[Route(DELETE_ITEM_ASYNC_ROUTE)]
        [Route(DELETE_ITEM_ASYNC_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            await this.itemService.DeleteAsync(id, cancellationToken);

            return Ok(new { Message = ITEM_SUCCESSFULLY_DELETED_MESSAGE } );
        }
    }
}