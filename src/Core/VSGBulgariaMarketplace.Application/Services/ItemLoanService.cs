namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Constants;
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

        public List<EmailWithLendItemsCountDto> GetUserEmailsWithLendItemsCount()
        {
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDtos = base.cacheAdapter.Get<List<EmailWithLendItemsCountDto>>(
                                                                                ServiceConstant.USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY);
            if (emailsWithLendItemsCountDtos is null)
            {
                Dictionary<string, int> emailsWithLendItemsCount = base.repository.GetUserEmailWithLendItemsCount();
                emailsWithLendItemsCountDtos = base.mapper.Map<Dictionary<string, int>,List<EmailWithLendItemsCountDto>>(emailsWithLendItemsCount);

                base.cacheAdapter.Set(ServiceConstant.USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY, emailsWithLendItemsCountDtos);
            }

            return emailsWithLendItemsCountDtos;
        }

        public UserLendItemDto[] GetUserLendItems(string email)
        {
            var emailValidator = new InlineValidator<string>();
            emailValidator.RuleFor(e => e).NotEmpty().Matches(ValidationConstant.VSG_EMAIL_REGEX_PATTERN).WithMessage(ValidationConstant.INVALID_EMAIL_FORMAT_ERROR_MESSAGE);
            var emailValidationResult = emailValidator.Validate(email);
            if (emailValidationResult.IsValid)
            {
                email = email.ToLower();

                UserLendItemDto[] userLendItemDtos = base.cacheAdapter.Get<UserLendItemDto[]>(string.Format(ServiceConstant.USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email));
                if (userLendItemDtos is null)
                {
                    ItemLoan[] userLendItems = base.repository.GetUserLendItems(email);
                    userLendItemDtos = base.mapper.Map<ItemLoan[], UserLendItemDto[]>(userLendItems);

                    base.cacheAdapter.Set(string.Format(ServiceConstant.USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email), userLendItemDtos);
                }

                return userLendItemDtos;
            }
            else throw new ArgumentException(ValidationConstant.INVALID_EMAIL_FORMAT_ERROR_MESSAGE);
        }

        public void LendItems(string itemId, LendItemsDto lendItems)
        {
            bool exists = this.itemRepository.TryGetAvailableQuantity(itemId, out int? availableQuantity);
            if (exists)
            {
                bool isEnoughQuantity = lendItems.Quantity <= availableQuantity;
                if (isEnoughQuantity)
                {
                    ItemLoan itemLoan = base.mapper.Map<LendItemsDto, ItemLoan>(lendItems);
                    itemLoan.ItemId = itemId;

                    base.repository.Create(itemLoan);

                    this.itemRepository.RequestItemLoan(itemId, lendItems.Quantity);

                    this.ClearLoanRelatedCache(itemLoan.Email);
                }
                else throw new ArgumentException(ServiceConstant.NOT_ENOUGH_AVAILABLE_QUANTITY_FOR_LENDING_IN_STOCK_ERROR_MESSAGE);
            }
            else throw new NotFoundException(ServiceConstant.SUCH_ITEM_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        public void Return(string id)
        {
            ItemLoan itemLoan = base.repository.GetItemLoanItemIdQuantityAndEmail(id);
            if (itemLoan is null) throw new NotFoundException(ServiceConstant.SUCH_ITEM_LOAN_DOES_NOT_EXISTS_ERROR_MESSAGE);

            this.itemRepository.RestoreItemQuantitiesWhenReturningLendItems(itemLoan.ItemId, itemLoan.Quantity);
            base.repository.Return(id);

            this.ClearLoanRelatedCache(itemLoan.Email);
        }

        private void ClearLoanRelatedCache(string email)
        {
            base.cacheAdapter.Remove(ServiceConstant.INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(ServiceConstant.USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ServiceConstant.USER_LEND_ITEMS_CACHE_KEY_TEMPLATE, email));
        }
    }
}