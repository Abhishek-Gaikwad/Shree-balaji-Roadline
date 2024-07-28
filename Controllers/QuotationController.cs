using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        private readonly QuotationBll _quote;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;

        public QuotationController(DataDbContext context, StoredProcedureDbContext spc, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _viewRender = renderService;
            _env = env;
            _quote = new QuotationBll(context, spc, httpContext, env);
        }

        // GET: Quotation
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "QUOTATION";
            List<Quotes> quoteList = await _quote.GetQuotationList();
            return View(quoteList);
        }

        // GET: Quotation/Edit/5
        public async Task<IActionResult> AddQuote(string? quoteId = null)
        {
            ViewBag.PageTitle = "New Quotation";
            QuoteDto q = await _quote.GetQuotationById(quoteId);
            return View(q);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuote(QuoteDto dt)
        {
            ViewBag.PageTitle = "New Quotation";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                var result = await _quote.Insert(dt);
                success = result.errorCode == 1 ? true : false;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllQuotes", _quote.GetQuotationList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddQuote), dt);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddQuote), dt);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Quotation/Edit/5
        public async Task<IActionResult> EditQuote(string? quoteId = null)
        {
            ViewBag.PageTitle = "Update Quotation";
            QuoteDto q = await _quote.GetQuotationById(quoteId);
            return View(q);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuote(QuoteDto dt)
        {
            ViewBag.PageTitle = "Update Quotation";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                var result = await _quote.Update(dt);
                success = result.errorCode == 1 ? true : false;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllQuotes", _quote.GetQuotationList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(EditQuote), dt);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(EditQuote), dt);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditDetail(QuoteDetailDto dt)
        {
            bool success = false;
            int statusCode = 300;
            string message = string.Empty;
            var tupple = await _quote.InsertDetails(dt);
            success = tupple.Item1;
            if (success)
            {
                statusCode = 200;
                message = "success";
            }
            return Json(new { success = success, status = 200, Message = "", srNo = tupple.Item2 });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDetail(string id)
        {
            bool success = false;
            int statusCode = 300;
            string message = string.Empty;
            var tupple = await _quote.Delete(id);
            success = tupple.Item1;
            if (success)
            {
                statusCode = 200;
                message = "success";
            }
            return Json(new { success = success, status = 200, Message = message });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCache(string id)
        {
            bool success = false;
            int statusCode = 300;
            string message = string.Empty;
            var result = await _quote.RemoveCache(id);
            success = result.errorCode == 1 ? true : false;
            if (success)
            {
                statusCode = 200;
                message = "success";
            }
            return Json(new { success = success, status = 200, Message = message });
        }

        [HttpGet, ActionName("export")]
        public async Task<IActionResult> ExportReport(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var stream = await _quote.GetQuotationExcel(id);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Quotation-{id}.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        // GET: Incidence/Report/5
        [HttpGet, ActionName("print")]
        public async Task<IActionResult> Report(string id)
        {
            ViewBag.PageTitle = "Print";

            var q = await _quote.GetQuoteById(id);
            if (q == null)
            {
                return NotFound();
            }
            return View(q);
        }

        // POST: MfgRoute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ViewBag.PageTitle = "Delete Quotation";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                success = await _quote.DeleteQuotation(id);
            }
            List<Quotes> quotationList = await _quote.GetQuotationList();
            htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoutes", quotationList);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


    }
}
