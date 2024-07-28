using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{
    public class ReportModel<T>
    {
        public ReportModel()
        {
            this.fromDate = DateTime.Now.Date;
            this.toDate = DateTime.Now.Date;
            this.destinationId = 0;
            this.companyId = 0;
            this.ids = new List<Int64>();
            this.statuses = new List<string>();
        }
        [DisplayName("From Date")]
        public DateTime? fromDate { get; set; }

        [DisplayName("To Date")]
        public DateTime? toDate { get; set; }


        [Required]
        [DisplayName("Destination")]
        [ForeignKey("Destination")]
        public Int64? destinationId { get; set; }

        [Required]
        [DisplayName("Company")]
        public Int64? companyId { get; set; }

        public List<Int64> ids { get; set; }
        public List<string> statuses { get; set; }
        public Int64? referenceNo { get; set; }

        [DisplayName("Destination")]
        [NotMapped]
        public string? dest { get; set; }
        [NotMapped]
        public string? keyword { get; set; }

        [DisplayName("Select File")]
        public IFormFile? dataFile { get; set; }
        [DisplayName("Status")]
        public string? status { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? reportId { get; set; }

        public List<DropDown> ddl = new List<DropDown>();
        public List<DropDown> ddl2 = new List<DropDown>();
        public List<T> data = new List<T>();
        public T? data1 { get; set; }
        [NotMapped]
        public List<DropDown> destinations = new List<DropDown>();
    }
    public class Diesel
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("VOUCHER NO")]
        public long voucherNo { get; set; }

        [DisplayName("PUMP NAME")]
        public string pumpName { get; set; }

        [DisplayName("LOCATION")]
        public string location { get; set; }

        [DisplayName("RECEIPT NO")]
        public Int64 receiptNo { get; set; }

        [DisplayName("QTY")]
        public decimal qty { get; set; }

        [DisplayName("RATE")]
        public decimal rate { get; set; }

        [DisplayName("AMOUNT")]
        public decimal amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime voucherDate { get; set; }
    }
    [Table("TBL_BILLING")]
    public class BillRow
    {
        public Int64 shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public DateTime shipmentDate { get; set; }
        public string plantDesc { get; set; }
        public string plantCode { get; set; }
        public string invoiceNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public decimal basicRate { get; set; }
        public decimal inroute { get; set; }
        public decimal totalExp { get; set; }
        public decimal discount { get; set; }
        public decimal taxable { get; set; }
        public decimal transitDays { get; set; }
        public decimal incidanceDelayed { get; set; }
        public decimal atd { get; set; }
        public string status { get; set; }
    }
    public class BillModel
    {
        public BillModel()
        {
            //vendor basic info
            this.vendorCode = "S64690";
            this.vendorName = "SHREE BALAJI LOGISTICS";
            this.serviceType = "CIF";
            this.sacCode = "996519";
            this.vendorGSTNo = "27BDOPS0723A1ZQ";
            this.vendorRegion = "MH";

            //Source TML Plant / RSO
            this.sourcePlant = "PUNE CV -1001";
            this.sourceGSTNo = "27AAACT2727Q1ZW";
            this.sourcePlantRegion = "MH";
            this.bussinessUnit = "Bussiness Unit CV";
        }
        public string vendorCode { get; set; }
        public string vendorName { get; set; }
        public string serviceType { get; set; }
        public string sacCode { get; set; }
        public string vendorGSTNo { get; set; }
        public string vendorRegion { get; set; }
        public string billNo { get; set; }
        public DateTime billDate { get; set; }
        public string shipmentNo { get; set; }
        public DateTime shipmentDate { get; set; }
        public string plantCode { get; set; }
        public string invoiceNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enroute { get; set; }
        public decimal totalfreight { get; set; }
        //public decimal discountAmount { get; set; }
        public decimal taxebleValue { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        /*public decimal tdsAmount { get; set; }
        public decimal penaltyAmount { get; set; }*/
        public decimal totalInvoiceAmount { get; set; }
        public string sourcePlant { get; set; }
        public string sourceGSTNo { get; set; }
        public string sourcePlantRegion { get; set; }
        public string bussinessUnit { get; set; }
        public string irnNo { get; set; }
    }
    public class Receipt
    {
        public Int64 receiptNo { get; set; }

        public string paidTo { get; set; }
        public string amountInWord { get; set; }
        public string remark { get; set; }
        public string payMode { get; set; }

        [DisplayName("Amount")]
        public decimal amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime voucherDate { get; set; }
    }
    public class ProfitLoss
    {
        public Int64 shipmentId { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        [DisplayName("VC NO")]
        public string vcNo { get; set; }

        [DisplayName("MFG CODE")]
        public string mfgCode { get; set; }

        [DisplayName("ROUTE CODE")]
        public string routeCode { get; set; }

        [DisplayName("TRIP EXPENSE")]
        public decimal expense { get; set; }

        [DisplayName("FREIGHT")]
        public decimal freight { get; set; }

        [DisplayName("P/L AMOUNT")]
        public decimal plAmount { get; set; }

        [DisplayName("STATUS P/L")]
        public string plStatus { get; set; }
    }
    public class FreightReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("ROUTE CODE")]
        public string? routeCode { get; set; }

        [DisplayName("ROUTE NAME")]
        public string? routeName { get; set; }

        [DisplayName("CHASSIS NO")]
        public string? chassisNo { get; set; }

        [DisplayName("VC NO")]
        public string? vcNo { get; set; }

        [DisplayName("MFG CODE")]
        public string? mfgCode { get; set; }

        [DisplayName("PLANT DESC")]
        public string? plantDesc { get; set; }

        [DisplayName("PLANT CODE")]
        public string? plantCode { get; set; }

        [DisplayName("BASIC")]
        public decimal? basic { get; set; }

        [DisplayName("ENROUTE")]
        public decimal? enroute { get; set; }
    }
    public class ExpenseReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chassisNo { get; set; }

        [DisplayName("DEALER NAME")]
        public string dealerName { get; set; }

        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [DisplayName("TOTAL HSD")]
        public decimal? totalHsd { get; set; }

        [DisplayName("SPOT HSD")]
        public decimal? spotHsd { get; set; }

        [DisplayName("IN HAND DIESAL")]
        public decimal? inHandDiesal { get; set; }

        [DisplayName("DRIVER PAYMENT")]
        public decimal? driverPayment { get; set; }

        [DisplayName("ENROUTE EXPENSE")]
        public decimal? enroute { get; set; }

        [DisplayName("FASTAG CHARGES")]
        public decimal? fastagCharges { get; set; }

        [DisplayName("EXTRA PAYMENT")]
        public decimal? extraPayment { get; set; }

        [DisplayName("EXTRA PAYMENT NARATION")]
        public string? paymentNaration { get; set; }

        [DisplayName("TOTAL EXPENSE")]
        public decimal? totalExpense { get; set; }
        [DisplayName("SPOT DIESEL AMOUNT")]
        public decimal spotAmount { get; internal set; }
        [DisplayName("POST DELIVERY EXPENSES")]
        public decimal? postDeliveryexp { get; internal set; }
        [DisplayName("POST DELIVERY REMARK")]
        public string? postExpNaration { get; internal set; }
    }
    public class DriverPaymentReport
    {

        [DisplayName("NO OF TRIPS TILL DATE")]
        public Int64 noOfTrips { get; set; }

        public List<DriverPayment> payments = new List<DriverPayment>();
    }
    public class DriverPayment
    {
        [DisplayName("INCHARGE NAME")]
        public string? inchargeName { get; set; }

        [DisplayName("TRIP DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? tripDate { get; set; }

        [DisplayName("DESTINATION")]
        public string? destination { get; set; }

        [DisplayName("VOUCHER NO")]
        public Int64 voucherNo { get; set; }

        [DisplayName("VOUCHER AMOUNT")]
        public decimal voucherAmount { get; set; }

        [DisplayName("RECEIPTS GENERATED AGAINST VOUCHER")]
        public Int64 receipts { get; set; }

        [DisplayName("PAID AMOUNT")]
        public decimal? paidAmount { get; set; }

        [DisplayName("BALANCE AMOUNT")]
        public decimal? balanceAmount { get; set; }
    }
    public class LoadingChargesReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? shipmentDate { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chassisNo { get; set; }

        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        [DisplayName("PLANT CODE")]
        public string plantCode { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [DisplayName("LOADING CHARGES")]
        public decimal? charges { get; set; }

        [DisplayName("PLANT OUT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? plantOutDate { get; set; }
    }
    public class EwayReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("CHASIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("EWAY BILL NO")]
        public string? ewayBillNo { get; set; }

        [DisplayName("ISSUE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? issueDate { get; set; }

        [DisplayName("EXPIRY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? expiryDate { get; set; }
    }
    public class EwayExpiryReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }
        [DisplayName("TEMP REG NO")]
        public string? tempRegNo { get; set; }

        [DisplayName("DRIVER INCHARGE")] 
        public string? driverIncharge { get; set; }

        [DisplayName("DRIVER NAME")] 
        public string? driverName { get; set; }

        [DisplayName("DESTINATION")] 
        public string destination { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("MOBILE NO")]
        public string? mobileNo { get; set; }

        [DisplayName("EWAY BILL NO")]
        public string? ewayNo { get; set; }

        [DisplayName("EXPIRY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime expDate { get; set; }
        public string ewayId { get; set; }
    }
    public class EstimatedDateReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [NotMapped]
        [DisplayName("PlantOut DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime createdDate { get; set; }

        [DisplayName("TEMP REG NO")]
        public string tempRegNo { get; set; }

        [NotMapped]
        public int? trasitDays { get; set; }

        [DisplayName("DESTINATION")]
        public string? destination { get; set; }

        [DisplayName("DRIVER INCHARGE")]
        public string? driverName { get; set; }

        [DisplayName("DRIVER INCHARGE")]
        public string inchargeId { get; set; }

        [DisplayName("MOBILE NO")]
        public string mobileNo { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("EXPIRY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime expDate { get; set; }

        [DisplayName("DAY COUNT")]
        public int? datediff { get; set; }
    }
    public class ShipmentReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("DEALER NAME")]
        public string dealerName { get; set; }


        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        //[DisplayName("DRIVER INCHARGE")]
        //public string? inchargeId { get; set; }

        [DisplayName("DRIVER INCHARGE")]
        public string? driverName{ get; set; }

        [DisplayName("REGION")]
        public string region { get; set; }

        [DisplayName("VC NO")]
        public string vcNo { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDescription { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chassisNo { get; set; }

        [DisplayName("EWAY BILL NO")]
        public string? ewayBillNo { get; set; }

        [DisplayName("INVOICE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime invoiceDate { get; set; }

        [DisplayName("INVOICE NO")]
        public string invoiceNo { get; set; }

        [DisplayName("CURRENT STATUS")]
        public string currentStatus { get; set; }

        [DisplayName("PLANT OUT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? plantOutDate { get; set; }

        [DisplayName("STANDARD TRANSIT TIME")]
        public long standardTransitTime { get; set; }

        [DisplayName("LAST ESTIMATED DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? lastEstimatedDate { get; set; }

        [DisplayName("DELIVERY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? deliveryDate { get; set; }

        [DisplayName("REACH DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? reachDate { get; set; }

        [DisplayName("PLANT CODE")]
        public string? plantCode { get; set; }

        [DisplayName("PLANT DESCRIPTION")]
        public string plantDescription { get; set; }

        [DisplayName("CREATED DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime createdDate { get; set; }

        [DisplayName("EPOD NUMBER")]
        public string? epodNo { get; set; }

        [DisplayName("EPOD DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? epodDate { get; set; }

        [DisplayName("TEMP REG NO")]
        public string? tempRegNo { get; set; }

        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [DisplayName("E-WAY EXP DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ewayExpDate { get; set; }
    }
    public class BillingReport
    {
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("BILL NO")]
        public string? billNo { get; set; }

        [DisplayName("BILL DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime billDate { get; set; }

        [DisplayName("BASIC RATE")]
        public decimal basicFreight { get; set; }

        [DisplayName("ENROUTE RATE")]
        public decimal enRoute { get; set; }

        [DisplayName("DISCOUNT")]
        public decimal discount { get; set; }

        [DisplayName("TAXABLE VALUE")]
        public decimal taxableValue { get; set; }

        [DisplayName("GST AMOUNT")]
        public decimal gstAmount { get; set; }

        [DisplayName("TML GST NO")]
        public string? tmlGstNo { get; set; }

        [DisplayName("TOTAL INVOICE AMOUNT")]
        public decimal invoiceAmount { get; set; }

        [DisplayName("INVOICE STATUS")]
        public string? invoiceStatus { get; set; }

        [DisplayName("RECEIVED AMOUNT")]
        public decimal receivedAmount { get; set; }

        [DisplayName("TDS AMOUNT")]
        public decimal tdsAmount { get; set; }

        [DisplayName("IRN NO")]
        public string? irnNo { get; set; }

        [DisplayName("REFERENCE NO")]
        public string? referenceNo { get; set; }

        [DisplayName("PAYMENT RECEIVED DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime paymentReceivedDate { get; set; }
    }

    public class Dashboard
    {
        public string Path { get; set; }
        public string? status { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }
    }

    public class DieselVoucherModel
    {
        [DisplayName("From Date")]
        public DateTime? fromDate { get; set; }

        [DisplayName("To Date")]
        public DateTime? toDate { get; set; }

        [DisplayName("PUMP NAME")]
        public Int64? pumpId { get; set; }

        [DisplayName("status")]
        public Int64? status { get; set; }
    }

    public class CalculateDiesel { 
        public long[] vouchers { get; set; }
        public int pumpId { get; set; }
    }

    public class DieselRequestModel<T> {
        public DieselRequestModel()
        {
            this.fromDate = DateTime.Now.Date;
            this.toDate = DateTime.Now.Date;
            this.pumpId = 0;
            this.status = 0;
        }
        [DisplayName("From Date")]
        public DateTime? fromDate { get; set; }

        [DisplayName("To Date")]
        public DateTime? toDate { get; set; }

        [Required]
        [DisplayName("Pump Name")]
        public Int64 pumpId { get; set; }

        [DisplayName("status")]
        public Int64? status { get; set; }

        [DisplayName("VOUCHER DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? voucherDate { get; set; }

        [DisplayName("PREV BALANCE")]
        public decimal? prevBalance { get; set; }

        [DisplayName("CURRENT BILL")]
        public decimal? currentAmount { get; set; }

        [DisplayName("TOTAL PAYABLE")]
        public decimal? payableAmount { get; set; }

        [DisplayName("PAYMENT AMOUNT")]
        public decimal? paidAmount { get; set; }
        [DisplayName("BALANCE AMOUNT")]
        public decimal? balanceAmount { get; set; }

        public decimal? spdQty { get; set; }
        public decimal? spdRate { get; set; }
        public decimal? spdAmount { get; set; }
        public string? vouchers { get; set; }

        public List<DropDown> ddl = new List<DropDown>();
        
        public List<T> data = new List<T>();
    }
}
