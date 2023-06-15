namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.API.Constants;
    using VSGBulgariaMarketplace.Application.Constants;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Enums;

    //[Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        //[Route(ControllerConstant.GET_LOCATIONS_ROUTE)]
        [Route(ControllerConstant.GET_LOCATIONS_ROUTE_SPARTAK)]
        [Authorize(Policy = AuthorizationConstant.AUTHORIZATION_ADMIN_POLICY_NAME)]
        public IActionResult GetLocations()
        {
            List<string> locations = EnumService.GetAll<Location>();

            return Ok(locations);
        }
    }
}
