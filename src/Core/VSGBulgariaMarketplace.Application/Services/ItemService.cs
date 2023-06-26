namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;
    using CloudinaryDotNet.Actions;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Constants;
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
            MarketplaceItemDto[] itemDtos = base.cacheAdapter.Get<MarketplaceItemDto[]>(ServiceConstant.MARKETPLACE_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetMarketplace();
                itemDtos = base.mapper.Map<Item[], MarketplaceItemDto[]>(items);

                foreach (MarketplaceItemDto marketplaceItem in itemDtos)
                {
                    marketplaceItem.ImageUrl = this.imageService.GetImageUrlByItemId(marketplaceItem.Id);
                }

                base.cacheAdapter.Set(ServiceConstant.MARKETPLACE_CACHE_KEY, itemDtos);
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
            InventoryItemDto[] itemDtos = base.cacheAdapter.Get<InventoryItemDto[]>(ServiceConstant.INVENTORY_CACHE_KEY);
            if (itemDtos is null)
            {
                Item[] items = base.repository.GetInventory();
                itemDtos = base.mapper.Map<Item[], InventoryItemDto[]>(items);

                foreach (InventoryItemDto inventoryItem in itemDtos)
                {
                    inventoryItem.ImageUrl = this.imageService.GetImageUrlByItemId(inventoryItem.Id);
                }

                base.cacheAdapter.Set(ServiceConstant.INVENTORY_CACHE_KEY, itemDtos);
            }

            return itemDtos;
        }

        public ItemDetailsDto GetById(string id)
        {
            string itemCacheKey = string.Format(ServiceConstant.ITEM_CACHE_KEY_TEMPLATE, id);

            ItemDetailsDto itemDto = base.cacheAdapter.Get<ItemDetailsDto>(itemCacheKey);
            if (itemDto is null)
            {
                Item item = base.repository.GetById(id);

                if (item is null) throw new NotFoundException(ServiceConstant.SUCH_ITEM_DOES_NOT_EXIST_ERROR_MESSAGE);

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

            base.cacheAdapter.Remove(ServiceConstant.MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(ServiceConstant.INVENTORY_CACHE_KEY);
        }

        public async Task UpdateAsync(string id, UpdateItemDto updateItemDto, CancellationToken cancellationToken) 
        {
            short pendingOrderdTotalItemQuantity = await this.orderRepository.GetPendingOrdersTotalItemQuantityByItemId(id, cancellationToken);
            if (updateItemDto.QuantityForSale <= pendingOrderdTotalItemQuantity)
            {
                throw new ArgumentException(ServiceConstant.NOT_ENOUGH_QUANTITY_FOR_SALE_IN_ORDER_TO_COMPLETE_PENDING_ORDERS_WITH_THIS_ITEM_ERROR_MESSAGE);
            }

            short itemLoansTotalItemQuantity = await this.itemLoanRepository.GetItemLoansTotalQuantityForItem(id, cancellationToken);
            if (updateItemDto.Quantity <= itemLoansTotalItemQuantity)
            {
                throw new ArgumentException(ServiceConstant.QUANTITY_COMBINED_MUST_NOT_BE_LOWER_THAN_THE_ACTIVE_LOANS_ITEM_QUANTITY_ERROR_MESSAGE);
            }

            Item item = base.mapper.Map<UpdateItemDto, Item>(updateItemDto);

            bool imageIsDeleted = false;

            string itemImagePublicId = await this.repository.GetItemImagePublicId(id, cancellationToken);
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
                        imageIsDeleted = deletionResult == ServiceConstant.CLOUDINARY_DELETION_RESULT_OK;
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

            base.cacheAdapter.Remove(ServiceConstant.MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(ServiceConstant.INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ServiceConstant.ITEM_CACHE_KEY_TEMPLATE, id));
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            bool isLoanWithItem = await this.itemLoanRepository.IsLoanWithItem(id, cancellationToken);
            if (!isLoanWithItem)
            {
                string itemPicturePublicId = await this.repository.GetItemImagePublicId(id, cancellationToken);

                await this.orderRepository.DeclineAllPendingOrdersWithDeletedItemAsync(id, cancellationToken);

                base.repository.Delete(id);

                if (itemPicturePublicId is not null)
                {
                    await this.imageService.DeleteAsync(itemPicturePublicId);
                }

                base.cacheAdapter.Clear();
            }
            else throw new InvalidOperationException(ServiceConstant.CAN_NOT_DELETE_ITEM_BECAUSE_IT_IS_LENT_TO_SOMEONE_ERROR_MESSAGE);
        }
    }
}