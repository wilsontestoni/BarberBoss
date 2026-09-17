using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Communication.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingController : ControllerBase
{
    [HttpPost]
    public IActionResult registerBilling(
        [FromServices] IRegisterBillingUseCase useCase,
        [FromBody] RequestBillingJson request)
    {
        var response = useCase.Execute(request);

        return Created(string.Empty, response);
    }
}
