using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class EmployeeBll
    {
        private readonly DataDbContext _context;
        private readonly StoredProcedureDbContext _spDbContext;
        private readonly IHostEnvironment _env;
        public EmployeeBll(DataDbContext context, StoredProcedureDbContext spDbContext, IHostEnvironment env = null)
        {
            _context = context;
            _spDbContext = spDbContext;
            _env = env;
        }

        #region "EMPLOYEE"
        public async Task<Employees> GetEmployeeById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Employees d = new Employees();
            try
            {
                d = await _context.Employees.FindAsync(id);

                if (d == null)
                {
                    d = new Employees()
                    { createdDate = dt, status = comonStatus.active.ToString() };
                }

                d.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdateEmployee(long id, Employees d)
        {
            bool success = false;
            try
            {
                d.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Employees.Add(d);
                }
                else
                {
                    _context.Employees.Update(d);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<List<Employees>> GetEmployeeList()
        {
            List<Employees> list = new List<Employees>();
            try
            {
                list = await _context.Employees.ToListAsync();
                if (list.Any())
                {
                    list = (from s in list
                            where s.status.Equals(comonStatus.active.ToString())
                            select s
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<bool> DeleteEmployee(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var d = await _context.Employees.FindAsync(id);
                if (d != null)
                {
                    d.updatedBy = user;
                    d.updatedDate = DateTime.Now;
                    d.status = comonStatus.deleted.ToString();
                    _context.Employees.Update(d);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool EmployeesExists(Int64 id = 0)
        {
            return _context.Employees.Any(e => e.employeeId == id);
        }
        #endregion "EMPLOYEE"

        #region "PAYROLL"
        public async Task<Payroll> GetPayrollById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Payroll d = new Payroll();
            try
            {
                d = await _context.Payroll.FindAsync(id);

                if (d == null)
                {
                    d = new Payroll()
                    { createdDate = dt, status = comonStatus.active.ToString() };
                }

                d.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdatePayroll(long id, Payroll d)
        {
            bool success = false;
            try
            {
                d.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Payroll.Add(d);
                }
                else
                {
                    _context.Payroll.Update(d);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<List<Payroll>> GetPayrollList()
        {
            List<Payroll> list = new List<Payroll>();
            try
            {
                list = await _context.Payroll.Where(o => o.status == comonStatus.active.ToString()).ToListAsync();
                var el = await _context.Employees.ToListAsync();
                if (list.Any() & el.Any())
                {
                    list.ForEach(x => x.employeeName = getEmployeeName(el, x.employeeId));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public async Task<bool> DeletePayroll(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var d = await _context.Payroll.FindAsync(id);
                if (d != null)
                {
                    d.updatedBy = user;
                    d.updatedDate = DateTime.Now;
                    d.status = comonStatus.deleted.ToString();
                    _context.Payroll.Update(d);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool PayrollExists(Int64 id = 0)
        {
            return _context.Payroll.Any(e => e.employeeId == id);
        }
        #endregion "PAYROLL"

        #region "PAYSLIP"
        public async Task<Payslip> GetPayslipById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Payslip d = new Payslip();
            try
            {
                d = await _context.Payslip.FindAsync(id);

                if (d == null)
                {
                    d = new Payslip()
                    { createdDate = dt, status = comonStatus.active.ToString() };
                }
                d.monthName = getMonthName(Convert.ToInt32(d.month));
                d.updatedDate = dt;
                d.employee=await _context.Employees.FindAsync(d.employeeId);
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdatePayslip(long id, Payslip d)
        {
            bool success = false;
            try
            {
                d.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Payslip.Add(d);
                }
                else
                {
                    _context.Payslip.Update(d);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<List<PayslipRes>> GetPayslipList(PayRequest model)
        {
            List<PayslipRes> list = new List<PayslipRes>();

            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spDbContext.PayslipRes
                .FromSqlRaw("CALL sbl_GetPaySlipList({0},{1},{2})", fromDate, toDate, model.employeeId)
                .ToListAsync();

                if (list.Any()) {
                    list = list.OrderByDescending(x => x.paymentDate).ToList();
                    list.ForEach(x => x.month = getMonthName(Convert.ToInt32(x.month)));
                }

            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        private string getMonthName(int month)
        {
            string monthName = GenericFunctions.months[month - 1];

            return monthName.Substring(0,3).ToUpper();
        }

        private string? getEmployeeName(List<Employees> el, long id)
        {
            string employeeName = null;
            try
            {
                var e = el.SingleOrDefault(x => x.employeeId == id);
                if (e != null)
                    employeeName = $"{e.firstName} {e.lastName}";
            }
            catch (Exception)
            {

                throw;
            }
            return employeeName;
        }

        public async Task<bool> DeletePayslip(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var d = await _context.Payslip.FindAsync(id);
                if (d != null)
                {
                    d.updatedBy = user;
                    d.updatedDate = DateTime.Now;
                    d.status = comonStatus.deleted.ToString();
                    _context.Payslip.Update(d);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool PayslipExists(Int64 id = 0)
        {
            return _context.Payslip.Any(e => e.employeeId == id);
        }
        #endregion "PAYSLIP"
    }
}
