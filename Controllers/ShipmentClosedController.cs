using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class ShipmentClosedController : Controller
    {
        private readonly ShipmentBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public ShipmentClosedController(DataDbContext context,StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }


        // GET: ShipmentClosed
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "SHIPMENT CLOSED/UPDATE";
            return View(await _context.GetShipmentForUpdate(Actions.closing.ToString()));
        }

        public async Task<IActionResult> CloseOrUpdate(long id = 0)
        {
            ViewBag.PageTitle = "Update Shipment";
            ShipmentClosed shp = new ShipmentClosed();
           // PlantOut po = new PlantOut();
                if (id != 0)
            {
                Shipment shipment = await _context.GetShipmentById(id);
                PlantOut po = await _context.GetPlanttById(id);
                if (shipment != null)
                {
                    shp.shipmentId = shipment.shipmentId;
                    shp.shipmentNo = shipment.shipmentNo;
                    shp.status = shipment.status;
                    shp.plantoutDate = po.createdDate.Date;
                    shp.estimatedDate = (po.createdDate != null) ? po.createdDate.Date.AddDays(shipment.trasitDays).ToString("dd/MM/yyyy") : "";
                }
            }

            shp.statusList = await DataLoader.GetShipmentStatusDropDown(Actions.closing.ToString());

            return View(shp);
        }

        // POST: Shipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseOrUpdate(long id, ShipmentClosed shp)
        {
            ViewBag.PageTitle = "Update Shipment";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            
            shp.statusList = await DataLoader.GetShipmentStatusDropDown(Actions.closing.ToString());
            shp.expenses = shp.expenses == null ? 0 : shp.expenses.Value;

            if (shp.expenses > 0 && string.IsNullOrEmpty(shp.narration))
            {
                message = !string.IsNullOrEmpty(shp.narration) ? null : "Please provide expenses detail";
                htmlString = await _viewRender.RenderToStringAsync(nameof(CloseOrUpdate), shp);
            }
            else if (ModelState.IsValid)
            {
               
                    success = await _context.CloseOrUpdateShipment(id, shp, user);
                    if (success)
                    {
                        htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", await _context.GetShipmentForUpdate(Actions.closing.ToString()));
                    }
                    else
                    {
                        htmlString = await _viewRender.RenderToStringAsync(nameof(CloseOrUpdate), shp);
                    }
                    // }
             }
            else
                    htmlString = await _viewRender.RenderToStringAsync(nameof(CloseOrUpdate), shp);


            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}
