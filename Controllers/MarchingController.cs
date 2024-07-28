using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class MarchingController : Controller
    {
        private readonly MarchingBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly PumpMasterBll _pcontext;
        private readonly DriverBll _dcontext;
        private readonly IHostEnvironment _env;
        private readonly IViewRenderService _viewRender;
        private readonly IHttpContextAccessor _http;
        private readonly string user;
        public MarchingController(DataDbContext context,StoredProcedureDbContext spcontext,IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new MarchingBll(context, spcontext,env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _pcontext = new PumpMasterBll(context, env);
            _dcontext = new DriverBll(context, env);
            _viewRender = renderService;
            _env = env;
            _http = httpContext;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Marching
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "MARCHING";
            var list = await _context.GetMarchingGridList();
            return View(list);
        }


        // GET: Marching/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Marching" : "Update Marching";
            var result = await _context.GetMarchingDetail(id);
            Marching mrc = result.Item1;
            ViewBag.errorMessage = result.Item2;

            if (id == 0)
            {
                mrc.createdBy = user;
                mrc.updatedBy = user;
            }
            else
            {
                if (string.IsNullOrEmpty(mrc.createdBy))
                    mrc.createdBy = user;

                mrc.updatedBy = user;
            }

            mrc.pumpList = await DataLoader.GetPumpDropDown(_pcontext);
            mrc.loadingCharges = await DataLoader.GetLoadingChargesDropDown();
            mrc.driverList = await DataLoader.GetDriverDropDown(_dcontext);

            return View(mrc);
        }

        // POST: Marching/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Marching mrc)
        {
            ViewBag.PageTitle = id == 0 ? "New Marching" : "Update Marching";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            mrc.pumpList = await DataLoader.GetPumpDropDown(_pcontext);
            mrc.loadingCharges = await DataLoader.GetLoadingChargesDropDown();
            mrc.driverList = await DataLoader.GetDriverDropDown(_dcontext);

            if (mrc.extraAmt > 0 && string.IsNullOrEmpty(mrc.remark))
            {
                message = !string.IsNullOrEmpty(mrc.remark) ? null : "Please provide expenses detail";
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);
            }
            if (string.IsNullOrEmpty(mrc.tempRegNo))
            {
                message = !string.IsNullOrEmpty(mrc.remark) ? null : "Please provide Temp.Reg.No";
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);
            }
            DateTime currentDate = DateTime.Now;
            if (mrc.licenseExpDate <= currentDate)
            {
                message = !string.IsNullOrEmpty(mrc.remark) ? null : "Driver License Expired";
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    mrc.extraAmt = mrc.extraAmt == null ? 0 : mrc.extraAmt.Value;
                    mrc.expenses = mrc.expenses == null ? 0 : mrc.expenses.Value;
                    var tupple = await _context.AddOrUpdateMarching(mrc);
                    success = tupple.Item1;
                    if (success)
                    {
                        htmlString = await _viewRender.RenderToStringAsync("_ViewAllMarchings", _context.GetMarchingGridList().Result);
                        message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Marching/print/{tupple.Item2}";
                    }
                    else
                        htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);
                }
                catch (Exception)
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), mrc);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Marching/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id)
        {
            ViewBag.PageTitle = "Delete Marching";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.DeleteMarchingDetail(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllMarchings", _context.GetMarchingGridList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Marching/Delete/5
        [HttpPost, ActionName("pushback")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PushBackToPlantout(Int64 id)
        {
            ViewBag.PageTitle = "Pushback Marching";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.ShipmentPushback(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllMarchings", _context.GetMarchingGridList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
       
        // GET: Incidence/Report/5
        [HttpGet, ActionName("print")]
        public async Task<IActionResult> Report(Int64 id)
        {
            ViewBag.PageTitle = "Print";

            var marching = await _context.GetMarchingByVoucherNo(id);
            if (marching == null)
            {
                return NotFound();
            }
            return View(marching);
        }
    }
}
