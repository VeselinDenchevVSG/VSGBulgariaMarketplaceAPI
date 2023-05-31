namespace VSGBulgariaMarketplace.Domain.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum Category
    {
        Laptops = 1,

        Furniture = 2,

        [Display(Name = "Office tools")]
        OfficeTools = 3,

        Misc = 4
    }
}
