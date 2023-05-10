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
            this.repository = itemRepository;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
        }

        public GridItemDto[] GetAll()
        {
            Item[] items = this.repository.GetAll();
            GridItemDto[] itemDtos = this.mapper.Map<GridItemDto[]>(items);

            return itemDtos;
        }

        public ItemDetailsDto GetById(int id)
        {
            Item item = this.repository.GetById(id);
            ItemDetailsDto itemDto = this.mapper.Map<Item, ItemDetailsDto>(item);

            return itemDto;
        }

        public void Create(ManageItemDto createItemDto)
        {
            Item item = this.mapper.Map<ManageItemDto, Item>(createItemDto);
            this.repository.Create(item);
        }

        public void Update(int id, ManageItemDto updateItemDto) 
        {
            Item item = this.mapper.Map<ManageItemDto, Item>(updateItemDto);
            this.repository.Update(id, item);
        }
    }
}
