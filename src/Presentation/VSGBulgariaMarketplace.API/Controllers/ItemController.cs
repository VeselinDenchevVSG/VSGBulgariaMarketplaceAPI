﻿namespace VSGBulgariaMarketplace.API.Controllers
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
        private IItemService itemService;

        public ItemController(IItemRepository itemRepository)
        {
            this.itemService = itemService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAll()
        {
            GridItemDto[] items = this.itemService.GetAll();

            return Ok(items);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            ItemDetailsDto item = this.itemService.GetById(id);

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
        [Route("delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                this.itemService.Delete(id);

                return Ok("Item has been successfully deleted!");

            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
        }
    }
}
