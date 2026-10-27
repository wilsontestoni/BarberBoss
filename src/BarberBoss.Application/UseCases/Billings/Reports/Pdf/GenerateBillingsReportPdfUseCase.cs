using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Colors;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System.Globalization;
using System.Reflection;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "$";

    private const int HEIGHT_ROW_EXPENSE_TABLE = 25;

    private readonly IBillingRepositoriesReadOnly _repository;

    public GenerateBillingsReportPdfUseCase(IBillingRepositoriesReadOnly repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new BillingsReportFontResolver();
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var billings = await _repository.FilterByWeek();

        if (billings.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(month);
        var page = CreatePage(document);

        var totalBillings = billings.Sum(billing => billing.Amount);
        CreateHeaderWithPhoto(page);
        CreateTotalSpentSection(page, totalBillings);

        foreach (var billing in billings)
        {
            var table = CreateExpenseTable(page);

            var row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;

            AddBillingServiceName(row.Cells[0], row.Cells[1], row.Cells[2], billing.ServiceName);
            AddHeaderForAmount(row.Cells[3]);

            row = table.AddRow();
            row.Height = HEIGHT_ROW_EXPENSE_TABLE;

            var culture = CultureInfo.CurrentCulture;

            var dateFormat = culture.Name switch
            {
                "pt-BR" => "dd 'de' MMMM 'de' yyyy",
                "en-US" => "MMMM dd, yyyy",
                "en" => "MMMM dd, yyyy",
                "fr-FR" => "dd MMMM yyyy",
                _ => "dd MMMM yyyy"
            };

            row.Cells[0].AddParagraph(
                billing.Date.ToString(dateFormat, culture)
            );
            SetStyleBaseForExpenseInformation(row.Cells[0]);
            row.Cells[0].Format.LeftIndent = 7;

            row.Cells[1].AddParagraph(billing.Date.ToString("t"));
            SetStyleBaseForExpenseInformation(row.Cells[1]);

            row.Cells[2].AddParagraph(billing.PaymentMethod.PaymentMethodToString());
            SetStyleBaseForExpenseInformation(row.Cells[2]);

            AddAmountForExpense(row.Cells[3], billing.Amount);

            if (string.IsNullOrWhiteSpace(billing.Notes) == false)
            {
                var descriptionRow = table.AddRow();
                descriptionRow.Height = HEIGHT_ROW_EXPENSE_TABLE;

                descriptionRow.Cells[0].AddParagraph(billing.Notes);
                descriptionRow.Cells[0].Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 9, Color = ColorsHelper.GRAY };
                descriptionRow.Cells[0].Shading.Color = ColorsHelper.LIGHT_GRAY;
                descriptionRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                descriptionRow.Cells[0].MergeRight = 2;
                descriptionRow.Cells[0].Format.LeftIndent = 7;
            }

            AddWhiteSpace(table);
        };


        return RenderDocument(document);
    }

    public Document CreateDocument(DateOnly month)
    {
        var document = new Document
        {
            Info =
                {
                    Title = $"{ResourceReportGenerationMessages.BILLINGS_FOR} {month:Y}",
                    Author = "BarberBoss"
                }
        };

        var style = document.Styles["Normal"];
        style!.Font.Name = Fonts.FontHelper.ROBOTO_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;

        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;

        return section;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document,
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        return file.ToArray();
    }

    private void CreateHeaderWithPhoto(Section page)
    {

        var table = page.AddTable();
        table.AddColumn();
        table.AddColumn("300");

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);
        var pathFile = Path.Combine(directoryName!, "Logo", "company_logo.png");

        row.Cells[0].AddImage(pathFile);

        row.Cells[1].AddParagraph(ResourceReportGenerationMessages.COMPANY_NAME);
        row.Cells[1].Format.Font = new Font { Name = Fonts.FontHelper.BEBASNEUE_REGULAR, Size = 25 };
        row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
        row.Cells[1].Format.LeftIndent = 18;
    }

    private void CreateTotalSpentSection (Section page, decimal totalSpent)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = "38";
        paragraph.Format.SpaceAfter = "64";

        var title = ResourceReportGenerationMessages.WEEKLY_BILLING;

        paragraph.AddFormattedText(title, new Font { Name = Fonts.FontHelper.ROBOTO_MEDIUM, Size = 15 });

        paragraph.AddLineBreak();

        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalSpent}", new Font { Name = Fonts.FontHelper.BEBASNEUE_REGULAR, Size = 50 });
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();

        table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("70").Format.Alignment = ParagraphAlignment.Right;

        return table;
    }

    private void AddBillingServiceName(Cell cell1, Cell cell2, Cell cell3, string serviceName)
    {
        cell1.AddParagraph(serviceName);
        cell1.Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 15, Color = ColorsHelper.WHITE };
        cell1.Shading.Color = ColorsHelper.DARK_GREEN;
        cell2.Shading.Color = ColorsHelper.DARK_GREEN;
        cell3.Shading.Color = ColorsHelper.DARK_GREEN;
        cell1.VerticalAlignment = VerticalAlignment.Center;
        cell1.Format.LeftIndent = 7;
    }

    private void AddHeaderForAmount(Cell cell)
    {
        cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
        cell.Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 15, Color = ColorsHelper.WHITE };
        cell.Shading.Color = ColorsHelper.GREEN;
        cell.VerticalAlignment = VerticalAlignment.Center;
       
    }

    private void SetStyleBaseForExpenseInformation(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 10, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.LIGHT_GREEN;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddAmountForExpense(Cell cell, decimal billingAmount)
    {
        cell.AddParagraph($"{CURRENCY_SYMBOL} {billingAmount.ToString()}");
        cell.Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 10, Color = ColorsHelper.BLACK };
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = ParagraphAlignment.Right;
    }

    private void AddWhiteSpace(Table table)
    {
        var row = table.AddRow();
        row.Height = 16;
        row.Borders.Visible = false;
    }
}
