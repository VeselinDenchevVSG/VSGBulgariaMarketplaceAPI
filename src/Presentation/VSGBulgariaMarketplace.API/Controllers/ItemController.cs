namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Enums;

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
        //[Route("categories")]
        [Route("/getcategories")]
        public IActionResult GetCategories()
        {
            List<string> categories = EnumService.GetAll<Category>();

            return Ok(categories);
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
        //[Route("{code}")]
        [Route("marketplace/{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            ItemDetailsDto item = this.itemService.GetById(id);

            return Ok(item);
        }

        [HttpPost]
        //[Route("create")]
        [Route("~/inventory/addItem")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateItemDto itemDto)
        {
            await this.itemService.CreateAsync(itemDto);

            return Ok(new { Message = $"Item {itemDto.Name} is successfully created!" } );
        }

        [HttpPut]
        //[Route("update/{code}")]
        [Route("~/inventory/modify/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromForm] UpdateItemDto itemDto)
        {
            await this.itemService.UpdateAsync(id, itemDto);

            return Ok(new { Message = $"Item {itemDto.Name} has been successfully updated!" } );
        }

        [HttpDelete]
        //[Route("delete/{code}")]
        [Route("/deleteItem/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await this.itemService.Delete(id);

            return Ok(new { Message = "Item has been successfully deleted!" } );
        }
    }
}