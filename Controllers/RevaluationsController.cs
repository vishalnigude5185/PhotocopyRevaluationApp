using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotocopyRevaluationAppMVC.Data;
using PhotocopyRevaluationAppMVC.Models;
using PhotocopyRevaluationAppMVC.Services;
using System.Globalization;

namespace PhotocopyRevaluationAppMVC.Controllers
{
    //[Authorize]
    public class RevaluationsController : Controller
    {
        private readonly PhotocopyRevaluationAppContext _context;
        private readonly IExportDataService _exportDataService;
        public RevaluationsController(PhotocopyRevaluationAppContext context, IExportDataService exportDataService)
        {
            _exportDataService = exportDataService;
            _context = context;
        }

        // GET: Revaluations
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> _List(string ColumnName, string Value)
        {
            List<Revaluation> Revaluations;
            string Procedure;

            if (ColumnName == "nodata")
            {
                Procedure = "EXEC GetAllRevaluations";
                Revaluations = await _context.Revaluations.FromSqlRaw(Procedure).ToListAsync();
            }
            else if (DateTime.TryParseExact(Value, "YYYY-MM-DD", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                // Use parameterized queries to prevent SQL Injection
                Revaluations = await _context.Revaluations
                    .Where(i => EF.Functions.Like($"I.%{ColumnName}%", $"%{DateTime.Parse(Value).Date}%"))
                    .ToListAsync();

                //Procedure = $"SELECT * FROM reValuation WHERE {ColumnName} like '{DateTime.Parse(Value).Date}%'";
            }
            else
            {
                // Use parameterized queries to prevent SQL Injection
                Revaluations = await _context.Revaluations
                    .Where(i => EF.Functions.Like($"I.%{ColumnName}%", $"%{DateTime.Parse(Value).Date}%"))
                    .ToListAsync();

                //Procedure = $"SELECT * FROM reValuation WHERE {ColumnName} like '{Value}%'";

            }

            return View(Revaluations);
        }

        [HttpGet]
        //[Consumes("application/json")] // This specifies that the action method expects JSON content
        public async Task<IActionResult> _Statastics(string X_Axis)
        {
            //HttpContext.Session.SetString("Title", X_Axis);

            if (X_Axis == "CreatedDate")
            {
                HttpContext.Session.SetString("Title", "Date");
                ViewData["Title"] = "Date";
                // Retrieve and group the data, then serialize it correctly for the front end
                var data = await _context.Revaluations
                    .GroupBy(r => new { r.CreatedDate.Date })
                    .Select(r => new
                    {
                        Column1 = r.Key.Date.ToString("yyyy-MM-dd"), // Format date as a string
                        Column2 = r.Count() // Count of students
                    })
                    .ToListAsync();

                return View("_Statastics", data);
            }
            else if (X_Axis == "EventName")
            {
                HttpContext.Session.SetString("Title", "Event");
                ViewData["Title"] = "Event";
                var Data = await _context.Revaluations.GroupBy(static p => new { p.EventName }).Select(d => new 
                {
                    Column1 = d.Key.EventName,
                    Column2 = d.Count()
                }).ToListAsync();

                return View(Data);
            }
            else
            {
                var Data = await _context.Revaluations.GroupBy(p => new { p.Scheme }).Select(d => new 
                {
                    Column1 = d.Key.Scheme,
                    Column2 = d.Count()
                }).ToListAsync();
                
                return View(Data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SubjectWiseRevaluationCount()
        {
            var SubjectWisePhotocopyCount = await _context.Revaluations.GroupBy(static p => new { p.Subject }).Select(d => new SubjectWiseRevaluationCountDTO
            {
                Subject = d.Key.Subject,
                Count = d.Count()
            }).ToListAsync();

            return View(SubjectWisePhotocopyCount);
        }
        [HttpGet]
        public async Task<IActionResult> DateWiseRevaluationCount()
        {
            var DatewiseRevaluationCountDTO = await _context.Revaluations.GroupBy(static p => new { CreatedDate = p.CreatedDate.Date }).Select(d => new DatewiseRevaluationCountDTO
            {
                CreatedDate = d.Key.CreatedDate.Date,
                RevaluationCount = d.Count()
            }).ToListAsync();

            return View(DatewiseRevaluationCountDTO);
        }
        [HttpGet]
        public async Task<IActionResult> SchemeWiseRevaluationCount()
        {
            var SchemeWiseRevaluationCountDTO = await _context.Photocopies.GroupBy(p => new { p.Scheme }).Select(d => new SchemeRevaluationCountDTO
            {
                Scheme = d.Key.Scheme,
                RevaluationCount = d.Count()
            }).ToListAsync();

            return View(SchemeWiseRevaluationCountDTO);
        }
        [HttpGet]
        public async Task<IActionResult> EventWiseRevaluationCount()
        {
            var EventWiseRevaluationCountDTO = await _context.Photocopies.GroupBy(static p => new { p.EventName }).Select(d => new EventRevaluationCountDTO
            {
                Event = d.Key.EventName,
                RevaluationCount = d.Count()
            }).ToListAsync();

            return View(EventWiseRevaluationCountDTO);
        }
        public async Task<IActionResult> ExportToExcelAsync(string ColumnName, string Value)
        {
            string Procedure = "EXEC GetAllRevaluations";

            // Fetch your data here, for example from a database
            IEnumerable<Revaluation> Revaluations = _context.Revaluations.FromSqlRaw(Procedure).ToList();

            byte[] content = await _exportDataService.ExportToExcelAsync<Revaluation>(Revaluations);

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TableData.xlsx");
        }

        //To export PDF
        //ExportDataToPdf using DinkToPdf nugate package using server side processing 
        //[FromBody] List<Photocopy> Photocopy
        [HttpGet]
        public async Task<IActionResult> ExportDataToPdf()
        {
            byte[] pdfBytes = await _exportDataService.ExportDataToPdfAsync();

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "TableData.pdf");
        }
        

        //// GET: Revaluations/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reValuation = await _context.reValuation
        //        .FirstOrDefaultAsync(m => m.SNO == id);
        //    if (reValuation == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(reValuation);
        //}

        //// GET: Revaluations/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Revaluations/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("SNO,EVENT_NAME,Scheme,Subject,SubPostCode,PinNumber,StudentName,OrderId,PGI_Ref_No,Amount,MobileNumber,Email,CreatedDate,IsPaid,Biller_Id")] ReValuation reValuation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(reValuation);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(CandidateWiseReValuation, Index));
        //    }
        //    return View(reValuation);
        //}

        //// GET: Revaluations/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reValuation = await _context.reValuation.FindAsync(id);
        //    if (reValuation == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(reValuation);
        //}

        //// POST: Revaluations/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("SNO,EVENT_NAME,Scheme,Subject,SubPostCode,PinNumber,StudentName,OrderId,PGI_Ref_No,Amount,MobileNumber,Email,CreatedDate,IsPaid,Biller_Id")] ReValuation reValuation)
        //{
        //    if (id != reValuation.SNO)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(reValuation);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ReValuationExists(reValuation.SNO))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(CandidateWiseReValuation, Index));
        //    }
        //    return View(reValuation);
        //}

        //// GET: Revaluations/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reValuation = await _context.reValuation
        //        .FirstOrDefaultAsync(m => m.SNO == id);
        //    if (reValuation == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(reValuation);
        //}

        //// POST: Revaluations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var reValuation = await _context.reValuation.FindAsync(id);
        //    if (reValuation != null)
        //    {
        //        _context.reValuation.Remove(reValuation);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(CandidateWiseReValuation, Index));
        //}

        //private bool ReValuationExists(int id)
        //{
        //    return _context.reValuation.Any(e => e.SNO == id);
        //}
    }
}
