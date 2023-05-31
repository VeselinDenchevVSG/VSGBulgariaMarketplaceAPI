namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Services.HelpServices.Location.Interfaces;

    //[Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        [HttpGet]
        //[Route("pending-orders")]
        [Route("/getLocations")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetLocations()
        {
            List<string> locations = this.locationService.GetAllLocations();

            return Ok(locations);
        }
    }
}
