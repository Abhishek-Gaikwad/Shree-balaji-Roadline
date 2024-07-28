using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly BillBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public BillController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new BillBll(context, spcontext, env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: PlantOut
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Search";
            var list = await _context.GetBillList();
            return View(list);
        }

      
    }
}
