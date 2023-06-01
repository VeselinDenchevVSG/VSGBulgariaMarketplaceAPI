namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class ItemService : BaseService<IItemRepository, Item>, IItemService
    {
        internal const string MARKETPLACE_CACHE_KEY = "marketplace";
        internal const string INVENTORY_CACHE_KEY = "inventory";
        internal const string ITEM_CACHE_KEY_TEMPLATE = "item-{0}";

        private IOrderRepository orderRepository;
        private ICloudImageService imageService;

        public ItemService(IItemRepository itemRepository, IOrderRepository orderRepository, ICloudImageService imageService, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(itemRepository, cacheAdapter, mapper)
        {
            this.orderRepository = orderRepository;
            this.imageService = imageService;
        }

        public MarketplaceItemDto[] GetMarketplace()
        {
            MarketplaceItemDto[] itemDtos = base.cacheAdapter.Get<MarketplaceItemDto[]>(MARKETPLACE_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetMarketplace();
                itemDtos = base.mapper.Map<Item[], MarketplaceItemDto[]>(items);

                foreach (MarketplaceItemDto marketplaceItem in itemDtos)
                {
                    marketplaceItem.ImageUrl = this.imageService.GetImageUrlByItemCode(marketplaceItem.Code);
                }

                base.cacheAdapter.Set(MARKETPLACE_CACHE_KEY, itemDtos);
            }

            return itemDtos;
        }

        public List<string> GetAllCategories()
        {
            List<string> locationStrings = new List<string>();

            var locations = Enum.GetValues(typeof(Location));

            foreach (Location location in locations)
            {
                string locationString = EnumService.GetEnumDisplayName(location);
                locationStrings.Add(locationString);
            }

            return locationStrings;
        }

        public InventoryItemDto[] GetInventory()
        {
            InventoryItemDto[] itemDtos = base.cacheAdapter.Get<InventoryItemDto[]>(INVENTORY_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetInventory();
                itemDtos = base.mapper.Map<Item[], InventoryItemDto[]>(items);

                foreach (InventoryItemDto inventoryItem in itemDtos)
                {
                    inventoryItem.ImageUrl = this.imageService.GetImageUrlByItemCode(inventoryItem.Code);
                }

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

                if (item is null) throw new NotFoundException($"Item with code {code} doesn't exist!");

                itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);
                itemDto.ImageUrl = this.imageService.GetImageUrlByItemCode(code);

                base.cacheAdapter.Set(itemCacheKey, itemDto);
            }

            return itemDto;
        }

        public async Task CreateAsync(CreateItemDto createItemDto)
        {
            if (createItemDto.QuantityForSale > createItemDto.Quantity)
            {
                throw new ArgumentOutOfRangeException("Quantity for sale should be less or equal to quantity combined!");
            }

            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);

            Item item = base.mapper.Map<CreateItemDto, Item>(createItemDto);

            if (createItemDto.Image is not null)
            {
                item.ImagePublicId = await this.imageService.UploadAsync(createItemDto.Image);
            }

            this.repository.Create(item);
        }

        public async Task UpdateAsync(int code, UpdateItemDto updateItemDto) 
        {
            if (updateItemDto.QuantityForSale > updateItemDto.Quantity)
            {
                throw new ArgumentOutOfRangeException("Quantity for sale should be less or equal than quantity combined!");
            }

            Item item = base.mapper.Map<UpdateItemDto, Item>(updateItemDto);

            string itemImagePublicId = this.GetItemPicturePublicId(code);
            if (itemImagePublicId is null)
            {
                if (updateItemDto.Image is not null)
                {
                    item.ImagePublicId = await this.imageService.UploadAsync(updateItemDto.Image);
                }
            }
            else
            {
                if (updateItemDto.ImageChanges)
                {
                    if (updateItemDto.Image is null)
                    {
                        await this.imageService.DeleteAsync(itemImagePublicId);
                    }
                    else
                    {
                        await this.imageService.UpdateAsync(itemImagePublicId, updateItemDto.Image);
                        item.ImagePublicId = itemImagePublicId.Split("/")[1];
                    }
                }
            }

            try
            {
                this.repository.Update(code, item);
            }
            catch (SqlException se) when (itemImagePublicId is not null)
            {
                await this.imageService.DeleteAsync(itemImagePublicId);
                throw se;
            }

            base.cacheAdapter.Clear();
        }

        public async Task Delete(int code)
        {
            this.orderRepository.DeclineAllPendingOrdersWithDeletedItem(code);

            string itemPicturePublicId = this.GetItemPicturePublicId(code);

            base.repository.DeleteByCode(code);

            if (itemPicturePublicId is not null)
            {
                await this.imageService.DeleteAsync(itemPicturePublicId);
            }

            base.cacheAdapter.Clear();
        }
        
        private string GetItemPicturePublicId(int code) => this.repository.GetItemPicturePublicId(code);
    }
}