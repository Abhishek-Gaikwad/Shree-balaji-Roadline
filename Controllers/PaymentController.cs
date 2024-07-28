using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly PaymentBll _pcontext;
        private readonly MarchingBll _mcontext;
        private readonly DataDbContext _context;
        private readonly PumpMasterBll _pmcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private readonly string user;

        public PaymentController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = context;
            _pcontext = new PaymentBll(context, spcontext, env);
            _mcontext = new MarchingBll(context, spcontext, env);
            _pmcontext = new PumpMasterBll(context, env);
            _viewRender = renderService;
            _env = env;
            _http = httpContext;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        #region "DRIVER PAYMENT"

        // GET: Payment
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "PAYMENT";
            return View(await _pcontext.GetInchargeVoucherList());
        }

        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Voucher" : "Update Voucher";
            InchargePayment vc = new InchargePayment();
            DateTime dt = DateTime.Now;
            if (id == 0)
            {
                vc.createdBy = user;
                vc.createdDate = dt;
            }
            vc.voucherDate = dt.Date;
            vc.updatedBy = user;
            vc.updatedDate = dt;

            vc.inchargeList = await DataLoader.GetDriverInchargeDropDown(_context);

            return View(vc);
        }

        // POST: VcMasters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int voucherId, InchargePayment vc)
        {
            ViewBag.PageTitle = voucherId == 0 ? "New Voucher" : "Update Voucher";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                var tupple = await _pcontext.AddOrUpdateInchargeVoucher(vc);
                success = tupple.Item1;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllVouchers", _pcontext.GetInchargeVoucherList().Result);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Payment/print/{tupple.Item2}";
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

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Voucher";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                res = await _pcontext.DeleteVoucher(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllVouchers", _pcontext.GetVoucherList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getvoucher")]
        public async Task<IActionResult> GetVoucherDetails(Int64 id)
        {
            ViewBag.PageTitle = "Get Voucher";
            string vclist = string.Empty;
            string message = string.Empty;
            try
            {
                InchargePayment m = await _pcontext.GetPayableByVoucherNo(id);

                if (m != null)
                {
                    var destInfo = new
                    {
                        balance = m.balanceAmount
                    };
                    return Json(new { isValid = true, result = destInfo });
                }
                return Json(new { isValid = true, result = m, message = message, source = ViewBag.PageTitle });

            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }

        // GET: Incidence/Report/5
        [HttpGet, ActionName("print")]
        public async Task<IActionResult> Report(Int64 id)
        {
            ViewBag.PageTitle = "Print";

            var vc = await _pcontext.GetInchargeVoucherInfo(id);
            if (vc == null)
            {
                return NotFound();
            }
            return View(vc);
        }

        #endregion "DRIVER PAYMENT"

        #region " DIESEL PAYMENT"

        // GET: Payment/diesel-payment
        [HttpGet, ActionName("diesel-payment")]
        public async Task<IActionResult> DieselPaymentIndex()
        {
            ViewBag.PageTitle = "DIESEL PAYMENT";
            return View(await _pcontext.GetDieselPaymentVouchers(true));
        }


        // GET: Payment/AddOrEditDieselPayment/5
        public async Task<IActionResult> AddOrEditDieselPayment()
        {
            DieselRequestModel<Diesel> model = new DieselRequestModel<Diesel>();
            model.status = 0;
            ViewBag.PageTitle = "DIESEL PAYMENT";
            model.data = new List<Diesel>();
            model.ddl = await DataLoader.GetPumpDropDown(_pmcontext);
            return View(model);
        }

        // POST: Payment/AddOrEditDieselPayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditDieselPayment(DieselRequestModel<Diesel> model, long id = 0)
        {
            ViewBag.PageTitle = "DIESEL PAYMENT";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            DieselRequestModel<Diesel> reportModel = new DieselRequestModel<Diesel>();
            if (ModelState.IsValid)
            {
                success = await _pcontext.CreateOrEditDieselBilllPayment(model, user);

                if (success)
                {
                    ViewData["dpr"] = null;
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllDieselPaymentVouchers", await _pcontext.GetDieselPaymentVouchers(true));
                }
                else
                {
                    reportModel.fromDate = model.fromDate;
                    reportModel.toDate = model.toDate;
                    reportModel.pumpId = model.pumpId;
                    reportModel.data = await _pcontext.GetDieselReport(model);
                    reportModel.ddl = await DataLoader.GetPumpDropDown(_pmcontext);

                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditDieselPayment), reportModel);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditDieselPayment), reportModel);

            return Json(new { isValid = success, html = htmlString, source = ViewBag.PageTitle });
        }

        // POST: Payment/AddOrEditDieselVoucher/5
        public async Task<IActionResult> AddOrEditDieselVoucher(long id)
        {
            ViewBag.PageTitle = "PAYMENT VOUCHER";
            DieselPayment model = new DieselPayment();
            model = await _pcontext.GetDieselVoucherById(id);
            return View(model);
        }

        // POST: Payment/AddOrEditDieselVoucher/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditDieselVoucher(DieselPayment model, long id = 0)
        {
            ViewBag.PageTitle = "DIESEL PAYMENT";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                success = await _pcontext.CreateDieselPaymentVoucher(model, user);

                if (success)
                {
                    ViewData["dpr"] = null;
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllDieselPaymentVouchers", await _pcontext.GetDieselPaymentVouchers(true));
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditDieselPayment), model);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditDieselPayment), model);

            return Json(new { isValid = success, html = htmlString, source = ViewBag.PageTitle });
        }

        // POST: Payment/AddOrEditDieselVoucher/5
        public async Task<IActionResult> ViewDieselReportByPump(long id)
        {
            ViewBag.PageTitle = "LEGDER";
            ViewData["dpr"] = "true";
            string pumpName = string.Empty;
            List<DieselPayment> model = _pcontext.GetDieselVoucherByPumpId(id, out pumpName);
            ViewBag.PageTitle = $"LEGDER: {pumpName}";
            return View(model);
        }

        [HttpPost]
        //public async Task<IActionResult> GetDieselVoucherList(DieselVoucherModel model)
        public async Task<IActionResult> GetDieselVoucherList(DieselRequestModel<Diesel> model)
        {
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                htmlString = await _viewRender.RenderToStringAsync("_tableDieselBody", _pcontext.GetDieselReport(model).Result);
                success = true;
            }

            return Json(new { isValid = success, html = htmlString });
        }

        [HttpPost]
        public async Task<IActionResult> getDieselTotalByIds([FromBody] CalculateDiesel model)
        {
            bool success = false;
            string htmlString = string.Empty;
            DieselPaymentCalc list = new DieselPaymentCalc();
            if (ModelState.IsValid)
            {
                if (model.vouchers.Count() > 0)
                {
                    list = await _pcontext.GetTotalDieselPayment(model);
                    success = true;
                }
            }
            return Json(new { isValid = success, data = list });
        }
        #endregion " DIESEL PAYMENT"
    }
}