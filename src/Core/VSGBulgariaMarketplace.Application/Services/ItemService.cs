namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemService : BaseService<IItemRepository, Item>, IItemService
    {
        internal const string MARKETPLACE_CACHE_KEY = "marketplace";
        internal const string INVENTORY_CACHE_KEY = "inventory";
        internal const string ITEM_CACHE_KEY_TEMPLATE = "item-{0}";

        public ItemService(IItemRepository itemRepository, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(itemRepository, cacheAdapter, mapper)
        {
        }

        public MarketplaceItemDto[] GetMarketplace()
        {
            MarketplaceItemDto[] itemDtos = base.cacheAdapter.Get<MarketplaceItemDto[]>(MARKETPLACE_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = this.repository.GetMarketplace();
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
                Item[] items = this.repository.GetInventory();
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
                Item item = this.repository.GetByCode(code);
                itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);

                base.cacheAdapter.Set(itemCacheKey, itemDto);
            }

            return itemDto;
        }

        public void Create(ManageItemDto createItemDto)
        {
            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);

            Item item = base.mapper.Map<ManageItemDto, Item>(createItemDto);
            this.repository.Create(item);
        }

        public void Update(int code, ManageItemDto updateItemDto) 
        {
            base.cacheAdapter.Clear();

            Item item = base.mapper.Map<ManageItemDto, Item>(updateItemDto);

            this.repository.Update(code, item);
        }

        public void Delete(int code)
        {
            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ITEM_CACHE_KEY_TEMPLATE, code));

            base.repository.Delete(code);
        }    
    }
}