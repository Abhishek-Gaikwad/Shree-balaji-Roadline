using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly BillingBll _context;
        private readonly DataDbContext _dbcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        private readonly IHttpContextAccessor _http;
        public BillingController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor http, IHostEnvironment env)
        {
            _dbcontext = context;
            _context = new BillingBll(context, spcontext, http,env);
            _viewRender = renderService;
            _env = env;
            _http = http;
            user = http.HttpContext.Session.GetString("username");
        }

        #region "BILLING"
        // GET: Billing
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "BILLING";
            var list = await _context.GetBillingList();
            return View(list);
        }

        // GET: Billing/Edit/5
        public async Task<IActionResult> AddOrEdit()
        {
            ViewBag.PageTitle = "New Billing";
            BillingModel b = await _context.GetBillingById();
            b.billingDate = DateTime.Now.Date;
            b.createdBy = String.IsNullOrEmpty(b.createdBy) ? user : b.createdBy;
            b.updatedBy = user;
            //b.companyList = DataLoader.GetCompanyDropdown(_dbcontext);
            b.shipmentList = await _context.GetShipmentsList();

            return View(b);
        }

        // POST: Billing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, BillingModel b)
        {
            ViewBag.PageTitle = id == 0 ? "New Billing" : "Update Billing";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields

            message = $"{ModelState.IsValid}";
            if (ModelState.IsValid)
            {
                var tupple = await _context.AddOrUpdateBilling(id, b);
                success = tupple.Item1;
                message = $"{tupple.Item1} :{tupple.Item2}";
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Billing/export/{tupple.Item2}";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), b);
                }
                //htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), b);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Billing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id)
        {
            ViewBag.PageTitle = "Delete Bill";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                res = await _context.DeleteBilling(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle }); ;
        }

        [HttpGet, ActionName("export")]
        public async Task<IActionResult> ExportReport(Int64 id)
        {
            if (id > 0)
            {
                var stream = await _context.GetBillingData(id);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Billing-{id}.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpGet, ActionName("exportirn")]
        public async Task<IActionResult> ExportIRNReport(Int64 id)
        {
            if (id > 0)
            {
                var stream = await _context.GetBillingIrnData(id);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UPDATE-AMOUNT-{id}.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        // GET: Incidence/Edit/5
        [HttpGet, ActionName("updateinvoice")]
        public async Task<IActionResult> UpdateIrnReceivedAmount()
        {
            ViewBag.PageTitle = "Update Received Amount";
            ReportModel<Billing> model = new ReportModel<Billing>();
            model.destinations = await DataLoader.GetBillReferenceDropDown(_context);
            return View(model);
        }

        [HttpPost, ActionName("updateinvoice")]
        // GET: Incidence/Edit/5
        public async Task<IActionResult> UpdateIrnReceivedAmount(ReportModel<Billing> model)
        {
            ViewBag.PageTitle = "Update Received Amount";
            ReportModel<Billing> reportModel = new ReportModel<Billing>();
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                if (model.dataFile.FileName.EndsWith(".xls") || model.dataFile.FileName.EndsWith(".xlsx"))
                {
                    success = await _context.UpdateIRNReceivedAmount(model, user);
                }
                else
                    message = "Invalid FIle. Please Select file with extention .csv, .xls OR .xlsx";
            }

            if (success)
                htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);

            else
            {
                model.destinations = await DataLoader.GetBillReferenceDropDown(_context);
                htmlString = await _viewRender.RenderToStringAsync(nameof(UpdateIrnReceivedAmount), model);
            }

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
        #endregion "BILLING"

        [HttpGet, ActionName("exportirns")]
        public async Task<IActionResult> ExportIRNSReport(Int64 id)
        {
            if (id > 0)
            {
                var stream = await _context.GetBillingIrnsData(id);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"UPDATE-IRNS-{id}.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpGet, ActionName("updateirn")]
        //GET: Incidence/Edit/5
        public async Task<IActionResult> UpdateIrn()
        {
            ViewBag.PageTitle = "Update IRN";
            ReportModel<Billing> model = new ReportModel<Billing>();
            model.destinations = await DataLoader.GetBillReferenceDropDown(_context);
            return View(model);
        }

        [HttpPost, ActionName("updateirn")]
        //GET: update IRN/Edit/5
        public async Task<IActionResult> UpdateIrn(ReportModel<Billing> model)
        {
            ViewBag.PageTitle = "Update IRN";
            ReportModel<Billing> reportModel = new ReportModel<Billing>();
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                if (model.dataFile.FileName.EndsWith(".xls") || model.dataFile.FileName.EndsWith(".xlsx"))
                {
                    success = await _context.UpdateIRN(model, user);
                }
                else
                    message = "Invalid FIle. Please Select file with extention .csv, .xls OR .xlsx";
            }

            if (success)
                htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);

            else
            {
                model.destinations = await DataLoader.GetBillReferenceDropDown(_context);
                htmlString = await _viewRender.RenderToStringAsync(nameof(UpdateIrn), model);
            }

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        #region "FBV BILLING"
        // GET: FBV Billing
        [HttpGet, ActionName("fbvindex")]
        public async Task<IActionResult> FbvIndex()
        {
            ReportModel<FbvBilling> model = new ReportModel<FbvBilling>();
            model.fromDate = DateTime.Now.Date;
            model.toDate = DateTime.Now.Date;
            ViewBag.PageTitle = "FBV Billing";
            var list = await _context.GetFBVBillingList(model);
            model.data = list;
            return View(model);
        }

        // POST: FBV Billing
        [HttpPost, ActionName("fbvindex")]
        public async Task<IActionResult> FbvIndex(ReportModel<FbvBilling> model)
        {
            ReportModel<FbvBilling> rptModel = new ReportModel<FbvBilling>();
            rptModel.fromDate = model.fromDate;
            rptModel.toDate = model.toDate;
            //from date in session
            _http.HttpContext.Session.SetString("fromDate", model.fromDate.Value.ToString("yyyy-MM-dd"));
            //to date in session
            _http.HttpContext.Session.SetString("toDate", model.toDate.Value.ToString("yyyy-MM-dd"));
            ViewBag.PageTitle = "FBV Billing";
            var list = await _context.GetFBVBillingList(model);
            rptModel.data = list;
            return View(rptModel);
        }

        // GET: Generate FBV Bill
        [HttpGet, ActionName("fbv-billing")]
        public async Task<IActionResult> FbvBilling(Int64 id = 0, Int64 shipmentId = 0)
        {
            ViewBag.PageTitle = "New FBV Billing";
            FbvBillingModel b = new FbvBillingModel();
            b = await _context.getFBVShipment(shipmentId, id);
            return View(b);
        }

        // POST: Generate FBV Billing
        [HttpPost, ActionName("fbv-billing")]
        public async Task<IActionResult> FbvBilling(Int64 id, Int64 shipmentId, FbvBillingModel model)
        {
            ReportModel<FbvBilling> rptModel = new ReportModel<FbvBilling>();

            ViewBag.PageTitle = id == 0 ? "New FBV Billing" : "Update FBV Billing";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields

            message = $"{ModelState.IsValid}";
            if (ModelState.IsValid)
            {
                var tupple = await _context.AddOrUpdateFBVBilling(id, shipmentId, model, user);
                success = tupple.Item1;
                //message = $"{tupple.Item1} :{tupple.Item2}";
                if (success)
                {
                    rptModel.fromDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("fromDate"));
                    rptModel.toDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("toDate"));
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllFbvBills", _context.GetFBVBillingList(rptModel).Result);
                    message = (!string.IsNullOrEmpty(tupple.Item2)) ?
                        $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Billing/fbv-print/{tupple.Item2}" :
                        "";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(FbvBilling), model);
                }
                //htmlString = await _viewRender.RenderToStringAsync("_ViewAllBills", _context.GetBillingList().Result);
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(FbvBilling), model);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // GET: Generate FBV Bill
        [HttpGet, ActionName("fbv-print")]
        public async Task<IActionResult> FbvPrint(Int64 id)
        {
            //  ViewBag.PageTitle = "FBV Print";
            FbvInvoice b = new FbvInvoice();
            b = await _context.GetFbvBillingInvoice(id);
            return View(b);
        }
        #endregion "FBV BILLING"

        #region "QUOTATION / IB BILLING [ PERFORMA | INVOICE ]"
        // GET: IB Billing
        [HttpGet, ActionName("ibindex")]
        public async Task<IActionResult> IbIndex()
        {
            ViewBag.PageTitle = "IB Billing";
            ReportModel<IbBilling> model = new ReportModel<IbBilling>();
            model.fromDate = DateTime.Now.Date;
            model.toDate = DateTime.Now.Date;
            var list = await _context.GetIbBillingList(model);
            model.data = list;
            return View(model);
        }

        // POST: FBV Billing
        [HttpPost, ActionName("ibindex")]
        public async Task<IActionResult> IbIndex(ReportModel<IbBilling> model)
        {
            ViewBag.PageTitle = "IB Billing";
            ReportModel<IbBilling> rptModel = new ReportModel<IbBilling>();
            rptModel.fromDate = model.fromDate;
            rptModel.toDate = model.toDate;
            //from date in session
            _http.HttpContext.Session.SetString("fromDate", model.fromDate.Value.ToString("yyyy-MM-dd"));
            //to date in session
            _http.HttpContext.Session.SetString("toDate", model.toDate.Value.ToString("yyyy-MM-dd"));
            var list = await _context.GetIbBillingList(model);
            rptModel.data = list;
            return View(rptModel);
        }


        [HttpGet, ActionName("generate")]
        public async Task<IActionResult> GenerateBill()
        {
            ViewBag.PageTitle = "Create Billing";
            IbBillingModel b =  await _context.GetIbBillingById();
            return View(b);
        }

        // POST: Marching/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("generate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBill(IbBillingModel b)
        {
            ViewBag.PageTitle = "Create Billing";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            message = $"{ModelState.IsValid}";
            if (ModelState.IsValid)
            {
                var tupple = await _context.generateIbBilling(b);
                success = tupple.errorCode != 1 ? false : true;
                if (success)
                {            //Initialize default fields
                    ReportModel<IbBilling> rptModel = new ReportModel<IbBilling>();
                    rptModel.fromDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("fromDate"));
                    rptModel.toDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("toDate"));
                    var list = await _context.GetIbBillingList(rptModel);

                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllIbBills", list);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Billing/print-invoice-1?id={tupple.id}&type=performa";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(GenerateBill), b);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(GenerateBill), b);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpGet, ActionName("update")]
        public async Task<IActionResult> updateBill(string id)
        {
            ViewBag.PageTitle = "Update Billing";

            IbBillingModel b = await _context.GetIbBillingById(id);

            return View(b);
        }

        // POST: Marching/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateBill(IbBillingModel b)
        {
            ViewBag.PageTitle = "Update Billing";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields

            message = $"{ModelState.IsValid}";
            if (ModelState.IsValid)
            {
                var tupple = await _context.updateIbBill(b);
                success = tupple.errorCode != 1 ? false : true;
                if (success)
                {
                    ReportModel<IbBilling> rptModel = new ReportModel<IbBilling>();
                    rptModel.fromDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("fromDate"));
                    rptModel.toDate = Convert.ToDateTime(_http.HttpContext.Session.GetString("toDate"));
                    var list = await _context.GetIbBillingList(rptModel);

                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllIbBills", list);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Billing/print-invoice-1?id={tupple.id}&type=invoice";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(updateBill), b);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(updateBill), b);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        [HttpPost, ActionName("getibshipment")]
        public async Task<IActionResult> getShipmetListByQuote(string id)
        {
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            var tupple = await _context.getShipmetListByQuote(id);
            success = tupple.Item1;
            message = success ? tupple.Item3 : tupple.Item2;

            return Json(new { isValid = success, html = tupple.Item2, message = message, source = ViewBag.PageTitle });
        }


        [HttpGet, ActionName("print-invoice-1")]
        public async Task<IActionResult> ReportPrint1(string id, string type)
        {
            ViewBag.PageTitle = type.Equals("performa") ? "Performa Invoice" : "Invoice";

            var vc = await _context.GetIbBillingInvoice(id, type);
            if (vc == null)
            {
                return NotFound();
            }
            return View(vc);
        }

        [HttpGet, ActionName("print-invoice-2")]
        public async Task<IActionResult> ReportPrint2(string id, string type)
        {
            ViewBag.PageTitle = type.Equals("performa") ? "Performa Invoice" : "Invoice";

            var vc = await _context.GetIbBillingInvoice(id, type, 2);
            if (vc == null)
            {
                return NotFound();
            }
            return View(vc);
        }

        [HttpGet, ActionName("export-ib")]
        public async Task<IActionResult> ExportIbReport(Int64 id)
        {
            if (id > 0)
            {
                var stream = await _context.GetBillingData(id);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Billing-{id}.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        #endregion "QUOTATION / IB BILLING [ PERFORMA | INVOICE ]"
    }
}
