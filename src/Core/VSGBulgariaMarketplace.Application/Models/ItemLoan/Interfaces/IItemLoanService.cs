﻿namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;

    public interface IItemLoanService
    {
        List<EmailWithLendItemsCountDto> GetUserEmailsWithLendItemsCount();

        UserLendItemDto[] GetUserLendItems(string email);

        void LendItems(string itemId, LendItemsDto lendItems);

        void Return(string id);
    }
}
