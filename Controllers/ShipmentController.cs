using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class ShipmentController : Controller
    {
        private readonly DataDbContext _dbc;
        private readonly StoredProcedureDbContext _spc;
        private readonly ShipmentBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public ShipmentController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            _dbc = context;
            _spc = spcontext;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Shipment
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "SHIPMENT LIST";
            return View(await _context.GetShipments(Actions.shipment.ToString()));
        }

        // GET: Shipment/Edit/5
        public async Task<IActionResult> AddOrEdit(long id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Shipment" : "Update Shipment";

            var shp = await _context.GetShipmentById(id, true);
            if (id == 0)
            {
                shp.createdBy = user;
                shp.updatedBy = user;
            }
            else
            {
                shp.updatedBy = user;
            }

            return View(shp);
        }

        // POST: Shipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(long id, Shipment shp)
        {
            ViewBag.PageTitle = id == 0 ? "New Shipment" : "Update Shipment";

            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            string cid1 = shp.cid.ToString();
            if (ModelState.IsValid)
            {
                if (_context.ShipmentIsExist(shp.chasisNo, cid1) && id == 0)
                {
                    message = "Chasis No already exist...";
                }
                else
                    success = await _context.AddOrUpdateShipment(id, shp);

                if (success)
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", await _context.GetShipments(Actions.shipment.ToString()));
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), shp);
                }
            }
            else
            {
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), shp);
            }

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Shipment
        public async Task<IActionResult> AddBulk()
        {
            ViewBag.PageTitle = "Bulk Shipment";
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
            ViewBag.PageTitle = "Bulk Shipment";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            List<BulkShipment> list = new List<BulkShipment>();
            if (ModelState.IsValid)
            {
                if (modal.dataFile.FileName.EndsWith(".xls") || modal.dataFile.FileName.EndsWith(".xlsx"))
                {
                    var res = await _context.AddBulkShipment(modal, user);

                    success = res.Item1;
                    list = res.Item2.Count() > 0 ? res.Item2 : null;
                    if (success)
                        htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", await _context.GetShipments(Actions.shipment.ToString()));
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

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle, data = list });
        }

        [HttpGet, ActionName("eway-request")]
        // GET: Incidence/Edit/5
        public async Task<IActionResult> EwayRequest()
        {
            ViewBag.PageTitle = "Eway Request";
            ReportModel<Shipment> model = new ReportModel<Shipment>();
            DateTime dt = DateTime.Now.Date;
            model.fromDate = dt;
            model.toDate = dt;
            model.ddl = DataLoader.GetChassisDropdown(_dbc);
            return View(model);
        }

        [HttpPost, ActionName("eway-request")]
        // GET: Incidence/Edit/5
        public async Task<IActionResult> EwayRequest(ReportModel<Shipment> model)
        {
            ViewBag.PageTitle = "Eway Request";

            ReportModel<Shipment> reportModel = new ReportModel<Shipment>();

            if (ModelState.IsValid)
            {
                reportModel.data = await _context.GetEwayRequestList(model);
            }
            reportModel.fromDate = model.fromDate;
            reportModel.toDate = model.toDate;
            reportModel.ids = model.ids;
            reportModel.ddl = DataLoader.GetChassisDropdown(_dbc);

            return View(reportModel);
        }


        // POST: Shipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            ViewBag.PageTitle = "Delete Shipment";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                await _context.DeleteShipment(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", _context.GetShipments(Actions.shipment.ToString()).Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getVcList")]
        public async Task<JsonResult> GetVcList(string quoteId=null)
        {
            ViewBag.PageTitle = "VC LIST";
            bool res = false;

            var vcList = await DataLoader.GetVCDropDown(_spc, quoteId);
            res = vcList.Count() > 0 ? true : false;

            return Json(new { isValid = res, html = "", message = "", source = vcList });
        }

/*        [HttpPost("GetVcList/{id}/{quoteId}"), ActionName("getVcList")]
        public async Task<JsonResult> GetVcList(long id, string quoteId)
        {
            ViewBag.PageTitle = "Delete Shipment";
            bool res = false;

            var vcList =await DataLoader.GetVCDropDown(_dbc, _spc,id, quoteId);
            res = vcList.Count() > 0 ? true : false;

            return Json(new { isValid = res, html = "", message = "", source = vcList });
        }*/
    }
}
