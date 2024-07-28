using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ReportBll _context;
        private readonly BillingBll _bcontext;
        private readonly DriverBll _dcontext;
        private readonly PumpMasterBll _pcontext;
        private readonly DataDbContext _dbcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public ReportController(DataDbContext context, StoredProcedureDbContext spDbContext, IViewRenderService renderService, IHttpContextAccessor http, IHostEnvironment env)
        {
            _context = new ReportBll(context, spDbContext, env);
            _dcontext = new DriverBll(context, env);
            _bcontext = new BillingBll(context, spDbContext, http, env);
            _pcontext = new PumpMasterBll(context, env);
            _dbcontext = context;
            _viewRender = renderService;
            _env = env;
            user = http.HttpContext.Session.GetString("username");
        }


        // GET: Report/billing-report
        [HttpGet, ActionName("billing-report")]
        public async Task<IActionResult> BillingReport()
        {
            ReportModel<BillingReport> model = new ReportModel<BillingReport>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "BILLING REPORT";
            model.companyId = 1;
            model.data = await _context.GetBillingList(model);
            model.destinations = await DataLoader.GetBillReferenceDropDown(_bcontext);
            return View(model);
        }

        [HttpPost, ActionName("billing-report")]
        // POST: Report/billing-report/5
        public async Task<IActionResult> BillingReport(ReportModel<BillingReport> model)
        {
            ReportModel<BillingReport> rptModel = new ReportModel<BillingReport>();
            ViewBag.PageTitle = "BILLING REPORT";
            if (ModelState.IsValid)
            {
                model.companyId = 1;
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.status = model.status;
                rptModel.destinationId = model.destinationId;
                rptModel.companyId = model.companyId;
                rptModel.referenceNo = model.referenceNo;
                rptModel.data = await _context.GetBillingList(model);
            }

            rptModel.destinations = await DataLoader.GetBillReferenceDropDown(_bcontext);

            return View(rptModel);
        }

        // GET: Report/billing-report
        [HttpGet, ActionName("fbv-billing-report")]
        public async Task<IActionResult> FbvBillingReport()
        {
            ReportModel<BillingReport> model = new ReportModel<BillingReport>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "FBV BILLING REPORT";
            model.companyId = 2;
            model.data = await _context.GetFBVBillingList(model);
            return View(model);
        }

        [HttpPost, ActionName("fbv-billing-report")]
        // POST: Report/billing-report/5
        public async Task<IActionResult> FbvBillingReport(ReportModel<BillingReport> model)
        {
            ReportModel<BillingReport> rptModel = new ReportModel<BillingReport>();
            ViewBag.PageTitle = "FBV BILLING REPORT";
            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.status = model.status;
                rptModel.destinationId = model.destinationId;
                rptModel.referenceNo = model.referenceNo;
                rptModel.companyId = 2;
                rptModel.data = await _context.GetFBVBillingList(rptModel);
            }

            rptModel.destinations = await DataLoader.GetBillReferenceDropDown(_bcontext);

            return View(rptModel);
        }

        [HttpGet, ActionName("incidence-report")]
        // GET: Report/incidence-report
        public async Task<IActionResult> IncidenceReport()
        {
            ViewBag.PageTitle = "INCIDENCE REPORT";
            ReportModel<IncidenceReport> model = new ReportModel<IncidenceReport>();
            model.data = await _context.GetIncidenceList(model);
            return View(model);
        }

        [HttpPost, ActionName("incidence-report")]
        // POST: Report/incidence-report/5
        public async Task<IActionResult> IncidenceReport(ReportModel<IncidenceReport> model)
        {
            ReportModel<IncidenceReport> reportModel = new ReportModel<IncidenceReport>();
            ViewBag.PageTitle = "INCIDENCE REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;
                reportModel.status = model.status;
                reportModel.data = await _context.GetIncidenceList(model);
            }

            return View(reportModel);
        }

        [HttpGet, ActionName("shipment-report")]
        // GET: Report/shipment-report
        public async Task<IActionResult> ShipmentReport()
        {
            ReportModel<ShipmentReport> model = new ReportModel<ShipmentReport>();
            model.status = Actions.all.ToString();
            model.companyId = 1;
            model.destinationId = 0;
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "SHIPMENT REPORT";
            model.data = await _context.GetShipmentList(model);

            model.destinations = await DataLoader.GetDestinationDropDown(_dbcontext, "all");
            model.ddl = await DataLoader.GetShipmentStatusDropDown(Actions.all.ToString(), true);
            model.ddl2 = DataLoader.GetCompanyDropdown(_dbcontext);
            return View(model);
        }

        [HttpPost, ActionName("shipment-report")]
        // POST: Report/shipment-report/5
        public async Task<IActionResult> ShipmentReport(ReportModel<ShipmentReport> model)
        {
            ReportModel<ShipmentReport> rptModel = new ReportModel<ShipmentReport>();
            ViewBag.PageTitle = "SHIPMENT REPORT";
            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.status = model.status;
                rptModel.destinationId = model.destinationId;
                rptModel.companyId = model.companyId;
                rptModel.keyword = model.keyword;
                rptModel.data = await _context.GetShipmentList(model);
            }

            rptModel.ddl = await DataLoader.GetShipmentStatusDropDown(Actions.all.ToString(), true);
            rptModel.ddl2 = DataLoader.GetCompanyDropdown(_dbcontext);
            rptModel.destinations = await DataLoader.GetDestinationDropDown(_dbcontext, "all");

            return View(rptModel);
        }

        [HttpGet, ActionName("driver-report")]
        // GET: Report/driver-report
        public async Task<IActionResult> ReportList()
        {
            ReportModel<Driver> model = new ReportModel<Driver>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "DRIVER REPORT";
            model.data = await _context.GetDriverList(model);
            model.ddl = await DataLoader.GetDriverStatusDropDown(true);
            return View(model);
        }

        [HttpPost, ActionName("driver-report")]
        // POST: Report/driver-report/5
        public async Task<IActionResult> ReportList(ReportModel<Driver> model)
        {
            ReportModel<Driver> rptModel = new ReportModel<Driver>();
            ViewBag.PageTitle = "DRIVER REPORT";
            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.status = model.status;
                rptModel.data = await _context.GetDriverList(model);
            }

            rptModel.ddl = await DataLoader.GetDriverStatusDropDown(true);

            return View(rptModel);
        }

        [HttpGet, ActionName("diesel-report")]
        // GET: Report/diesel-report
        public async Task<IActionResult> DieselReport()
        {
            ReportModel<Diesel> model = new ReportModel<Diesel>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "DIESEL REPORT";
            model.data = await _context.GetDieselReport(model);
            model.ddl = await DataLoader.GetPumpDropDown(_pcontext, "all");
            return View(model);
        }

        [HttpPost, ActionName("diesel-report")]
        // POST: Report/diesel-report/5
        public async Task<IActionResult> DieselReport(ReportModel<Diesel> model)
        {
            ReportModel<Diesel> reportModel = new ReportModel<Diesel>();
            ViewBag.PageTitle = "DIESEL REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;
                reportModel.destinationId = model.destinationId;
                reportModel.data = await _context.GetDieselReport(model);
            }

            reportModel.ddl = await DataLoader.GetPumpDropDown(_pcontext, "all");

            return View(reportModel);
        }

        [HttpGet, ActionName("pl-report")]
        // GET: Report/pl-report
        public async Task<IActionResult> ProfitAndLossReport()
        {
            ViewBag.PageTitle = "PROFIT & LOSS REPORT";
            ReportModel<ProfitLoss> model = new ReportModel<ProfitLoss>();
            DateTime dt = DateTime.Now.Date;
            model.fromDate = dt;
            model.toDate = dt;
            return View(model);
        }

        [HttpPost, ActionName("pl-report")]
        // POST: Report/pl-report/5
        public async Task<IActionResult> ProfitAndLossReport(ReportModel<ProfitLoss> model)
        {
            ViewBag.PageTitle = "PROFIT & LOSS REPORT";
            ReportModel<ProfitLoss> reportModel = new ReportModel<ProfitLoss>();

            if (ModelState.IsValid)
            {
                reportModel.data = await _context.GetProfiLossReport(model);
            }
            reportModel.fromDate = model.fromDate;
            reportModel.toDate = model.toDate;
            reportModel.destinationId = model.destinationId;

            return View(reportModel);
        }

        [HttpGet, ActionName("payment-receipt-report")]
        // GET: Report/payment-receipt-report
        public async Task<IActionResult> CashReport()
        {
            ReportModel<Voucher> model = new ReportModel<Voucher>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "PAYMENT RECEIPT REPORT";
            model.data = await _context.GetCashReport(model);
            model.ddl = await DataLoader.GetPayModeDropDown();
            return View(model);
        }

        [HttpPost, ActionName("payment-receipt-report")]
        // POST: Report/payment-receipt-report/5
        public async Task<IActionResult> CashReport(ReportModel<Voucher> model)
        {
            ReportModel<Voucher> reportModel = new ReportModel<Voucher>();
            ViewBag.PageTitle = "PAYMENT RECEIPT REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;
                reportModel.status = model.status;
                reportModel.data = await _context.GetCashReport(model);
            }

            reportModel.ddl = await DataLoader.GetPayModeDropDown();

            return View(reportModel);
        }

        [HttpGet, ActionName("authority-report")]
        // GET: Report/authority-report
        public async Task<IActionResult> AuthorityReport()
        {
            ReportModel<Authority> model = new ReportModel<Authority>();
            ViewBag.PageTitle = "AUTHORITY REPORT";
            model.data = await _context.GetAuthorityList(model);
            return View(model);
        }

        [HttpPost, ActionName("authority-report")]
        // POST: Report/authority-report/5
        public async Task<IActionResult> AuthorityReport(ReportModel<Authority> model)
        {
            ReportModel<Authority> reportModel = new ReportModel<Authority>();
            ViewBag.PageTitle = "AUTHORITY REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;
                reportModel.status = model.status;
                reportModel.data = await _context.GetAuthorityList(model);
            }

            return View(reportModel);
        }

        [HttpGet, ActionName("voucher-report")]
        // GET: Report/voucher-report
        public async Task<IActionResult> VoucherReport()
        {
            ReportModel<Voucher> model = new ReportModel<Voucher>();
            model.status = Actions.all.ToString();
            ViewBag.PageTitle = "VOUCHER REPORT";
            model.data = await _context.GetVoucherList(model);
            return View(model);
        }

        [HttpPost, ActionName("voucher-report")]
        // POST: Report/voucher-report/5
        public async Task<IActionResult> VoucherReport(ReportModel<Voucher> model)
        {
            ReportModel<Voucher> reportModel = new ReportModel<Voucher>();
            ViewBag.PageTitle = "VOUCHER REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;
                reportModel.status = model.status;
                reportModel.data = await _context.GetVoucherList(model);
            }

            return View(reportModel);
        }

        [HttpGet, ActionName("freight-report")]
        // GET: Report/freight-report
        public async Task<IActionResult> freightReport()
        {
            ReportModel<FreightReport> model = new ReportModel<FreightReport>();
            ViewBag.PageTitle = "FREIGHT REPORT";

            model.data = await _context.GetFreightReport(model);

            return View(model);
        }

        [HttpPost, ActionName("freight-report")]
        // POST: Report/freight-report/5
        public async Task<IActionResult> freightReport(ReportModel<FreightReport> model)
        {
            ReportModel<FreightReport> rptModel = new ReportModel<FreightReport>();
            ViewBag.PageTitle = "FREIGHT REPORT";

            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.destinationId = model.destinationId;
                rptModel.keyword = model.keyword;
                rptModel.data = await _context.GetFreightReport(model);
            }

            return View(rptModel);
        }

        [HttpGet, ActionName("expense-report")]
        // GET: Report/expense-report
        public async Task<IActionResult> expenseReport()
        {
            ReportModel<ExpenseReport> model = new ReportModel<ExpenseReport>();
            ViewBag.PageTitle = "EXPENSE REPORT";
            
            model.data = await _context.GetExpenseReport(model);

            return View(model);
        }

        [HttpPost, ActionName("expense-report")]
        // POST: Report/expense-report/5
        public async Task<IActionResult> expenseReport(ReportModel<ExpenseReport> model)
        {
            ReportModel<ExpenseReport> rptModel = new ReportModel<ExpenseReport>();
            ViewBag.PageTitle = "EXPENSE REPORT";

            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.destinationId = model.destinationId;
                rptModel.keyword = model.keyword;
                rptModel.data = await _context.GetExpenseReport(model);
            }

            return View(rptModel);
        }

        [HttpGet, ActionName("driver-payment-report")]
        // GET: Report/driver-payment-report
        public async Task<IActionResult> driverPaymentReport()
        {
            ReportModel<DriverPaymentReport> model = new ReportModel<DriverPaymentReport>();
            ViewBag.PageTitle = "DRIVER PAYMENT REPORT";
    
            model.data1 = await _context.GetDriverPaymentReport(model);
            
            model.ddl = await DataLoader.GetDriverDropDown(_dcontext);
            
            return View(model);
        }

        [HttpPost, ActionName("driver-payment-report")]
        // POST: Report/driver-payment-report/5
        public async Task<IActionResult> driverPaymentReport(ReportModel<DriverPaymentReport> model)
        {
            ReportModel<DriverPaymentReport> rptModel = new ReportModel<DriverPaymentReport>();
            ViewBag.PageTitle = "DRIVER PAYMENT REPORT";

            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.destinationId = model.destinationId;
                rptModel.keyword = model.keyword;
                if (model.destinationId > 0)
                {
                    rptModel.data1 = await _context.GetDriverPaymentReport(model);
                }
            }

            rptModel.ddl = await DataLoader.GetDriverDropDown(_dcontext);

            return View(rptModel);
        }

        [HttpGet, ActionName("lc-report")]
        // GET: Report/lc-report
        public async Task<IActionResult> loadingChargeReport()
        {
            ReportModel<LoadingChargesReport> model = new ReportModel<LoadingChargesReport>();
            ViewBag.PageTitle = "LOADING CHARGES REPORT";

            model.data = await _context.GetLoadingChargesReport(model);

            return View(model);
        }

        [HttpPost, ActionName("lc-report")]
        // POST: Report/lc-report/5
        public async Task<IActionResult> loadingChargeReport(ReportModel<LoadingChargesReport> model)
        {
            ReportModel<LoadingChargesReport> rptModel = new ReportModel<LoadingChargesReport>();
            ViewBag.PageTitle = "LOADING CHARGES REPORT";

            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.destinationId = model.destinationId;
                rptModel.keyword = model.keyword;
                rptModel.data = await _context.GetLoadingChargesReport(model);
            }

            return View(rptModel);
        }

        [HttpGet, ActionName("eway-report")]
        // GET: Report/eway-report
        public async Task<IActionResult> ewayReport()
        {
            ReportModel<EwayReport> model = new ReportModel<EwayReport>();
            ViewBag.PageTitle = "EWAY REPORT";

            model.data = await _context.GetEwayReport(model);

            return View(model);
        }

        [HttpPost, ActionName("eway-report")]
        // POST: Report/eway-report/5
        public async Task<IActionResult> ewayReport(ReportModel<EwayReport> model)
        {
            ReportModel<EwayReport> rptModel = new ReportModel<EwayReport>();
            ViewBag.PageTitle = "EWAY REPORT";

            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.destinationId = model.destinationId;
                rptModel.keyword = model.keyword;
                rptModel.data = await _context.GetEwayReport(model);
            }

            return View(rptModel);
        }
        [HttpGet, ActionName("loadingApprovalPending")]
        public async Task<IActionResult> loadingApproval()
        {
            ViewBag.PageTitle = "LOADING APPROVAL";
            var list = await _context.GetPlantOutridList();
            return View(list);
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Int64 id)
        {
            ViewBag.PageTitle = "LOADING CHARGES";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.LoadingChargesUpdate(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_tableloadingApproval", _context.GetPlantOutridList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("approveLoading")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> approveLoading(Int64 id)
        {
            ViewBag.PageTitle = "LOADING CHARGES";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.LoadingChargesUpdate1(id, user);

            }

            htmlString = await _viewRender.RenderToStringAsync("_tableloadingApproval", _context.GetPlantOutridList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}