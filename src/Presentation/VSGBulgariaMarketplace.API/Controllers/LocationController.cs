namespace VSGBulgariaMarketplace.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Enums;

    //[Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        //[Route("pending-orders")]
        [Route("/getLocations")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetLocations()
        {
            List<string> locations = EnumService.GetAll<Location>();

            return Ok(locations);
        }
    }
}
