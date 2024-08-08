using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.JSInterop;
using TheHighInnovation.POS.Web.Model.Response.CloseTill;
using Element = iTextSharp.text.Element;

namespace TheHighInnovation.POS.Web.Services.Reports;

public static class ReportService
{
    public static void GeneratePdfReport(IJSRuntime jsRuntime, List<CloseTillResponseDto> transactionDetails, string employeeName, string role, string companyName)
    {
        jsRuntime.InvokeAsync<object>("downloadPdfReport", "Close Till - Transaction Details.pdf", Convert.ToBase64String(PdfReport(transactionDetails, employeeName, role, companyName)));
    }

    private static byte[] PdfReport(List<CloseTillResponseDto> transactionDetails, string employeeName, string role, string companyName)
    {
        var document = new Document(PageSize.A4, 20, 20, 40, 40);

        var output = new MemoryStream();

        var writer = PdfWriter.GetInstance(document, output);

        writer.CloseStream = false;

        writer.PageEvent = new PdfPageEventHelper();

        document.Open();

        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);

        var title = new Paragraph("The High Innovation", titleFont)
        {
            Alignment = Element.ALIGN_CENTER
        };

        document.Add(title);

        var companyTag = new Paragraph(companyName, titleFont)
        {
            Alignment = Element.ALIGN_CENTER
        };

        document.Add(companyTag);

        var headingFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

        var headingParagraph = new Paragraph("Close Till Summary", headingFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };

        document.Add(headingParagraph);

        var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        var currentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

        var infoParagraph = new Paragraph();

        var dateChunk = new Chunk($"{currentDate}", dateFont);

        infoParagraph.Add(Chunk.TABBING);

        infoParagraph.Add(Chunk.TABBING);

        infoParagraph.Add(dateChunk);

        infoParagraph.TabSettings = new TabSettings(200f);

        infoParagraph.SpacingAfter = 20;

        document.Add(infoParagraph);

        var transactionTable = new PdfPTable(5)
        {
            WidthPercentage = 100
        };

        transactionTable.SetWidths(new[] { 2, 1, 1, 1, 1 });

        transactionTable.AddCell(CreateCell("Transaction Option", true));
        transactionTable.AddCell(CreateCell("Total Sales", true));
        transactionTable.AddCell(CreateCell("Total After Discount", true));
        transactionTable.AddCell(CreateCell("Total Discount Allocated", true));
        transactionTable.AddCell(CreateCell("Total Safe Drop", true));

        if (!transactionDetails.Any())
        {
            var noRecordsCell = new PdfPCell(new Phrase("No records to display.", FontFactory.GetFont(FontFactory.HELVETICA)))
            {
                Colspan = 4,
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            transactionTable.AddCell(noRecordsCell);
        }
        else
        {
            foreach (var item in transactionDetails)
            {
                transactionTable.AddCell(CreateCell(item.TransactionOption));
                transactionTable.AddCell(CreateCell($"Rs {item.TotalSales.ToString("N2", CultureInfo.CreateSpecificCulture("ne-NP"))}"));
                transactionTable.AddCell(CreateCell($"Rs {item.TotalAfterDiscount.ToString("N2", CultureInfo.CreateSpecificCulture("ne-NP"))}"));
                transactionTable.AddCell(CreateCell($"Rs {item.TotalDiscountGiven.ToString("N2", CultureInfo.CreateSpecificCulture("ne-NP"))}"));
                transactionTable.AddCell(CreateCell($"Rs {item.TotalSafeDrop.ToString("N2", CultureInfo.CreateSpecificCulture("ne-NP"))}"));
            }
        }

        document.Add(transactionTable);

        document.Add(new Paragraph("\n"));

        var reportSignatureFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 12);

        var reportFooterSignature = new Paragraph("_____________________________________", reportSignatureFont)
        {
            Alignment = Element.ALIGN_RIGHT,
            SpacingAfter = 10
        };

        document.Add(reportFooterSignature);

        var reportFooterFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 12);

        var reportFooterParagraph = new Paragraph($"{employeeName} ({role})", reportFooterFont)
        {
            Alignment = Element.ALIGN_RIGHT,
            SpacingAfter = 10
        };

        document.Add(reportFooterParagraph);

        document.Close();

        return output.ToArray();
    }

    private static PdfPCell CreateCell(string content, bool isHeader = false)
    {
        var cell = new PdfPCell(new Phrase(content, isHeader ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD) : FontFactory.GetFont(FontFactory.HELVETICA)))
        {
            Padding = 8,
            BackgroundColor = isHeader ? new BaseColor(200, 200, 200) : BaseColor.WHITE
        };

        return cell;
    }

}