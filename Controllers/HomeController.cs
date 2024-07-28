using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppUserBll _context;
        private readonly ReportBll _scontext;
        private readonly ILogger<HomeController> _logger;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly string user;
        private readonly IncidenceBll bl;
        private readonly PlanOutBll pbl;
        private readonly ShipmentBll sbl;
        private readonly ShipmentBll trncnt;
        private readonly Incidence inc;

        public HomeController(ILogger<HomeController> logger, AppUserDbContext context, DataDbContext dbcontext, StoredProcedureDbContext spcontext, IHostEnvironment env, IHttpContextAccessor httpContext, IConfiguration config)
        {
            _context = new AppUserBll(context);
            _scontext = new ReportBll(dbcontext, spcontext, env);
            _logger = logger;
            _env = env;
            _config = config;
            user = httpContext.HttpContext.Session.GetString("username");
            bl = new IncidenceBll(dbcontext, env);
            sbl = new ShipmentBll(dbcontext, spcontext, env);
        }
        [Authorize]
        [HttpGet, ActionName("index")]
        public IActionResult Index()
        {

            /* current month count */

            int allocate_monthCount = sbl.AllocateCount();
            ViewBag.allocMonthCount = allocate_monthCount;

            int dispatch_monthCount = sbl.DispatchCount();
            ViewBag.dispatchMonthCount = dispatch_monthCount; 

            int trans_dispatch_monthCount = sbl.TransDispatchCount();
            ViewBag.transDispatchMonthCount = trans_dispatch_monthCount;

            int QuotationCount = sbl.QuotationCount();
            ViewBag.QuotationsCount = QuotationCount;

            int deliver_monthCount = sbl.DeliveredCount();
            ViewBag.delMonthCount = deliver_monthCount;

            int EPOD_PENDINGCount = sbl.EPOD_PENDING();
            ViewBag.EPOD_PENDINGCount = EPOD_PENDINGCount;

            int transit_monthCount = sbl.InTransit_Count();
            ViewBag.trnMonthCount = transit_monthCount;

            int ab_monthCount = bl.AccidentOrBreakdownCount();
            ViewBag.abMonthCount = ab_monthCount;

            decimal DieselLit = bl.DieselLit();
            ViewBag.DieselLit = DieselLit;

            decimal DieselAmount = bl.DieselAmount();
            ViewBag.DieselAmount = DieselAmount;

            ViewBag.reportUrl = _config.GetSection("ReportUrl").Value;
            ViewBag.PageTitle = "DASHBOARD";


            return View();
        }

        [HttpPost, ActionName("Index")]
        // POST: Report/incidence-report/5
        public async Task<IActionResult> GetData(ReportModel<Dashboard> model)
        {
            ReportModel<Dashboard> reportModel = new ReportModel<Dashboard>();
            ViewBag.PageTitle = "DASHBOARD REPORT";
            if (ModelState.IsValid)
            {
                reportModel.fromDate = model.fromDate;
                reportModel.toDate = model.toDate;

                int allocatedCount = sbl.GetStatusCount("IN_PLANT", model);
                ViewBag.allocatedCount_ = allocatedCount;

                int dispatchCount = sbl.GetStatusCount("IN_YARD", model);
                ViewBag.dispatchCount_ = dispatchCount;

                int transDispatchCount = sbl.GetStatusCount("NOT_IN_RCP", model);
                ViewBag.transDispatchCount_ = transDispatchCount;

                int deliveredCount = sbl.GetStatusCount("DELIVERED", model);
                ViewBag.deliveredCount_ = deliveredCount;

                int inTransitCount = sbl.GetStatusCount("IN_TRANSIT", model);
                ViewBag.inTransitCount_ = inTransitCount;

                int billingCount = sbl.GetStatusCount("BILLING_DONE", model);
                ViewBag.billingCount_ = billingCount;

                int accidentCount = sbl.GetStatusCount("ACCIDENT", model);
                int breakdownCount = sbl.GetStatusCount("BREAKDOWN", model);
                ViewBag.accidentBreakdownCount_ = accidentCount + breakdownCount;
            }

            return View(reportModel);
        }



        [HttpGet, ActionName("login")]
        public IActionResult Login()
        
        {
            LoginModel model = new LoginModel();
            if (_env.EnvironmentName == "Development") {
                model.userName = "sysadmin";
                model.password = "sbl@2O22";
            }
            ViewBag.PageTitle = "Log In";
            return View(model);
        }

        [HttpPost, ActionName("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ViewBag.PageTitle = "Log In";
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.email) | !string.IsNullOrEmpty(model.password))
                {
                    AppUser user = await _context.GetUserByIdPassword(model);
                    if (user != null)
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.email) },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(principal);
                        HttpContext.Session.SetString("username", user.userName);
                        //var rpt = JsonSerializer.Serialize(user.pages.Where(x => x.urlName.StartsWith("Report")));
                        HttpContext.Session.SetString("pages", JsonSerializer.Serialize(user.pages));

                        //return RedirectToAction("Index", "Quotation");
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid User Credentials..!";
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpGet, ActionName("forgot-password")]
        public IActionResult ForgotPassword()
        {
            ForgotModel model = new ForgotModel();
            ViewBag.PageTitle = "Forgot Password";
            return View(model);
        }

        [HttpPost, ActionName("forgot-password")]
        public IActionResult ForgotPassword(ForgotModel model)
        {
            ViewBag.PageTitle = "Forgot Password";
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.email))
                {
                    if (model.email.Equals("sysadmin@gmail.com"))
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.email) },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(principal);
                        HttpContext.Session.SetString("username", model.email);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid User Credentials..!";
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpGet, ActionName("reset-password")]
        [Authorize]
        public IActionResult ResetPassword()
        {
            ResetModel model = new ResetModel();
            ViewBag.PageTitle = "Reset Password";
            return View(model);
        }

        [HttpPost, ActionName("reset-password")]
        [Authorize]
        public IActionResult ResetPassword(ResetModel model)
        {
            ViewBag.PageTitle = "Reset Password";
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.password) | !string.IsNullOrEmpty(model.confirmPassword))
                {
                    if (model.password.Equals(model.confirmPassword))
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "") },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(principal);
                        HttpContext.Session.SetString("username", "");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid User Credentials..!";
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        [HttpGet, ActionName("logoff")]
        public IActionResult LockedScreen()
        {
            LoginModel model = new LoginModel()
            {
                userName = "System Admin",
                email = "sysadmin"
            };
            ViewBag.PageTitle = "Lock Screen";
            return View(model);
        }

        [HttpPost, ActionName("logoff")]
        public IActionResult LockedScreen(LoginModel model)
        {
            ViewBag.PageTitle = "Lock Screen";
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.password))
                {
                    if (model.password.Equals("Sp0rtz@2O22"))
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.email) },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(principal);
                        HttpContext.Session.SetString("username", model.email);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid User Credentials..!";
                        return View(model);
                    }
                }
            }
            return View(model);
        }


        [HttpGet, ActionName("logout")]
        [Authorize]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Home");
        }

        [HttpGet, ActionName("logger-info")]
        public IActionResult LogDetails()
        {

            ViewBag.basedir = $"{_env.ContentRootPath}/logs";
            return View();
        }

        [HttpGet, ActionName("report")]
        [AllowAnonymous]
        public IActionResult PrintReport()
        {

            return View();
        }

        [HttpGet, ActionName("eway-expiry")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEWayExpiryList()
        {
            var data = await _scontext.GetEwayExpiryList();

            return View(data);
        }

        [HttpGet, ActionName("EstimatedDateReport")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEstimatedDateList()
        {
            var data = await _scontext.GetEstimatedDateExpiryList();

            return View(data);
        }
        [HttpGet, ActionName("EstimatedDateDelayReport")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEstimatedDateDelayList()
        {
            var data = await _scontext.GetEstimatedDateDelayExpiryList();

            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}