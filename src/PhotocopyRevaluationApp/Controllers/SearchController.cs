using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotocopyRevaluationApp.Data;
using System.Linq.Expressions;

namespace PhotocopyRevaluationApp.Controllers {
    public class SearchController : Controller {
        private readonly PhotocopyRevaluationAppContext _context;
        public SearchController(PhotocopyRevaluationAppContext context) {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query) {
            if (string.IsNullOrEmpty(query))
                return Json(new List<string>());

            // Use parameterized queries to prevent SQL Injection
            var results = await _context.Users
                .Where(i => EF.Functions.Like(i.UserName, $"%{query}%"))
                .Select(i => i.UserName)
                .ToListAsync();

            //// Use parameterized queries to prevent SQL Injection
            //var results = await _context.Photocopies
            //    .Where(i => EF.Functions.Like($"i.{ColumnName}", $"%{Value}%"))
            //.Select(i => new
            //{
            //        ColumnName = $"i.Key.{ColumnName}",
            //        Count = i.Count()

            //    })
            //    .ToListAsync();

            //var parameter = Expression.Parameter(typeof(Photocopy), "i");
            //var property = Expression.Property(parameter, ColumnName); // Get the property dynamically
            //var likeMethod = typeof(EF).GetMethod(nameof(EF.Functions.Like), new[] { typeof(string), typeof(string) });

            //if (likeMethod != null)
            //{
            //    var likeExpression = Expression.Call(
            //        null,
            //        likeMethod,
            //        Expression.Constant(EF.Functions),
            //        property,
            //        Expression.Constant($"%{Value}%"));

            //    var lambda = Expression.Lambda<Func<Photocopy, bool>>(likeExpression, parameter);

            //    var results = await _context.Photocopies
            //        .Where(lambda)
            //        .Select(i => new
            //        {
            //            ColumnName = EF.Property<string>(i, ColumnName),
            //            Count = 1  // Since you want to count, we can return a constant value and later sum it
            //        })
            //        .GroupBy(x => x.ColumnName)
            //        .Select(g => new
            //        {
            //            ColumnName = g.Key,
            //            Count = g.Count()
            //        })
            //        .ToListAsync();
            //}
            return Json(results);
        }
    }
}
