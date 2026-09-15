using BarberBoss.Communication.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingController : ControllerBase
{
    [HttpPost]
    public IActionResult registerBilling([FromBody] RequestBillingJson request)
    {
        return Ok(request);
    }
}
