using Microsoft.AspNetCore.Mvc;
using Spenses.Domain.Models.Homes;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomesController : ControllerBase
{
    [HttpGet]
    public ActionResult<Home[]> Get()
    {
        return new[]
        {
            new Home
            {
                Id = Guid.NewGuid(),
                Name = "Our house",
                Description = "Is in the middle of the street"
            }
        };
    }
}
