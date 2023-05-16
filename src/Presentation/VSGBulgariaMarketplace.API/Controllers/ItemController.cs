namespace VSGBulgariaMarketplace.API.Controllers
{
    using FluentValidation.AspNetCore;

    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;

    using VSGBulgariaMarketplace.Application.Helpers.Validators;

    [Route("api/[controller]")]
    [ApiController]
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
        [Route("create")]
        
        public async Task<IActionResult> CreateAsync([FromForm] ManageItemDto itemDto, IFormFile? imageFile)
        {
            await this.itemService.CreateAsync(itemDto, imageFile);

            return Ok($"Item {itemDto.Name} is successfully created!");
        }

        [HttpPut]
        [Route("update/{code}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int code, [FromForm] ManageItemDto itemDto, IFormFile? imageFile)
        {
            await this.itemService.UpdateAsync(code, itemDto, imageFile);

            return Ok();
        }

        [HttpDelete]
        [Route("delete/{code}")]
        public IActionResult Delete([FromRoute] int code)
        {
            this.itemService.Delete(code);

            return Ok("Item has been successfully deleted!");
        }
    }
}