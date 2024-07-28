using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        private readonly DriverBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;
        public DriverController(DataDbContext context, IViewRenderService renderService, IHttpContextAccessor httpContext, IHostEnvironment env)
        {
            _context = new DriverBll(context, env);
            _viewRender = renderService;
            _env = env;
            user = httpContext.HttpContext.Session.GetString("username");
        }

        // GET: Driver
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "DRIVER MASTER";
            return View(await _context.GetDriverList());
        }

        // GET: Driver/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Driver" : "Update Driver";
            Driver dr = await _context.GetDriverById(id);

            dr.createdBy = String.IsNullOrEmpty(dr.createdBy) ? user : dr.createdBy;

            dr.updatedBy = user;

            dr.statusList = await DataLoader.GetDriverStatusDropDown();

            return View(dr);
        }

        // POST: Driver/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Driver dr)
        {
            ViewBag.PageTitle = id == 0 ? "New Driver" : "Update Driver";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;


            dr.statusList = await DataLoader.GetDriverStatusDropDown();

            //Initialize default fields
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    if (dr.aadharcard.Count>0 && dr.licensecard.Count>0) {
                        if (_context.DriverIsExist(dr.aadharNo, "aadhar"))
                            message = "Aadhar No alreay exist...";
                        else if (_context.DriverIsExist(dr.dlNo, "dl"))
                            message = "Driving License No alreay exist...";
                        else
                        {
                            success = await _context.AddOrUpdateDriver(id, dr);
                        } 
                    }
                }
                else
                    success = await _context.AddOrUpdateDriver(id, dr);

                if (success)
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllDrivers", _context.GetDriverList().Result);
                else
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dr);
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dr);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Driver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Driver";
            bool res = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                res = await _context.DeleteDriver(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllDrivers", _context.GetDriverList().Result);

            return Json(new { isValid = res, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        [HttpPost, ActionName("getdriver")]
        public async Task<IActionResult> GetDriverDetails(Int64 id)
        {
            ViewBag.PageTitle = "Get Driver";
            string message = string.Empty;
            List<string> urls = new List<string>();
            Driver dr = new Driver();
            try
            {
                if (id > 0)
                {
                    dr = await _context.GetDriverById(id);
                    if (dr != null)
                    {
                        if (dr.status == driverStatus.blocked.ToString())
                        {
                            message = "Driver is blocked...!";
                            return Json(new { isValid = false, message = message, source = ViewBag.PageTitle });
                        }
                    }
                }
                return Json(new
                {
                    isValid = true,
                    result = new
                    {
                        driverId = dr.driverId,
                        driverName = dr.driverName,
                        mobileNo = dr.mobileNo,
                        dlNo = dr.dlNo,
                        aadharNo = dr.aadharNo,
                        licenseExpDate = dr.licenseExpDate,
                        remark = dr.remark,
                        aadharDocs = dr.aadharDocs,
                        licenseDocs = dr.licenseDocs,
                        photos = dr.photos,
                        bankDocs = dr.bankDocs
                    },
                    message = message,
                    source = ViewBag.PageTitle
                });
            }
            catch (Exception e)
            {
                return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
            }
        }

        [HttpPost, ActionName("upload")]
        public async Task<IActionResult> OnPostMyUploader(IFormFile docFile, string type, string fileName)
        {
            ViewBag.PageTitle = "Upload Docs";
            string message = string.Empty;
            if (docFile != null)
            {
                try
                {
                    var url = $"wwwroot/docs/{type}/{fileName.Replace(" ", "-").ToLower()}.png";
                    string filePath = Path.Combine(_env.ContentRootPath, url);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        docFile.CopyTo(fileStream);
                    }
                    return Json(new { isValid = true, result = url.Replace("wwwroot", "") });
                }
                catch (Exception e)
                {
                    throw;
                    return Json(new { isValid = false, result = e.Message, message = message, source = ViewBag.PageTitle });
                }
            }
            return Json(new { isValid = false, result = "", message = message, source = ViewBag.PageTitle });

        }
    }
}
