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

        public MarketplaceItemDto[] GetAllMarketplace()
        {
            Item[] items = this.repository.GetAll();
            MarketplaceItemDto[] itemDtos = base.mapper.Map<MarketplaceItemDto[]>(items);

            return itemDtos;
        }

        public InventoryItemDto[] GetAllInventory()
        {
            Item[] items = this.repository.GetAll();
            InventoryItemDto[] itemDtos = base.mapper.Map<InventoryItemDto[]>(items);

            return itemDtos;
        }

        public ItemDetailsDto GetById(int id)
        {
            Item item = this.repository.GetById(id);
            ItemDetailsDto itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);

            return itemDto;
        }

        public void Create(ManageItemDto createItemDto)
        {
            Item item = base.mapper.Map<ManageItemDto, Item>(createItemDto);
            this.repository.Create(item);
        }

        public void Update(int id, ManageItemDto updateItemDto) 
        {
            Item item = base.mapper.Map<ManageItemDto, Item>(updateItemDto);
            this.repository.Update(id, item);
        }

        public void Delete(int id) => base.repository.Delete(id);
    }
}