using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{

    public class Employees
    {
        [Key]
        public Int64 employeeId { get; set; }

        [DisplayName("FIRST NAME")]
        public string firstName { get; set; }

        [DisplayName("LAST NAME")]
        public string lastName { get; set; }

        [DisplayName("BIRTH DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dob { get; set; }

        [DisplayName("GENDER")]
        public string? gender { get; set; }

        [DisplayName("EMAIL")]
        public string? email { get; set; }

        [DisplayName("PHONE / MOBILE")]
        public string? phoneNo { get; set; }

        [DisplayName("ADDRESS")]
        public string? address { get; set; }

        [DisplayName("CITY")]
        public string? city { get; set; }

        [DisplayName("STATE")]
        public string? state { get; set; }

        [DisplayName("PIN")]
        public string? postalCode { get; set; }

        [DisplayName("COUNTRY")]
        public string? country { get; set; }

        [DisplayName("EMERGENCY PERSON")]
        public string? emergencyContactName { get; set; }

        [DisplayName("EMERGENCY PHONE")]
        public string? emergencyContactPhone { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
    }
    public class Payroll
    {
        public Payroll() {
            ddl = new List<DropDown>();
        }

        [Key]
        public long payrollId { get; set; }
        public long employeeId { get; set; }

        [DisplayName("BASIC SALARY")]
        public decimal basic { get; set; }

        [DisplayName("HOME RENT")]
        public decimal hra { get; set; }

        [DisplayName("TRAVELLING")]
        public decimal lta { get; set; }

        [DisplayName("CONVENIANCE")]
        public decimal conveniance { get; set; }

        [DisplayName("OTHER")]
        public decimal oa { get; set; }

        [DisplayName("HEALTH INSURANCE")]
        public decimal hic { get; set; }

        [DisplayName("PROFESSIONAL TAX")]
        public decimal pt { get; set; }

        [DisplayName("PROVIDEND FUND")]
        public decimal pf { get; set; }

        [DisplayName("OTHER")]
        public decimal od { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        public List<DropDown> ddl { get; set; }

        [NotMapped]
        public string? employeeName { get; set; }
    }

    public class Payslip
    {
        public Payslip()
        {
            ddl = new List<DropDown>();
        }

        [Key]
        public long payId { get; set; }
        public long employeeId { get; set; }

        [DisplayName("MONTH")]
        public int month { get; set; }

        [DisplayName("YEAR")]
        public int year { get; set; }

        [DisplayName("BASIC SALARY")]
        public decimal? basic { get; set; }

        [DisplayName("HOME RENT")]
        public decimal? hra { get; set; }

        [DisplayName("TRAVELLING")]
        public decimal? lta { get; set; }

        [DisplayName("CONVENIANCE")]
        public decimal? conveniance { get; set; }

        [DisplayName("OTHER")]
        public decimal? oa { get; set; }

        [DisplayName("HEALTH INSURANCE")]
        public decimal? hic { get; set; }

        [DisplayName("PROFESSIONAL TAX")]
        public decimal? pt { get; set; }

        [DisplayName("PROVIDEND FUND")]
        public decimal? pf { get; set; }

        [DisplayName("OTHER")]
        public decimal? od { get; set; }
        public decimal? taxable { get; set; }
        public decimal? taxRate { get; set; }
        public decimal? workedDays { get; set; }
        public decimal? netSalary { get; set; }
        public DateTime paymentDate { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        public List<DropDown> ddl { get; set; }

        [NotMapped]
        public string? employeeName { get; set; }

        [NotMapped]
        public Employees employee { get; set; }

        [NotMapped]
        public string monthName { get; set; }
    }

    public class PayRequest {
        public PayRequest() {
            fromDate = DateTime.Now.Date;
            toDate=DateTime.Now.Date;
            ddl = new List<DropDown>();
            list = new List<PayslipRes>();
            employeeId = 0;
        }

        public long employeeId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM/yyyy}")]
        [DisplayName("FROM")]
        public DateTime? fromDate { get; set; }

        [DisplayName("TO")]
        [DisplayFormat(DataFormatString = "{0:MMM/yyyy}")]
        public DateTime? toDate { get; set; }

        [NotMapped]
        public List<DropDown> ddl { get; set; }

        [NotMapped]
        public List<PayslipRes> list { get; set; }
    }

    public class PayslipRes {
        public long payId { get; set; }

        public long employeeId { get; set; }

        [DisplayName("NAME")]
        public string? employeeName { get; set; }

        [DisplayName("MONTH")]
        public string month { get; set; }

        [DisplayName("YEAR")]
        public int year { get; set; }

        [DisplayName("GROSS SALARY")]
        public decimal? earning { get; set; }

        [DisplayName("DEDUCTION")]
        public decimal? deduction { get; set; }

        [DisplayName("WORK DAYS")]
        public decimal? workedDays { get; set; }

        [DisplayName("NET SALARY")]
        public decimal? netSalary { get; set; }

        [DisplayName("PAY DATE")]
        public DateTime paymentDate { get; set; }
    }
}
