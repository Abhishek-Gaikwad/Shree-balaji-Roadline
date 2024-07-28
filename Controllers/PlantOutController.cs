using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class PlantOutController : Controller
    {
        private readonly PlanOutBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public PlantOutController(DataDbContext context,StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new PlanOutBll(context, spcontext, env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: PlantOut
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "PLANT OUT";
            return View(await _spcontext.GetShipments(Actions.plantout.ToString()));
        }

        // GET: PlantOut/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Plant-Out" : "Update Plant-Out";
            PlantOut po = await DataLoader.GetPlanOutInfo(_spcontext, id);

            if (id == 0)
            {
                po.createdBy = user;
                po.updatedBy = user;
            }
            else
            {
                if (string.IsNullOrEmpty(po.createdBy))
                    po.createdBy = user;

                po.updatedBy = user;
            }

            po.ewayStatus = await DataLoader.GetShipmentStatusDropDown(Actions.plantout.ToString());

            po.shipmentId = id;

            return View(po);
        }

        // POST: PlantOut/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, PlantOut po)
        {
            ViewBag.PageTitle = id == 0 ? "New Plant-Out" : "Update Plant-Out";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                //if (string.IsNullOrEmpty(po.ewayNo))
                //{
                //    message = "Please provide eway bill number";
                //    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), po);
                //}
                //else
                //{
                    success = await _context.AddOrUpdatePlantout(id, po, user);
                    if (success)
                    {
                        htmlString = await _viewRender.RenderToStringAsync("_ViewAllPlantOuts", await _spcontext.GetShipments(Actions.plantout.ToString()));
                    }
                    else
                    {
                        htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), po);
                    }
               // }
            }
            else
            {
                message = "Eway Bill No should not be Empty";
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), po);
            }

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}
