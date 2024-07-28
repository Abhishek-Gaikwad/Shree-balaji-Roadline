using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly AppUserBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly string user;

        public RolesController(AppUserDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext)
        {
            _context = new AppUserBll(context);
            _viewRender = renderService;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: PumpMaster
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Roles";
            return View(await _context.GetRolesList());
        }

        // GET: PumpMaster/Edit/5
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.PageTitle = id==0 ? "New Roles" : "Update Roles";
            RoleModal pm;
            if (id!=0)
            {
                pm = await _context.GetRoleById(id);
            }
            else
            {
                pm = new RoleModal();
            }
            ViewBag.pages = await DataLoader.GetPagesDropDownList(_context);
            return View(pm);

        }

        // POST: PumpMaster/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, RoleModal role)
        {
            ViewBag.PageTitle = id == 0 ? "New Roles" : "Update Roles";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateRole(id, role);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoles", _context.GetRolesList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), role);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), role);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // POST: PumpMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //ViewBag.PageTitle = "Delete Roles";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                res = await _context.DeleteRole(id);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllRoles", _context.GetRolesList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}
