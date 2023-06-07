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
        private const string USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY = "user-emails-with-lend-items-count";
        private const string USER_LEND_ITEMS_CACHE_KEY_TEMPLATE = "user-lend-items-{0}";

        private IItemRepository itemRepository;

        public ItemLoanService(IItemLoanRepository repository, IItemRepository itemRepository, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
            : base(repository, cacheAdapter, mapper)
        {
            this.itemRepository = itemRepository;
        }

        public List<EmailWithLendItemsCountDto> GetUserEmailsWithLendItemsCount()
        {
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDtos = base.cacheAdapter.Get<List<EmailWithLendItemsCountDto>>(USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY);
            if (emailsWithLendItemsCountDtos is null)
            {
                Dictionary<string, int> emailsWithLendItemsCount = base.repository.GetUserEmailWithLendItemsCount();
                emailsWithLendItemsCountDtos = base.mapper.Map<Dictionary<string, int>,List<EmailWithLendItemsCountDto>>(emailsWithLendItemsCount);

                base.cacheAdapter.Set(USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY, emailsWithLendItemsCountDtos);
            }

            return emailsWithLendItemsCountDtos;
        }

        public UserLendItemDto[] GetUserLendItems(string email)
        {
            if (email is not null)
            {
                email = email.ToLower();

                UserLendItemDto[] userLendItemDtos = base.cacheAdapter.Get<UserLendItemDto[]>(string.Format(USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email));
                if (userLendItemDtos is null)
                {
                    ItemLoan[] userLendItems = base.repository.GetUserLendItems(email);
                    userLendItemDtos = base.mapper.Map<ItemLoan[], UserLendItemDto[]>(userLendItems);

                    base.cacheAdapter.Set(string.Format(USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email), userLendItemDtos);
                }

                return userLendItemDtos;
            }
            else throw new ArgumentException("Email is empty!");
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

                    this.ClearLoanRelatedCache(itemLoan.Email);
                }
            }
            else throw new ArgumentException("Such item doesn't exist!");
        }

        public void Return(string id)
        {
            ItemLoan itemLoan = base.repository.GetItemLoanItemIdQuantityAndEmail(id);
            if (itemLoan is null) throw new NotFoundException($"Item loan doesn't exist!");

            this.itemRepository.RestoreItemQuantitiesWhenReturningLendItems(itemLoan.ItemId, itemLoan.Quantity);
            base.repository.Return(id);

            this.ClearLoanRelatedCache(itemLoan.Email);
        }

        private void ClearLoanRelatedCache(string email)
        {
            base.cacheAdapter.Remove(ItemService.INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email));
        }
    }
}