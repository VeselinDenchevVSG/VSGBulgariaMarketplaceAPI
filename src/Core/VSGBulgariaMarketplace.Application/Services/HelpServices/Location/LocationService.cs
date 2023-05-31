namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Location
{
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Location.Interfaces;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class LocationService : ILocationService
    {
        public List<string> GetAllLocations()
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
    }
}
