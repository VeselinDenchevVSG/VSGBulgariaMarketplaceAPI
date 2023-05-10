namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;

    public interface IItemService
    {
        public GridItemDto[] GetAll();

        public ItemDetailsDto GetById(int id);

        public void Create(ManageItemDto createItemDto);

        public void Update(int id, ManageItemDto updateItemDto);

        public void Delete(int id);
    }
}
