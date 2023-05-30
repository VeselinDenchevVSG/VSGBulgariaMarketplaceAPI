namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;

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
        [Route("marketplace")]
        public IActionResult GetMarketplace()
        {
            MarketplaceItemDto[] items = this.itemService.GetMarketplace();

            return Ok(items);
        }
        [HttpGet]
        [Route("inventory")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetInventory()
        {
            InventoryItemDto[] items = this.itemService.GetInventory();

            return Ok(items);
        }

        [HttpGet]
        [Route("{code}")]
        public IActionResult GetByCode([FromRoute] int code)
        {
            ItemDetailsDto item = this.itemService.GetByCode(code);

            if (item is null) return NotFound("Item doesn't exist!");

            return Ok(item);
        }

        [HttpPost]
        [Route("/inventory/addItem")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] ManageItemDto itemDto)
        {
            await this.itemService.CreateAsync(itemDto);

            return Ok($"Item {itemDto.Name} is successfully created!");
        }

        [HttpPut]
        [Route("/inventory/modify/{code}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int code, [FromForm] ManageItemDto itemDto)
        {
            await this.itemService.UpdateAsync(code, itemDto);

            return Ok();
        }

        [HttpDelete]
        [Route("/deleteItem/{code}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int code)
        {
            await this.itemService.Delete(code);

            return Ok("Item has been successfully deleted!");
        }
    }
}