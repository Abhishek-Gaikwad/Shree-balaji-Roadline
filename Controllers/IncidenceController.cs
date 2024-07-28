using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class IncidenceController : Controller
    {
        private readonly IncidenceBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly IViewRenderService _viewRender;
        private readonly string user;
        private readonly IHttpContextAccessor _http;

        public IncidenceController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new IncidenceBll(context, env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _http = httpContext;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Incidence
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "INCIDENCE";
            var list = await _spcontext.GetShipments(Actions.incidence.ToString());
            //var list = await _spcontext.Shipment.Where(o => o.status == shipmentStatus.plantout.ToString()).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> AuthIndex()
        {
            ViewBag.PageTitle = "Authority";
            var list = await _spcontext.GetShipments(Actions.auth.ToString());
            //var list = await _spcontext.Shipment.Where(o => o.status == shipmentStatus.plantout.ToString()).ToListAsync();
            return View(list);
        }

        // GET: Incidence/Edit/5
        public async Task<IActionResult> Breakdown(Int64 shipmentId)
        {
            ViewBag.PageTitle = "Breakdown";
            var incidence = await _context.GetIncedentInfo(shipmentId);

            incidence.type = "breakdown";
            incidence.createdBy = String.IsNullOrEmpty(incidence.createdBy) ? user : incidence.createdBy;
            incidence.updatedBy = user;

            return View(incidence);
        }

        // POST: Incidence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Breakdown(Int64 id, Incidence inc)
        {
            ViewBag.PageTitle = "Breakdown";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                var tupple = await _context.BreakdownOrAccidentUpdate(id, inc);
                success = tupple.Item1;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", _spcontext.GetShipments(Actions.incidence.ToString()).Result);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Incidence/print/{tupple.Item2}";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(Breakdown), inc);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(Breakdown), inc);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Incidence/Edit/5
        public async Task<IActionResult> Accident(Int64 shipmentId)
        {
            ViewBag.PageTitle = "Accident";
            var incidence = await _context.GetIncedentInfo(shipmentId);
            incidence.type = "accident";
            incidence.createdBy = String.IsNullOrEmpty(incidence.createdBy) ? user : incidence.createdBy;
            incidence.updatedBy = user;

            return View(incidence);
        }

        // POST: Incidence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accident(Int64 id, Incidence inc)
        {
            ViewBag.PageTitle = "Accident";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                var tupple = await _context.BreakdownOrAccidentUpdate(id, inc);
                success = tupple.Item1;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", _spcontext.GetShipments(Actions.incidence.ToString()).Result);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Incidence/print/{tupple.Item2}";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(Breakdown), inc);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(Breakdown), inc);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Incidence/Edit/5
        public async Task<IActionResult> Authority(Int64 shipmentId)
        {
            ViewBag.PageTitle = "Authority";
            var authority = await _context.GetAuthorityInfo(shipmentId);
            //  incidence.type = "authority";
            authority.createdBy = String.IsNullOrEmpty(authority.createdBy) ? user : authority.createdBy;
            authority.updatedBy = user;

            return View(authority);
        }

        // POST: Incidence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authority(Int64 id, Authority auth)
        {
            ViewBag.PageTitle = "Authority";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            //Initialize default fields
            if (ModelState.IsValid)
            {
                var tupple = await _context.AuthorityUpdate(id, auth);
                success = tupple.Item1;
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", _spcontext.GetShipments(Actions.incidence.ToString()).Result);
                    message = $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host.Value}/Incidence/authorityprint/{tupple.Item2}";
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(Authority), auth);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(Authority), auth);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Incidence/Report/5
        [HttpGet, ActionName("print")]
        public async Task<IActionResult> Report(Int64 id)
        {
            ViewBag.PageTitle = "Report";
            ViewBag.transporter = "SHREE BALAJI LOGISTICS";
            ViewBag.transInchargeInfo = "MR. JAGDISH SOROUT\r\n9975705253";

            var incidence = await _context.GetIncedentReport(id);
            if (incidence == null)
            {
                return NotFound();
            }
            return View(incidence);
        }

        // GET: Incidence/Report/5
        [HttpGet, ActionName("authorityprint")]
        public async Task<IActionResult> authorityReport(Int64 id)
        {
            ViewBag.PageTitle = "Authority Report";

            var authority = await _context.GetAuthorityReport(id);
            if (authority == null)
            {
                return NotFound();
            }
            return View(authority);
        }

        // POST: Incidence/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Incidence";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                success = await _context.DeleteIncidence(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllShipments", _spcontext.GetShipments(Actions.incidence.ToString()).Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}
