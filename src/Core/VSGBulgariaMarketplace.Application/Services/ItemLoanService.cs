namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemLoanService : BaseService<IItemLoanRepository, ItemLoan>, IItemLoanService
    {
        private IItemRepository itemRepository;

        public ItemLoanService(IItemLoanRepository repository, IItemRepository itemRepository, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(repository, cacheAdapter, mapper)
        {
            this.itemRepository = itemRepository;
        }

        public List<EmailWithLendItemsCountDto> GetUserEmailWithLendItemsCount()
        {
            Dictionary<string, int> emailsWithLendItemsCount = base.repository.GetUserEmailWithLendItemsCount();
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDtos  = base.mapper.Map<Dictionary<string, int>, List<EmailWithLendItemsCountDto>>(emailsWithLendItemsCount);

            return emailsWithLendItemsCountDtos;
        } 

        public UserLendItemDto[] GetUserLendItems(string email)
        {
            ItemLoan[] userLendItems = base.repository.GetUserLendItems(email);
            UserLendItemDto[] userLendItemDtos = base.mapper.Map<ItemLoan[], UserLendItemDto[]>(userLendItems);

            return userLendItemDtos;
        }

        public void LendItems(string itemId, LendItemsDto lendItems)
        {
            bool exists = this.itemRepository.TryGetAvailableQuantity(itemId, out int? availableQuantity);
            if (exists)
            {
                if (lendItems.Quantity < availableQuantity)
                {
                    ItemLoan itemLoan = base.mapper.Map<LendItemsDto, ItemLoan>(lendItems);
                    itemLoan.ItemId = itemId;

                    base.repository.Create(itemLoan);

                    this.itemRepository.RequestItemLoan(itemId, lendItems.Quantity);
                }
            }
            else throw new ArgumentException("Such item doesn't exist!");
        }

        public void Return(string id)
        {
            ItemLoan itemLoan = base.repository.GetItemLoanItemIdAndQuantity(id);
            if (itemLoan is null) throw new NotFoundException($"Order doesn't exist!");

            this.itemRepository.RestoreItemQuantitiesWhenReturningLendItems(itemLoan.ItemId, itemLoan.Quantity);
            base.repository.Return(id);
        }
    }
}