using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using PhotocopyRevaluationApp.Services;
using PhotocopyRevaluationApp.Models;
using PhotocopyRevaluationApp.Data;

namespace PhotocopyRevaluationApp.Controllers {
    public class PhotocopiesController : Controller {
        private readonly PhotocopyRevaluationAppContext _context;
        private readonly IExportDataService _exportDataService;
        public PhotocopiesController(PhotocopyRevaluationAppContext context, IExportDataService exportDataService) {
            _context = context;
            _exportDataService = exportDataService;
        }

        // GET: Photocopies
        public async Task<IActionResult> Index() {
            //var photocopy = await _context.Photocopies.FromSqlRaw("SELECT * FROM PHOTOCOPY").ToListAsync();

            //var photocopyButtonViewModel = new PhotocopyButtonViewModel
            //{
            //    eventSchemeSubjectWiseCountList = candidateWisePhotoCopy,
            //    photocopies = photocopy
            //};

            return View();
        }

        public async Task<IActionResult> _List(string ColumnName, string Value) {
            List<Photocopy> photocopyies;
            string Procedure;
            ViewData["X-Axis"] = ColumnName;

            if (ColumnName == "nodata") {
                Procedure = "EXEC GetAllPhotocopies";
                photocopyies = await _context.Photocopies.FromSqlRaw(Procedure).ToListAsync();
            }
            else if (DateTime.TryParseExact(Value, "YYYY-MM-DD", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)) {
                // Use parameterized queries to prevent SQL Injection
                photocopyies = await _context.Photocopies
                    .Where(i => EF.Functions.Like($"I.%{ColumnName}%", $"%{DateTime.Parse(Value).Date}%"))
                    .ToListAsync();

                //Procedure = $"SELECT * FROM PHOTOCOPY WHERE {ColumnName} like '{DateTime.Parse(Value).Date}%'";
            }
            else {
                // Use parameterized queries to prevent SQL Injection
                photocopyies = await _context.Photocopies
                    .Where(i => EF.Functions.Like($"I.%{ColumnName}%", $"%{Value}%"))
                    .ToListAsync();

                //Procedure = $"SELECT * FROM PHOTOCOPY WHERE {ColumnName} like '{Value}%'";

            }

            //if (TempData.ContainsKey("ColumnName") && TempData.ContainsKey("Value"))
            //{

            //}  + " " + $"{TempData["ColumnName"] = TempData["Value"]}"
            //

            return View(default);
        }

        [HttpGet]
        public async Task<IActionResult> _Statastics(string? X_Axis) {
            if (X_Axis == "Date") {
                ViewData["Title"] = "CreatedDate";
                var Data = await _context.Photocopies.GroupBy(r => new { r.CreatedDate }).Select(r => new {
                    Column1 = r.Key.CreatedDate.Date,
                    Column2 = r.Count()
                }).ToListAsync();

                return View(Data);
            }
            else if (X_Axis == "EventName") {
                ViewData["Title"] = "Event";
                var Data = await _context.Photocopies.GroupBy(static p => new { p.EventName }).Select(d => new {
                    Column1 = d.Key.EventName,
                    Column2 = d.Count()
                }).ToListAsync();

                return View(Data);
            }
            else {
                ViewData["Title"] = "Scheme";
                var Data = await _context.Photocopies.GroupBy(p => new { p.Scheme }).Select(d => new {
                    Column1 = d.Key.Scheme,
                    Column2 = d.Count()
                }).ToListAsync();

                return View(Data);
            }

        }

