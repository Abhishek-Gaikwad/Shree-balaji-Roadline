using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class DestinationController : Controller
    {
        private readonly DestinationBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public DestinationController(DataDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new DestinationBll(context, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Destination
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "DESTINATION MASTER";
            return View(await _context.GetDestinationList());
        }


        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Destination" : "Update Destination";
            Destination dst = await _context.GetDestinationById(id);

            dst.createdBy = String.IsNullOrEmpty(dst.createdBy) ? user : dst.createdBy;
            dst.updatedBy = user;

            return View(dst);
        }

        // POST: Destination/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Destination dst)
        {
            ViewBag.PageTitle = id == 0 ? "New Destination" : "Update Destination";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateDestination(id, dst);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllDestinations", _context.GetDestinationList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dst);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dst);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // POST: Destination/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Destination";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                res = await _context.DeleteDestination(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllDestinations", _context.GetDestinationList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getdestination")]
        public async Task<IActionResult> GetDestinationDetails(Int64 id = 0)
        {
            ViewBag.PageTitle = "Get Destination";
            string message = string.Empty;
            List<string> urls = new List<string>();
            try
            {
                Destination dest = await _context.GetDestinationById(id);

                if (dest != null)
                {
                    var destInfo = new
                    {
                        detsinationId = dest.detsinationId,
                        destination = dest.destination,
                        region = dest.region,
                        trasitDays = dest.trasitDays,
                        routeCode = dest.routeCode
                    };
                    return Json(new { isValid = true, result = destInfo, message = message, source = ViewBag.PageTitle });
                }
                return Json(new { isValid = true, result = dest, message = message, source = ViewBag.PageTitle });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }
    }
}
