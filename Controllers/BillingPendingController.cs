using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class BillingsPendingController : Controller
    {
        private readonly BillingsPendingBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public BillingsPendingController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new BillingsPendingBll(context, spcontext, env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: BillingPending
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "BillingsPending";
            var list = await _context.GetBillingsPendingList();
            return View(list);
        }
    }
}
