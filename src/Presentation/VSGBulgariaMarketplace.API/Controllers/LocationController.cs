namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Enums;

    using static VSGBulgariaMarketplace.API.Constants.ControllerConstant;
    using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

    //[Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        //[Route(GET_LOCATIONS_ROUTE)]
        [Route(GET_LOCATIONS_ROUTE_SPARTAK)]
        [Authorize(Policy = AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult GetLocations()
        {
            List<string> locations = EnumService.GetAll<Location>();

            return Ok(locations);
        }
    }
}
