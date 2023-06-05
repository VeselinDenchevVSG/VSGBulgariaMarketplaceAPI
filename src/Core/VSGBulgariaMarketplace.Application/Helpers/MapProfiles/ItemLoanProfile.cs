namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;
    using System.Collections.Generic;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemLoanProfile : Profile
    {
        public ItemLoanProfile()
        {
            CreateMap<Dictionary<string, int>, List<EmailWithLendItemsCountDto>>().ConvertUsing((dictionary) => 
                                                                                                    MapEmailsWithLendItemsCount(dictionary));

            CreateMap<ItemLoan, UserLendItemDto>().ForMember(dto => dto.StartDate, x => x.MapFrom(e => e.CreatedAtUtc.ToLocalTime()
                                                                                                                                                                .ToShortDateString()))
                                                    .ForMember(dto => dto.EndDate, 
                                                                x => x.MapFrom(e => e.EndDatetimeUtc.HasValue ? e.EndDatetimeUtc.Value.ToLocalTime()
                                                                                                                                                                .ToShortDateString() 
                                                                                                                                                                : null));

            CreateMap<LendItemsDto, ItemLoan>();
        }

        private List<EmailWithLendItemsCountDto> MapEmailsWithLendItemsCount(Dictionary<string, int> emailsWithlendItemsCount)
        {
            List<EmailWithLendItemsCountDto> emailsWithlendItemsCountDto = new List<EmailWithLendItemsCountDto>();

            foreach (KeyValuePair<string, int> pair in emailsWithlendItemsCount)
            {
                emailsWithlendItemsCountDto.Add(new EmailWithLendItemsCountDto
                {
                    Email = pair.Key,
                    LendItemsCount = pair.Value
                });
            }

            return emailsWithlendItemsCountDto;
        }
    }
}