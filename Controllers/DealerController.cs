using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class DealerController : Controller
    {
        private readonly DealerBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly string user;

        public DealerController(DataDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext)
        {
            _context = new DealerBll(context);
            _viewRender = renderService;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Dealer
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "DEALER MASTER";
            return View(await _context.GetDealerList());
        }

        // GET: Dealer/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Dealer" : "Update Dealer";
            Dealer dlr = await _context.GetDealerById(id);

            dlr.createdBy = String.IsNullOrEmpty(dlr.createdBy) ? user : dlr.createdBy;
            dlr.updatedBy = user;

            dlr.dealerTypes = await DataLoader.GetDealerTypeDropDown();

            return View(dlr);
        }

        // POST: Dealer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Dealer dlr)
        {
            ViewBag.PageTitle = id == 0 ? "New Dealer" : "Update Dealer";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateDealer(id, dlr);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllDealers", _context.GetDealerList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dlr);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dlr);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Dealer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Dealer";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                success = await _context.DeleteDealer(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllDealers", _context.GetDealerList().Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getdealer")]
        public async Task<IActionResult> GetDealerDetails(Int64 id = 0)
        {
            ViewBag.PageTitle = "Get Dealer";
            string message = string.Empty;
            List<string> urls = new List<string>();
            try
            {
                Dealer dlr = await _context.GetDealerById(id);

                if (dlr != null)
                {
                    var dlrInfo = new
                    {
                        dealerId = dlr.dealerId,
                        dealerName = dlr.dealerName,
                        city = dlr.city
                    };
                    return Json(new { isValid = true, result = dlrInfo, message = message, source = ViewBag.PageTitle });
                }
                return Json(new { isValid = true, result = dlr, message = message, source = ViewBag.PageTitle });

            }
            catch (Exception e)
            {
                throw;
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }
    }
}
