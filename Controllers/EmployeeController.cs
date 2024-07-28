using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;

namespace Trans9.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly DataDbContext _dbcontext;
        private readonly EmployeeBll _context;
        private readonly IViewRenderService _viewRender;
        private readonly string user;

        public EmployeeController(DataDbContext context, StoredProcedureDbContext spDbContext, IViewRenderService renderService, IHttpContextAccessor httpContext)
        {
            _context = new EmployeeBll(context, spDbContext);
            _dbcontext = context;
            _viewRender = renderService;
            user = httpContext.HttpContext.Session.GetString("username");
        }


        #region " EMPLOYEE"
        // GET: Employee
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "EMPLOYEE MASTER";
            return View(await _context.GetEmployeeList());
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> AddOrEdit(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Employee" : "Update Employee";
            Employees dlr = await _context.GetEmployeeById(id);

            dlr.createdBy = String.IsNullOrEmpty(dlr.createdBy) ? user : dlr.createdBy;
            dlr.updatedBy = user;

            return View(dlr);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Employees dlr)
        {
            ViewBag.PageTitle = id == 0 ? "New Employee" : "Update Employee";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdateEmployee(id, dlr);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllEmployees", _context.GetEmployeeList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dlr);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEdit), dlr);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Employee";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                success = await _context.DeleteEmployee(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllEmployees", _context.GetEmployeeList().Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
        #endregion " EMPLOYEE"

        #region "PAYROLL"
        // GET: Payroll
        [HttpGet, ActionName("payroll-index")]
        public async Task<IActionResult> PayrollIndex()
        {
            ViewBag.PageTitle = "PAYROLL MASTER";
            return View(await _context.GetPayrollList());
        }

        // GET: Payroll/Edit/5
        [HttpGet, ActionName("AddOrEditPayroll")]
        public async Task<IActionResult> AddOrEditPayroll(Int64 id = 0)
        {
            ViewBag.PageTitle = id == 0 ? "New Payroll" : "Update Payroll";
            Payroll p = await _context.GetPayrollById(id);

            p.createdBy = String.IsNullOrEmpty(p.createdBy) ? user : p.createdBy;
            p.updatedBy = user;

            p.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext);

            return View(p);
        }

        // POST: Payroll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("AddOrEditPayroll")]
        public async Task<IActionResult> AddOrEditPayroll(Int64 id, Payroll p)
        {
            ViewBag.PageTitle = id == 0 ? "New Payroll" : "Update Payroll";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            //Initialize default fields
            p.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext);

            if (ModelState.IsValid)
            {
                success = await _context.AddOrUpdatePayroll(id, p);
                if (success)
                {
                    htmlString = await _viewRender.RenderToStringAsync("_ViewAllPayroll", _context.GetPayrollList().Result);
                }
                else
                {
                    htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditPayroll), p);
                }
            }
            else
                htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditPayroll), p);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // POST: Payroll/Delete/5
        [HttpPost, ActionName("delete-payroll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayroll(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Payroll";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                success = await _context.DeletePayroll(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllPayroll", _context.GetPayrollList().Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }
        #endregion "PAYROLL"

        #region "PAYSLIP"
        // GET: Payslip
        [HttpGet, ActionName("payslip-index")]
        public async Task<IActionResult> PayslipIndex()
        {
            ViewBag.PageTitle = "PAYSLIP MASTER";
            PayRequest model = new PayRequest();
            model.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext,true);
            model.list = await _context.GetPayslipList(model);
            return View(model);
        }

        // GET: Payslip
        [HttpPost, ActionName("payslip-index")]
        public async Task<IActionResult> PayslipIndex(PayRequest model)
        {
            ViewBag.PageTitle = "PAYSLIP MASTER";
            PayRequest rptModel = new PayRequest();


            if (ModelState.IsValid)
            {
                rptModel.fromDate = model.fromDate;
                rptModel.toDate = model.toDate;
                rptModel.employeeId = model.employeeId;
                rptModel.list = await _context.GetPayslipList(rptModel);
            }

            rptModel.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext, true);

            return View(rptModel);
        }

        //// GET: Payslip/Edit/5
        //[HttpGet, ActionName("AddOrEditPayslip")]
        //public async Task<IActionResult> AddOrEditPayslip(Int64 id = 0)
        //{
        //    ViewBag.PageTitle = id == 0 ? "New Payslip" : "Update Payslip";
        //    Payslip p = await _context.GetPayslipById(id);

        //    p.createdBy = String.IsNullOrEmpty(p.createdBy) ? user : p.createdBy;
        //    p.updatedBy = user;

        //    p.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext);

        //    return View(p);
        //}

        //// POST: Payslip/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[HttpPost, ActionName("AddOrEditPayslip")]
        //public async Task<IActionResult> AddOrEditPayslip(Int64 id, Payslip p)
        //{
        //    ViewBag.PageTitle = id == 0 ? "New Payslip" : "Update Payslip";
        //    bool success = false;
        //    string htmlString = string.Empty;
        //    string message = string.Empty;
        //    //Initialize default fields
        //    p.ddl = await DataLoader.GetEmployeeDropDown(_dbcontext);

        //    if (ModelState.IsValid)
        //    {
        //        success = await _context.AddOrUpdatePayslip(id, p);
        //        if (success)
        //        {
        //            htmlString = await _viewRender.RenderToStringAsync("_ViewAllPayslip", _context.GetPayslipList(new PayRequest()).Result);
        //        }
        //        else
        //        {
        //            htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditPayslip), p);
        //        }
        //    }
        //    else
        //        htmlString = await _viewRender.RenderToStringAsync(nameof(AddOrEditPayslip), p);

        //    return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        //}

        // POST: Payslip/Delete/5
        [HttpPost, ActionName("delete-payslip")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayslip(Int64 id = 0)
        {
            ViewBag.PageTitle = "Delete Payslip";
            bool success = false;
            string htmlString = string.Empty;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                success = await _context.DeletePayslip(id, user);
            }

            htmlString = await _viewRender.RenderToStringAsync("_ViewAllPayslip", _context.GetPayslipList(new PayRequest()).Result);

            return Json(new { isValid = success, html = htmlString, message = message, source = ViewBag.PageTitle });
        }

        // GET: Payslip/print/5
        [HttpGet, ActionName("print")]
        public async Task<IActionResult> Report(Int64 id)
        {
            ViewBag.PageTitle = "Print";

            var vc = await _context.GetPayslipById(id);
            if (vc == null)
            {
                return NotFound();
            }
            return View(vc);
        }
        #endregion "PAYSLIP"

    }
}

