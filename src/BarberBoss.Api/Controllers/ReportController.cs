using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    [HttpGet("excel")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetExcel(
       [FromServices] IGenerateBillingsReportExcelUseCase useCase)
    {
        byte[] file = await useCase.Execute();

        if (file.Length <= 0)
        {
            return NoContent();
        }

        return File(file, MediaTypeNames.Application.Octet, "report.xlsx");
    }

    [HttpGet("pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetPdf(
       [FromServices] IGenerateBillingsReportPdfUseCase useCase)
    {
        byte[] file = await useCase.Execute();

        if (file.Length <= 0)
        {
            return NoContent();
        }

        return File(file, MediaTypeNames.Application.Pdf, "report.pdf");
    }
}