        [HttpGet]
        public async Task<IActionResult> SubjectWisePhotocopyCount() {
            var SubjectWisePhotocopyCount = await _context.Photocopies.GroupBy(static p => new { p.Subject }).Select(d => new SubjectWisePhotocopyCountDTO {
                Subject = d.Key.Subject,
                Count = d.Count()
            }).ToListAsync();

            return View(SubjectWisePhotocopyCount);
        }
        public async Task<IActionResult> DateWisePhotocopyCount() {
            var DatewisePhotocopyCount = await _context.Photocopies.GroupBy(static p => new { CreatedDate = p.CreatedDate.Date }).Select(d => new DatewisePhotocopyCountViewDTO {
                CreatedDate = d.Key.CreatedDate.Date,
                PhotocopyCount = d.Count()
            }).ToListAsync();

            return View(DatewisePhotocopyCount);
        }
        [HttpGet]
        public async Task<IActionResult> SchemeWisePhotocopyCount() {
            var SchemePhotocopyCountDTO = await _context.Photocopies.GroupBy(p => new { p.Scheme }).Select(d => new SchemePhotocopyCountDTO {
                Scheme = d.Key.Scheme,
                PhotocopyCount = d.Count()
            }).ToListAsync();

            return View(SchemePhotocopyCountDTO);
        }
        [HttpGet]
        public async Task<IActionResult> EventWisePhotocopyCount() {
            var EventPhotocopyCountDTO = await _context.Photocopies.GroupBy(static p => new { p.EventName }).Select(d => new EventPhotocopyCountDTO {
                Event = d.Key.EventName,
                PhotocopyCount = d.Count()
            }).ToListAsync();

            return View(EventPhotocopyCountDTO);
        }
        public async Task<IActionResult> ExportToExcelAsync(string ColumnName, string Value) {
            string Procedure = "EXEC GetAllPhotocopies";

            // Fetch your data here, for example from a database
            IEnumerable<Photocopy> photocopyies = _context.Photocopies.FromSqlRaw(Procedure).ToList();

            byte[] content = await _exportDataService.ExportToExcelAsync(photocopyies);

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TableData.xlsx");
        }

        //To export PDF
        //ExportDataToPdf using DinkToPdf nugate package using server side processing 
        //[FromBody] List<Photocopy> Photocopy
        [HttpGet]
        public async Task<IActionResult> ExportDataToPdf() {
            byte[] pdfBytes = await _exportDataService.ExportDataToPdfAsync();

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "TableData.pdf");
        }












        // GET: Photocopies/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var photocopy = await _context.photocopy
        //            .FirstOrDefaultAsync(m => m.SNO == id);
        //        if (photocopy == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(photocopy);
        //    }

        //    // GET: Photocopies/Create
        //    public IActionResult Create()
        //    {
        //        return View();
        //    }

        //    // POST: Photocopies/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("SNO,EVENT_NAME,Scheme,Subject,SubPostCode,PinNumber,StudentName,OrderId,PGI_Ref_No,Amount,MobileNumber,Email,CreatedDate,IsPaid")] Photocopy photocopy)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(photocopy);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(photocopy);
        //    }

        //    // GET: Photocopies/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var photocopy = await _context.photocopy.FindAsync(id);
        //        if (photocopy == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(photocopy);
        //    }

        //    // POST: Photocopies/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("SNO,EVENT_NAME,Scheme,Subject,SubPostCode,PinNumber,StudentName,OrderId,PGI_Ref_No,Amount,MobileNumber,Email,CreatedDate,IsPaid")] Photocopy photocopy)
        //    {
        //        if (id != photocopy.SNO)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(photocopy);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!PhotocopyExists(photocopy.SNO))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(photocopy);
        //    }

        //    // GET: Photocopies/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var photocopy = await _context.photocopy
        //            .FirstOrDefaultAsync(m => m.SNO == id);
        //        if (photocopy == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(photocopy);
        //    }

        //    // POST: Photocopies/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var photocopy = await _context.photocopy.FindAsync(id);
        //        if (photocopy != null)
        //        {
        //            _context.photocopy.Remove(photocopy);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool PhotocopyExists(int id)
        //    {
        //        return _context.photocopy.Any(e => e.SNO == id);
        //    }
    }
}
