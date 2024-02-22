namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    using static VSGBulgariaMarketplace.Application.Constants.ServiceConstant;

    public class ItemService : BaseService<IItemRepository, Item>, IItemService
    {
        private IOrderRepository orderRepository;
        private IItemLoanRepository itemLoanRepository;

        private ICloudImageService imageService;

        public ItemService(IItemRepository itemRepository, IOrderRepository orderRepository, IItemLoanRepository itemLoanRepository,  ICloudImageService imageService, 
                            IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(itemRepository, cacheAdapter, mapper)
        {
            this.orderRepository = orderRepository;
            this.itemLoanRepository = itemLoanRepository;
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
                    marketplaceItem.ImageUrl = this.imageService.GetImageUrlByItemId(marketplaceItem.Id);
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
                    inventoryItem.ImageUrl = this.imageService.GetImageUrlByItemId(inventoryItem.Id);
                }

                base.cacheAdapter.Set(INVENTORY_CACHE_KEY, itemDtos);
            }

            return itemDtos;
        }

        public ItemDetailsDto GetById(string id)
        {
            string itemCacheKey = string.Format(ITEM_CACHE_KEY_TEMPLATE, id);

            ItemDetailsDto itemDto = base.cacheAdapter.Get<ItemDetailsDto>(itemCacheKey);
            if (itemDto is null)
            {
                Item item = base.repository.GetById(id);

                if (item is null) throw new NotFoundException(SUCH_ITEM_DOES_NOT_EXIST_ERROR_MESSAGE);

                itemDto = base.mapper.Map<Item, ItemDetailsDto>(item);
                itemDto.ImageUrl = this.imageService.GetImageUrlByItemId(id);

                base.cacheAdapter.Set(itemCacheKey, itemDto);
            }

            return itemDto;
        }

        public async Task CreateAsync(CreateItemDto createItemDto, CancellationToken cancellationToken)
        {
            Item item = base.mapper.Map<CreateItemDto, Item>(createItemDto);

            if (createItemDto.Image is not null)
            {
                item.ImagePublicId = await this.imageService.UploadAsync(createItemDto.Image, cancellationToken);
            }

            try
            {
                await this.repository.CreateAsync(item, cancellationToken);
            }
            catch (Exception) when (item.ImagePublicId is not null)
            {
                await this.imageService.DeleteAsync(item.ImagePublicId);

                throw;
            }

            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);
        }

        public async Task UpdateAsync(string id, UpdateItemDto updateItemDto, CancellationToken cancellationToken) 
        {
            short pendingOrderdTotalItemQuantity = await this.orderRepository.GetPendingOrdersTotalItemQuantityByItemIdAsync(id, cancellationToken);
            if (updateItemDto.QuantityForSale <= pendingOrderdTotalItemQuantity)
            {
                throw new ArgumentException(NOT_ENOUGH_QUANTITY_FOR_SALE_IN_ORDER_TO_COMPLETE_PENDING_ORDERS_WITH_THIS_ITEM_ERROR_MESSAGE);
            }

            short itemLoansTotalItemQuantity = await this.itemLoanRepository.GetItemLoansTotalQuantityForItemAsync(id, cancellationToken);
            if (updateItemDto.Quantity <= itemLoansTotalItemQuantity)
            {
                throw new ArgumentException(QUANTITY_COMBINED_MUST_NOT_BE_LOWER_THAN_THE_ACTIVE_LOANS_ITEM_QUANTITY_ERROR_MESSAGE);
            }

            Item item = base.mapper.Map<UpdateItemDto, Item>(updateItemDto);

            bool imageIsDeleted = false;

            string itemImagePublicId = await this.repository.GetItemImagePublicIdAsync(id, cancellationToken);
            if (itemImagePublicId is null)
            {
                if (updateItemDto.Image is not null)
                {
                    item.ImagePublicId = await this.imageService.UploadAsync(updateItemDto.Image, cancellationToken);
                }
            }
            else
            {
                if (updateItemDto.ImageChanges)
                {
                    if (updateItemDto.Image is null)
                    {
                        string deletionResult = await this.imageService.DeleteAsync(itemImagePublicId);
                        imageIsDeleted = deletionResult == CLOUDINARY_DELETION_RESULT_OK;
                    }
                    else
                    {
                        await this.imageService.UpdateAsync(itemImagePublicId, updateItemDto.Image, cancellationToken);
                        item.ImagePublicId = itemImagePublicId.Split("/")[1];
                    }
                }
                else
                {
                    item.ImagePublicId = itemImagePublicId.Split("/")[1];
                }
            }

            try
            {
                this.repository.Update(id, item);
            }
            catch (SqlException) when (itemImagePublicId is not null && !imageIsDeleted)
            {
                await this.imageService.DeleteAsync(itemImagePublicId);

                throw;
            }

            base.cacheAdapter.Remove(MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ITEM_CACHE_KEY_TEMPLATE, id));
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            bool isLoanWithItem = await this.itemLoanRepository.IsLoanWithItemAsync(id, cancellationToken);
            if (!isLoanWithItem)
            {
                string itemPicturePublicId = await this.repository.GetItemImagePublicIdAsync(id, cancellationToken);

                await this.orderRepository.DeclineAllPendingOrdersWithDeletedItemAsync(id, cancellationToken);

                base.repository.Delete(id);

                if (itemPicturePublicId is not null)
                {
                    await this.imageService.DeleteAsync(itemPicturePublicId);
                }

                base.cacheAdapter.Clear();
            }
            else throw new InvalidOperationException(CAN_NOT_DELETE_ITEM_BECAUSE_IT_IS_LENT_TO_SOMEONE_ERROR_MESSAGE);
        }
    }
}