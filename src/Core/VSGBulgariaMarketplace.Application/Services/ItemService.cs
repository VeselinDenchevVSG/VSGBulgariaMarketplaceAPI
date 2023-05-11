namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;
    using Microsoft.Extensions.Caching.Memory;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemService : BaseService<IItemRepository, Item>, IItemService
    {
        public ItemService(IItemRepository itemRepository, IMemoryCache memoryCache, IMapper mapper)
            : base(itemRepository, memoryCache, mapper)
        {
        }

        public MarketplaceItemDto[] GetMarketplace()
        {
            Item[] items = this.repository.GetMarketplace();
            MarketplaceItemDto[] itemDtos = base.mapper.Map<MarketplaceItemDto[]>(items);

            return itemDtos;
        }

        public InventoryItemDto[] GetInventory()
        {
            Item[] items = this.repository.GetInventory();
            InventoryItemDto[] itemDtos = base.mapper.Map<InventoryItemDto[]>(items);

            return itemDtos;
        }

        public ItemDetailsDto GetByCode(int code)
        {
            Item item = this.repository.GetById(code);
            ItemDetailsDto itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);

            return itemDto;
        }

        public void Create(ManageItemDto createItemDto)
        {
            Item item = base.mapper.Map<ManageItemDto, Item>(createItemDto);
            this.repository.Create(item);
        }

        public void Update(int code, ManageItemDto updateItemDto) 
        {
            Item item = base.mapper.Map<ManageItemDto, Item>(updateItemDto);
            this.repository.Update(code, item);
        }

        public void Delete(int id) => base.repository.Delete(id);
    }
}