namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using FluentValidation;

    using Microsoft.AspNetCore.Http;

    using System.ComponentModel.DataAnnotations;

    using VSGBulgariaMarketplace.Application.Helpers.Validators;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Image.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemService : BaseService<IItemRepository, Item>, IItemService
    {
        internal const string MARKETPLACE_CACHE_KEY = "marketplace";
        internal const string INVENTORY_CACHE_KEY = "inventory";
        internal const string ITEM_CACHE_KEY_TEMPLATE = "item-{0}";

        private IImageCloudService imageService;

        public ItemService(IItemRepository itemRepository, IImageCloudService imageService, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(itemRepository, cacheAdapter, mapper)
        {
            this.imageService = imageService;
        }

        public MarketplaceItemDto[] GetMarketplace()
        {
            MarketplaceItemDto[] itemDtos = base.cacheAdapter.Get<MarketplaceItemDto[]>(MARKETPLACE_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetMarketplace();
                itemDtos = base.mapper.Map<MarketplaceItemDto[]>(items);

                base.cacheAdapter.Set(MARKETPLACE_CACHE_KEY, itemDtos);
            }

            return itemDtos;
        }

        public InventoryItemDto[] GetInventory()
        {
            InventoryItemDto[] itemDtos = base.cacheAdapter.Get<InventoryItemDto[]>(INVENTORY_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetInventory();
                itemDtos = base.mapper.Map<InventoryItemDto[]>(items);

                base.cacheAdapter.Set(INVENTORY_CACHE_KEY, itemDtos);
            }

            return itemDtos;
        }

        public ItemDetailsDto GetByCode(int code)
        {
            string itemCacheKey = string.Format(ITEM_CACHE_KEY_TEMPLATE, code);

            ItemDetailsDto itemDto = base.cacheAdapter.Get<ItemDetailsDto>(itemCacheKey);
            if (itemDto is null)
            {
                Item item = base.repository.GetByCode(code);
                itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);

                base.cacheAdapter.Set(itemCacheKey, itemDto);
            }

            return itemDto;
        }

        public async Task CreateAsync(ManageItemDto createItemDto, IFormFile? imageFile)
        {
            if (imageFile is not null)
            {
                ImageFileValidator imageFileValidator = new ImageFileValidator();
                imageFileValidator.ValidateAndThrow(imageFile);
            }

            if (createItemDto.QuantityForSale > createItemDto.QuantityCombined)
            {
                throw new ArgumentOutOfRangeException("Quantity for sale should be less or equal than quantity combined!");
            }

            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);

            Item item = base.mapper.Map<ManageItemDto, Item>(createItemDto);

            if (imageFile is not null)
            {
                item.PicturePublicId = await this.imageService.UploadAsync(imageFile);
            }

            this.repository.Create(item);

        }

        public async Task UpdateAsync(int code, ManageItemDto updateItemDto, IFormFile? imageFile) 
        {
            if (imageFile is not null)
            {
                ImageFileValidator imageFileValidator = new ImageFileValidator();
                imageFileValidator.ValidateAndThrow(imageFile);
            }

            Item item = base.mapper.Map<ManageItemDto, Item>(updateItemDto);

            string itemPicturePublicId = this.GetItemPicturePublicId(code);
            if (itemPicturePublicId is null)
            {
                if (imageFile is not null)
                {
                    item.PicturePublicId = await this.imageService.UploadAsync(imageFile);
                }
            }
            else
            {
                if (imageFile is null)
                {
                    await this.imageService.DeleteAsync(itemPicturePublicId);
                }
                else
                {
                    await this.imageService.UpdateAsync(itemPicturePublicId, imageFile);
                }
            }

            this.repository.Update(code, item);


            base.cacheAdapter.Clear();
        }


        public void Delete(int code)
        {
            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ITEM_CACHE_KEY_TEMPLATE, code));

            base.repository.Delete(code);

            string itemPicturePublicId = this.GetItemPicturePublicId(code);
            this.imageService.DeleteAsync(itemPicturePublicId);
        }
        
        private string GetItemPicturePublicId(int code) => this.repository.GetItemPicturePublicId(code);
    }
}