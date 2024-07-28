using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class VcMasterController : Controller
    {
        private readonly VcMasterBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public VcMasterController(DataDbContext context, StoredProcedureDbContext spc, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new VcMasterBll(context, spc,env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: VcMasters
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "VC MASTER";
            return View(await _context.GetVcMasterList());
        }

        // GET: VcMasters/Edit/5
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New VC" : "Update VC";
            VcMaster vc = await _context.GetVcMasterById(id);
            if (id == 0)
            {
                vc.createdBy = user;
                vc.updatedBy = user;
            }
            else
            {
                vc.updatedBy = user;

            }

            return View(vc);
        }

        // POST: VcMasters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, VcMaster vc)
        {
            ViewBag.PageTitle = id == 0 ? "New VC" : "Update VC";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateVcMaster(id, vc);

                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllVcs", await _context.GetVcMasterList());
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), vc);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), vc);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: VcMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id)
        {
            ViewBag.PageTitle = "Delete VC";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.DeleteVcMaster(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllVcs", _context.GetVcMasterList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getvc")]
        public async Task<IActionResult> GetVcDetails(string id)
        {
            try
            {
                var vc = await _context.GetVcMasterByNo(null, id);

                return Json(new
                {
                    isValid = true,
                    result = vc.FirstOrDefault()
                });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message });
            }
        }

        [HttpPost, ActionName("getvcbyquote")]
        public async Task<IActionResult> GetVcDetails(string quoteId, string id)
        {
            try
            {
                var vc = await _context.GetVcMasterByNo(quoteId, id);

                return Json(new
                {
                    isValid = true,
                    result = vc.FirstOrDefault()
                });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message });
            }
        }

        [HttpPost, ActionName("getVcList")]
        public async Task<IActionResult> GetVcList(String quoteId = null)
        {
            try
            {
                var vc = await _context.GetVcMasterByNo(quoteId,null);

                return Json(new
                {
                    isValid = true,
                    result = vc
                });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message });
            }
        }

    }
}
