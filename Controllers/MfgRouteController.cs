using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class MfgRouteController : Controller
    {
        private readonly MfgRouteBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public MfgRouteController(DataDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new MfgRouteBll(context, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: MfgRoute
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "MFG-ROUTE MASTER";
            return View(await _context.GetMfgRouteList());
        }

        // GET: MfgRoute/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New MFG-Route" : "Update MFG-Route";
            MfgRoute r = await _context.GetMfgRouteById(id);

            if (id == 0)
            {
                r.createdBy = user;
                r.updatedBy = user;
            }
            else
            {
                r.updatedBy = user;
            }

            return View(r);
        }

        // POST: MfgRoute/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, MfgRoute route)
        {
            ViewBag.PageTitle = id == 0 ? "New MFG-Route" : "Update MFG-Route";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateMfgRoute(id, route);
                if (success)
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoutes", await _context.GetMfgRouteList());
                else
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), route);
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), route);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Shipment
        public async Task<IActionResult> AddBulk()
        {
            ViewBag.PageTitle = "Bulk MFG-Route";
            BulkInsert b = new BulkInsert()
            {
                createdBy = user,
                updatedBy = user
            };
            return View(b);
        }

        // POST: Shipment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBulk(BulkInsert modal)
        {
            ViewBag.PageTitle = "Bulk MFG-Route";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                if (modal.dataFile.FileName.EndsWith(".xls") || modal.dataFile.FileName.EndsWith(".xlsx"))
                {
                    success = await _context.AddBulkMfgRoutes(modal, user);

                    if (success)
                        htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoutes", await _context.GetMfgRouteList());
                    else
                        htmlString = await _viewRender.RenderToStringAsync(nameof(AddBulk), modal);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddBulk), modal);
                    message = "Invalid FIle. Please Select file with extention .csv, .xls OR .xlsx";
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddBulk), modal);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // POST: MfgRoute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete MFG-Route";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                success = await _context.DeleteMfgRoute(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoutes", _context.GetMfgRouteList().Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getfreight")]
        public async Task<IActionResult> GetFreightDetails(string id, string quoteId=null, string vcNo=null)
        {
            ViewBag.PageTitle = "Get Freight";
            string message = string.Empty;
            try
            {
                MfgRoute route = await _context.GetMfgRouteByRoute(id, quoteId, vcNo);

                if (route != null)
                {
                    var freight = new
                    {
                        mfgRoute = route.mfgRoute,
                        basicFreight = route.basicRate,
                        enRoute = route.inroute,
                        totalFreight = route.totalExp
                    };
                    return Json(new { isValid = true, result = freight, message = message, source = ViewBag.PageTitle });
                }
                return Json(new { isValid = true, result = route, message = message, source = ViewBag.PageTitle });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }

    }
}
