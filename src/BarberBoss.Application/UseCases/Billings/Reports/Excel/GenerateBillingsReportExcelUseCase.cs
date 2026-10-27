using AutoMapper;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using ClosedXML.Excel;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportExcelUseCase : IGenerateBillingsReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "$";

    private readonly IBillingRepositoriesReadOnly _repository;

    public GenerateBillingsReportExcelUseCase(IBillingRepositoriesReadOnly repository)
    {
        _repository = repository;
    }

    public async Task<byte[]> Execute()
    {
        var billings = await _repository.FilterByWeek();

        if (billings.Count == 0)
        {
            return [];
        }

        using var workbook = new XLWorkbook();

        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var worksheet = workbook.Worksheets.Add(ResourceReportGenerationMessages.BARBER_BOSS_REPORT);

        CreateHeader(worksheet);

        var line = 2;
        foreach (var billing in billings) {

            worksheet.Cell($"A{line}").Value = billing.ServiceName;
            worksheet.Cell($"B{line}").Value = billing.Date;
            worksheet.Cell($"C{line}").Value = billing.PaymentMethod.PaymentMethodToString();

            worksheet.Cell($"D{line}").Value = billing.Amount;
            worksheet.Cell($"D{line}").Style.NumberFormat.Format = $"{CURRENCY_SYMBOL} #,##0.00";

            worksheet.Cell($"E{line}").Value = billing.Notes;

            worksheet.Cell($"A{line}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Cell($"B{line}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"C{line}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"D{line}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            worksheet.Cell($"E{line}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            line++;
        }

        foreach (var cell in worksheet.Column("E").CellsUsed())
        {
            cell.Style.Alignment.WrapText = true;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        }

        worksheet.Columns().AdjustToContents(minWidth: 20, maxWidth: 50);

        var file = new MemoryStream();
        workbook.SaveAs(file);

        return file.ToArray();
    }

    private void CreateHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        worksheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE; 
        worksheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
        worksheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        worksheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        worksheet.Cells("A1:E1").Style.Font.Bold = true;

        worksheet.Cells("A1:E1").Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
        worksheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#205858");

        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}
