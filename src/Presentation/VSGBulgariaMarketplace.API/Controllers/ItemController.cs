namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;

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
        [Route("add")]
        public IActionResult Create([FromForm] ManageItemDto itemDto)
        {
            try
            {
                this.itemService.Create(itemDto);

                return Ok($"Brand {itemDto.Name} is successfully added!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut]
        [Route("update/{code}")]
        public IActionResult Update([FromRoute] int code, [FromForm] ManageItemDto itemDto)
        {
            try
            {
                this.itemService.Update(code, itemDto);

                return Ok();
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{code}")]
        public IActionResult Delete([FromRoute] int code)
        {
            try
            {
                this.itemService.Delete(code);

                return Ok("Item has been successfully deleted!");

            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }
    }
}
