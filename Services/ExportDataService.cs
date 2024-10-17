using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PhotocopyRevaluationApp.Data;
using System.Collections.Generic;

namespace PhotocopyRevaluationApp.Services {
    public class ExportDataService : IExportDataService {
        private readonly PhotocopyRevaluationAppContext _context;
        //To export PDF
        private readonly IConverter _converter;
        public ExportDataService(IConverter converter, PhotocopyRevaluationAppContext context) {
            _context = context;
            _converter = _converter;
        }

        public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> collection) {
            try {
                using (var package = new ExcelPackage()) {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells.LoadFromCollection(collection, true); // Load data into worksheet

                    // Save the package into a memory stream
                    using (var stream = new MemoryStream()) {
                        await package.SaveAsAsync(stream); // Asynchronously save the Excel package to memory
                        byte[] content = stream.ToArray(); // Convert the stream to a byte array
                        return content; // Return success flag and byte array
                    }
                }
            }
            catch (Exception ex) {
                // Handle or log the exception
                Console.WriteLine($"Error exporting to Excel: {ex.Message}");
                return null; // Return failure flag and null content
            }
        }


        public async Task<byte[]> ExportDataToPdfAsync() {
            //To get the data from form body
            //public IActionResult ExportDataToPdf([FromBody] List<ExportDataModel> tableData)
            //{
            //    // Step 4: Generate HTML content for the PDF
            //    var htmlContent = GenerateHtmlForPdf(tableData);

            // Step 4: Generate HTML content for the PDF
            var htmlContent = await GenerateHtmlForPdfAsync();

            // Step 5: Convert HTML to PDF using DinkToPdf
            var pdfDocument = new HtmlToPdfDocument() {
                GlobalSettings = new GlobalSettings {
                    PaperSize = PaperKind.A4, // Set to A4 size
                    Orientation = Orientation.Portrait, // Portrait or Landscape
                    Out = null // No physical file output; use MemoryStream
                },
                Objects = {
                new ObjectSettings {
                    PagesCount = true,
                    HtmlContent = htmlContent, // Insert generated HTML
                    WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            // Generate the PDF
            byte[] pdfBytes = _converter.Convert(pdfDocument);
            return pdfBytes;
        }
        // This method generates the HTML content for the PDF
        private async Task<string> GenerateHtmlForPdfAsync() {
            string Procedure = "SELECT * FROM PHOTOCOPY";

            string html = "<h2>Photocopies</h2><table class=table  border='1' cellpadding='5' cellspacing='0'>";
            html += "<thead><tr><th><b>Sno</b></th><th>@Html.DisplayNameFor(model => model.EventName)</th><th>@Html.DisplayNameFor(model => model.Scheme)</th><th>@Html.DisplayNameFor(model => model.Subject)</th><th>@Html.DisplayNameFor(model => model.SubPostCode)</th><th>@Html.DisplayNameFor(model => model.PinNumber)</th><th>@Html.DisplayNameFor(model => model.StudentName)</th><th>@Html.DisplayNameFor(model => model.OrderId)</th><th>@Html.DisplayNameFor(model => model.PgiRefNo)</th><th>@Html.DisplayNameFor(model => model.Amount)</th><th>@Html.DisplayNameFor(model => model.MobileNumber)</th><th>@Html.DisplayNameFor(model => model.Email)</th><th>@Html.DisplayNameFor(model => model.CreatedDate)</th><th>@Html.DisplayNameFor(model => model.IsPaid)</th><th>@Html.DisplayNameFor(model => model.BillerId)</th></tr></thead><tbody>";

            var photocopyies = await _context.Photocopies.FromSqlRaw(Procedure).ToListAsync();

            int i = 1;
            foreach (var row in photocopyies) {
                html += $"<tr><td><b>{i}</b></td><td>{row.EventName}</td><td>{row.Scheme}</td><td>{row.Subject}</td><td>{row.SubPostCode}</td><td>{row.PinNumber}</td><td>{row.StudentName}</td><td>{row.OrderId}</td><td>{row.PgiRefNo}</td><td>{row.Amount}</td><td>{row.MobileNumber}</td><td>{row.Email}</td><td>{row.CreatedDate}</td><td>{row.IsPaid}</td><td>{row.BillerId}</td></tr>";
            }

            html += "</tbody></table>";

            return html;
        }
    }
}
