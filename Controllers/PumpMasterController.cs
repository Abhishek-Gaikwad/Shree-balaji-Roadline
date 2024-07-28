using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class PumpMasterController : Controller
    {
        private readonly PumpMasterBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public PumpMasterController(DataDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new PumpMasterBll(context, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: PumpMaster
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "PUMP MASTER";
            return View(await _context.GetPumpList());
        }

        // GET: PumpMaster/Edit/5
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Pump" : "Update Pump";
            PumpMaster pm = await _context.GetPumpById(id);
            if (id == 0)
            {
                pm.createdBy = user;
                pm.updatedBy = user;
            }
            else
            {
                pm.updatedBy = user;
            }

            return View(pm);

        }

        // POST: PumpMaster/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, PumpMaster pm)
        {
            ViewBag.PageTitle = id == 0 ? "New Pump" : "Update Pump";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields

            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdatePump(id, pm);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllPumps", _context.GetPumpList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), pm);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), pm);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // POST: PumpMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Pump";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.DeletePump(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllPumps", _context.GetPumpList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getpump")]
        public async Task<IActionResult> GetPumpDetails(Int64 id = 0)
        {
            ViewBag.PageTitle = "Get Pump";
            string message = string.Empty;
            try
            {
                PumpMaster pm = await _context.GetPumpById(id);

                if (pm != null)
                {
                    var destInfo = new
                    {
                        pumpId = pm.pumpId,
                        pumpName = pm.pumpName,
                        rate = pm.rate
                    };
                    return Json(new { isValid = true, result = destInfo });
                }
                return Json(new { isValid = true, result = pm, message = message, source = ViewBag.PageTitle });

            }
            catch (Exception e)
            {
                throw;
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }

    }
}
