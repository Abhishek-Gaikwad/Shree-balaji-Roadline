using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class AppUserController : Controller
    {
        private readonly AppUserBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly string user;

        public AppUserController(AppUserDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext)
        {
            _context = new AppUserBll(context);
            _viewRender = renderService;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: PumpMaster
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "User";
            return View(await _context.GetUserList());
        }

        // GET: PumpMaster/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New User" : "Update User";
            AppUser pm;
            if (id != 0)
            {
                pm = await _context.GetUserById(id);
            }
            else
            {
                pm = new AppUser();
                pm.status = comonStatus.active.ToString();
            }
            ViewBag.roles = await DataLoader.GetUserRolesDropDown(_context);

            return View(pm);

        }

        // POST: PumpMaster/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Int64 id, AppUser user)
        {
            ViewBag.PageTitle = id == 0 ? "New User" : "Update User";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateUser(id, user);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllUsers", _context.GetUserList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), user);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), user);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }


        // POST: PumpMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete User";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                res = await _context.DeleteUser(id);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllUsers", _context.GetUserList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
    }
}
