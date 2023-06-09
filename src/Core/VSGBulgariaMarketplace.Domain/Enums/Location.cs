namespace VSGBulgariaMarketplace.Domain.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum Location
    {
        Home = 1,
        Plovdiv = 2,

        [Display(Name = "Veliko Tarnovo")]
        VelikoTarnovo = 3
    }
}
