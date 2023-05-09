namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private IItemRepository itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAll()
        {
            Item[] items = this.itemRepository.GetAll();

            return Ok(items);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            Item item = this.itemRepository.GetById(id);

            if (item is null) return NotFound("Item doesn't exist!");

            return Ok(item);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Create([FromForm] ItemDto itemDto)
        {
            Item item = new Item()
            {
                Name = itemDto.Name,
                PicturePublicId = itemDto.PicturePublicId,
                Price = itemDto.Price,
                QuantityCombined = itemDto.QuantityCombined,
                QuantityForSale = itemDto.QuantityForSale,
                Description = itemDto.Description,
                CreatedAtUtc = DateTime.UtcNow,
                ModifiedAtUtc = DateTime.UtcNow,
                //Orders = new List<Order>(),
            };
            Enum.TryParse(itemDto.Category, out Category category);
            item.Category = category;

            this.itemRepository.Create(item);

            return Ok($"Brand {item.Name} is successfully added!");
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult Update([FromRoute] int id, [FromForm] ItemDto itemDto)
        {
            try
            {
                Item item = new Item()
                {
                    Name = itemDto.Name,
                    PicturePublicId = itemDto.PicturePublicId,
                    Price = itemDto.Price,
                    QuantityCombined = itemDto.QuantityCombined,
                    QuantityForSale = itemDto.QuantityForSale,
                    Description = itemDto.Description,
                    CreatedAtUtc = DateTime.UtcNow,
                    ModifiedAtUtc = DateTime.UtcNow
                };
                Enum.TryParse(itemDto.Category, out Category category);
                item.Category = category;

                this.itemRepository.Update(id, item);

                return Ok(item);
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                this.itemRepository.Delete(id);

                return Ok("Item has been successfully deleted!");

            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }
    }
}
