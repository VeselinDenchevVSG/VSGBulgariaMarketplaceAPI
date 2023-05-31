namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Location
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

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
                string locationString = GetLocationDisplayName(location);
                locationStrings.Add(locationString);
            }

            return locationStrings;
        }

        private static string GetLocationDisplayName(Location location)
        {
            MemberInfo? enumMember = location.GetType().GetMember(location.ToString())[0];
            DisplayAttribute? displayAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null ? displayAttribute.Name : location.ToString();
        }
    }
}
