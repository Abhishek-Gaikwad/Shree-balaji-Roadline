using DocumentFormat.OpenXml.Office.CustomUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{
    public class PlantOut
    {
        public PlantOut()
        {
            this.expDate = DateTime.Now.Date;
            this.loadingCharges = 0;
        }
        [Key]
        public Int64 ewayId { get; set; }

        [Required]
        [ForeignKey("Shipment")]
        public Int64 shipmentId { get; set; }

        //[NotMapped]
        //[DisplayName("VC No")]
        //public string vcNo { get; set; }

        //[DisplayName("Plant Desc")]
        //public string plantDesc { get; set; }

        [NotMapped]
        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }


        [Required]
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("EWAY BILL NO")]
        public string? ewayNo { get; set; }

        [DisplayName("EXP. DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? expDate { get; set; }

        [DisplayName("STATUS")]
        [NotMapped]
        public string? spStatus { get; set; }

        public string? status { get; set; }

        [DisplayName("REASON")]
        public string? reason { get; set; }

        [DisplayName("LOADING CHARGES")]
        public decimal loadingCharges { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("PLANT OUT DATE")]
        public DateTime createdDate { get; set; }
        public DateTime? actualPlantOut { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }

        [NotMapped]
        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("DESTINATION")]
        [NotMapped]
        public string dest { get; set; }

        [NotMapped]
        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [NotMapped]
        public List<DropDown> ewayStatus = new List<DropDown>();
    }
    public class Marching
    {
        public Marching()
        {
            this.aadharcard = new List<IFormFile>();
            this.licensecard = new List<IFormFile>();
            this.photo = new List<IFormFile>();
            this.bankdetail = new List<IFormFile>();
            this.extraAmt = (decimal)0.00;
            this.expenses = (decimal)0.00;
            this.spdPaid = 0;
        }
        [Key]
        public Int64 voucherNo { get; set; }

        [Required]
        [DisplayName("SHIPMENT ID")]
        [ForeignKey("Shipment")]
        public Int64 shipmentId { get; set; }


        [Required]
        [DisplayName("TEMP.REG.NO")]
        public string tempRegNo { get; set; }

        [DisplayName("DRIVER INCHARGE")]
        public string? driverIncharge { get; set; }

        [Required]
        [DisplayName("DRIVER INCHARGE")]
        public Int64 inchargeId { get; set; }

        [Required]
        [DisplayName("DRIVER")]
        [ForeignKey("Driver")]
        public Int64 driverId { get; set; }

        [Required]
        [DisplayName("TOTAL HSD")]
        public decimal totalHsd { get; set; }

        [Required]
        [DisplayName("PUMP NAME")]
        public Int64 pumpName { get; set; }

        [Required]
        [DisplayName("RECEIPT NO")]
        public Int64 receiptNo { get; set; }

        [Required]
        [DisplayName("SPD QTY")]
        public decimal spdQty { get; set; }

        [Required]
        [DisplayName("SPD RATE")]
        public decimal spdRate { get; set; }

        [Required]
        [DisplayName("SPD AMOUNT")]
        public decimal spdAmount { get; set; }

        [Required]
        [DisplayName("EN-ROUTE EXP")]
        public decimal inRouteExp { get; set; }

        [Required]
        [DisplayName("FAST-TAG RECHARGE")]
        public decimal tollExp { get; set; }

        [Required]
        [DisplayName("LOADING CHARGES")]
        public decimal loadingCharge { get; set; }

        [Required]
        [DisplayName("DRIVER PAYMENT")]
        public decimal driverPayment { get; set; }

        [Required]
        [DisplayName("TOTAL EXP.")]
        public decimal totalExp { get; set; }

        public decimal? remainBalance { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public DateTime? actualMarching { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? status { get; set; }

        [DisplayName("EXTRA AMOUNT")]
        public decimal? extraAmt { get; set; }


        [DisplayName("REMARK")]
        public string? remark { get; set; }

        [DisplayName("EXTRA/POST DELIVERY EXPENSE")]
        public decimal? expenses { get; set; }

        [DisplayName("NARRATION")]
        public string? narration { get; set; }

        public int spdPaid { get; set; }

        [NotMapped]
        public decimal sblKm { get; set; }


        [NotMapped]
        [DisplayName("CHASSIS NO")]
        public string? chasisNo { get; set; }

        [NotMapped]
        [DisplayName("SHIPMENT NO")]
        public string? shipmentNo { get; set; }

        [NotMapped]
        public List<DropDown> loadingCharges = new List<DropDown>();

        [NotMapped]
        public List<DropDown> pumpList = new List<DropDown>();

        [NotMapped]
        public List<DropDown> driverList = new List<DropDown>();

        [NotMapped]
        [DisplayName("DRIVER NAME")]
        public string driverName { get; set; }

        [NotMapped]
        [DisplayName("ESTIMATED DATE")]
        [ReadOnly(true)]
        public string? estimatedDate { get; set; }
        [NotMapped]
        [DisplayName("PLANTOUT DATE")]
        [ReadOnly(true)]
        public string? plantOutDate { get; set; }

        [NotMapped]
        [DisplayName("MOBILE NO")]
        [StringLength(11, MinimumLength = 10)]
        public string mobileNo { get; set; }

        [NotMapped]
        [DisplayName("DL NO")]
        public string dlNo { get; set; }

        [NotMapped]
        [DisplayName("MODEL DESCRIPTION")]
        public string? model { get; set; }

        [NotMapped]
        [DisplayName("DESTINATION")]
        public string? destination { get; set; }

        [NotMapped]
        [DisplayName("AADHAR NO")]
        [StringLength(12, MinimumLength = 5)]
        public string aadharNo { get; set; }

        [NotMapped]
        [DisplayName("AADHAR CARD")]
        public List<IFormFile> aadharcard { get; set; }

        [NotMapped]
        [DisplayName("DRIVING LICENSE")]
        public List<IFormFile> licensecard { get; set; }

        [NotMapped]
        [DisplayName("license Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? licenseExpDate { get; set; }

        [NotMapped]
        [DisplayName("DRIVER PHOTO")]
        public List<IFormFile?> photo { get; set; }

        [NotMapped]
        public string? cameraPhoto { get; set; }

        [NotMapped]
        [DisplayName("CHEQUE/PASSBOOK/PHOTO")]
        public List<IFormFile> bankdetail { get; set; }
    }
    public class Voucher
    {
        public Voucher()
        {
            this.voucherDate = DateTime.Now.Date;
            this.balance = 0;
        }
        [Key]
        public Int64 id { get; set; }

        [Required]
        [DisplayName("RECEIPT NO")]
        public Int64 receiptNo { get; set; }

        [Required]
        [DisplayName("SHIPMENT ID")]
        [ForeignKey("Shipment")]
        public long shipmentId { get; set; }

        [Required]
        [DisplayName("VOUCHER NO")]
        public Int64 voucherNo { get; set; }

        [DisplayName("VOUCHER DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? voucherDate { get; set; }

        [DisplayName("REMARK")]
        public string? remark { get; set; }

        [Required]
        [DisplayName("AMOUNT")]
        public decimal amount { get; set; }

        [DisplayName("PAYMENT MODE")]
        public string payMode { get; set; }

        [DisplayName("PAYMENT PERCENTAGE")]
        public decimal payPercentage { get; set; }

        [DisplayName("STATUS")]
        public string status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        public List<Payment> vouchers = new List<Payment>();
        [NotMapped]
        public List<DropDown> shipmentList = new List<DropDown>();

        [NotMapped]
        [DisplayName("DRIVER NAME")]
        public string? driverName { get; set; }

        [NotMapped]
        [DisplayName("INCHARGE NAME")]
        public string? inchargeName { get; set; }

        [NotMapped]
        [DisplayName("LOCATION")]
        public string? location { get; set; }

        [NotMapped]
        [DisplayName("BALANCE")]
        public decimal? balance { get; set; }
    }
    public class BillingModel
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Bill Date")]
        public DateTime billingDate { get; set; }

        [Required]
        [DisplayName("SHIPMENT IDs")]
        public string? shipmentNos { get; set; }
        
        /*[Required]
        public int cid { get; set; }*/
        public string? status { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        public List<Shipment> shipmentList = new List<Shipment>();

        /*[NotMapped]
        public List<DropDown> companyList = new List<DropDown>();*/
    }
    public class Billing
    {
        public Billing()
        {
            this.tdsAmount = 0;
            this.penaltyAmount = 0;
            this.receivedAmount = 0;
        }
        [Key]
        public Int64 id { get; set; }

        [Required]
        [DisplayName("REFERENCE NO")]
        public Int64 billGrp { get; set; }

        [Required]
        [DisplayName("BILL NO")]
        public string billNo { get; set; }
        [DisplayName("SHIPMENT ID")]
        public Int64 shipmentId { get; set; }
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("BILL DATE")]
        public DateTime billingDate { get; set; }

        [Required]
        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [Required]
        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [Required]
        [DisplayName("TOTAL FREIGHT")]
        public decimal totalFreight { get; set; }

        //[Required]
        //[DisplayName("DISCOUNT(0.47%)")]
        //public decimal epcAmount { get; set; }

        [Required]
        [DisplayName("TAXABLE AMOUNT")]
        public decimal taxableAmount { get; set; }

        [Required]
        [DisplayName("GST AMOUNT")]
        public decimal gstAmount { get; set; }

        [Required]
        [DisplayName("TDS AMOUNT")]
        public decimal tdsAmount { get; set; }

        [Required]
        [DisplayName("PENALTY AMOUNT")]
        public decimal penaltyAmount { get; set; }

        [Required]
        [DisplayName("PAYABLE AMOUNT")]
        public decimal payableAmount { get; set; }

        [DisplayName("AKNOWLEDGE NO")]
        public string? aknowledgeno { get; set; }

        [DisplayName("REF NO")]
        public string? refno { get; set; }

        [DisplayName("C V NO")]
        public string? cvno { get; set; }

        [DisplayName("PAYMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? paymentdate { get; set; }

        [DisplayName("AKNOWLEDGE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? aknowledgedate { get; set; }

        [NotMapped]
        [DisplayName("RECEIVED AMOUNT")]
        public decimal receivedAmount { get; set; }

        public string? irnNo { get; set; }
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        public string? attachments { get; set; }
    }

    public class IrnBilling
    {
        public IrnBilling()
        {
            this.tdsAmount = 0;
            this.penaltyAmount = 0;
            this.receivedAmount = 0;
            this.paymentdate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            this.refno = "0";
            this.cvno = "0";
        }
        public Int64 id { get; set; }
        public Int64 billGrp { get; set; }
        public string billNo { get; set; }
        public Int64 shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public DateTime billingDate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal totalFreight { get; set; }
        //public decimal epcAmount { get; set; }
        public decimal taxableAmount { get; set; }
        public decimal gstAmount { get; set; }
        public decimal tdsAmount { get; set; }
        public decimal penaltyAmount { get; set; }
        public decimal payableAmount { get; set; }
        public decimal receivedAmount { get; set; }
        public string refno { get; set; }
        public string paymentdate { get; set; }
        public string cvno { get; set; }
    }

    public class IrnBillingModel
    {
        public string id { get; set; }
        public string billGrp { get; set; }
        public string billNo { get; set; }
        public string shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public string billingDate { get; set; }
        public string basicFreight { get; set; }
        public string enRoute { get; set; }
        public string totalFreight { get; set; }
        public string epcAmount { get; set; }
        public string taxableAmount { get; set; }
        public string gstAmount { get; set; }
        public string tdsAmount { get; set; }
        public string penaltyAmount { get; set; }
        public string payableAmount { get; set; }
        public string receivedAmount { get; set; }
        public string? refno { get; set; }
        public string? paymentdate { get; set; }
        public string? cvno { get; set; }
    }

    public class IrnsBilling
    {
        public IrnsBilling()
        {
            this.tdsAmount = 0;
            this.penaltyAmount = 0;
            this.aknowledgedate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            this.aknowledgeno = "0";
            this.irnNo = "0";
        }
        public Int64 id { get; set; }
        public Int64 billGrp { get; set; }
        public string billNo { get; set; }
        public Int64 shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public DateTime billingDate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal totalFreight { get; set; }
        //public decimal epcAmount { get; set; }
        public decimal taxableAmount { get; set; }
        public decimal gstAmount { get; set; }
        public decimal tdsAmount { get; set; }
        public decimal penaltyAmount { get; set; }
        public decimal payableAmount { get; set; }
        public string irnNo { get; set; }
        public string aknowledgeno { get; set; }
        public string aknowledgedate { get; set; }
    }

    public class IrnModel
    {
        public string id { get; set; }
        public string billGrp { get; set; }
        public string billNo { get; set; }
        public string shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public string billingDate { get; set; }
        public string basicFreight { get; set; }
        public string enRoute { get; set; }
        public string totalFreight { get; set; }
        public string epcAmount { get; set; }
        public string taxableAmount { get; set; }
        public string gstAmount { get; set; }
        public string tdsAmount { get; set; }
        public string penaltyAmount { get; set; }
        public string payableAmount { get; set; }
        public string irnNo { get; set; }
        public string aknowledgeno { get; set; }
        public string aknowledgedate { get; set; }
    }
    public class Incidence
    {
        public Incidence()
        {
            this.incidenceDate = DateTime.Now.Date;
        }

        [Key]
        public Int64 id { get; set; }

        [Required]
        public Int64 shipmentId { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string? shipmentNo { get; set; }

        [Required]
        [DisplayName("INCIDENT TYPE")]
        public string type { get; set; }
        public string? tempRegNo { get; set; }
        public string? engineNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? incidenceDate { get; set; }
        public string? incidencePlace { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? depositDate { get; set; }
        public string? incidenceNature { get; set; }
        public string? immediateAction { get; set; }
        public string? panchnamaHeld { get; set; }
        public string? driverReleased { get; set; }
        public string? thirdPartyInvolved { get; set; }
        public string? vcHandedOver { get; set; }
        public string? pcStationName { get; set; }
        public string? ChassisReleased { get; set; }
        public string? insSurveyDone { get; set; }
        public string? nearestDealer { get; set; }
        public string? complaintNo { get; set; }
        public string? attachments { get; set; }

        [NotMapped]
        [DisplayName("CHASSIS NO")]
        public string? chasisNo { get; set; }

        [NotMapped]
        [DisplayName("LOCATION")]
        public string? location { get; set; }

        [NotMapped]
        [DisplayName("MODEL DESC")]
        public string? modelDesc { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? status { get; set; }

        [DisplayName("ATTACHMENT")]
        [NotMapped]
        public List<IFormFile>? incidenceFiles { get; set; }
    }
    public class Payment
    {
        public Int64 id { get; set; }

        [DisplayName("VOUCHER NO")]
        public Int64 voucherNo { get; set; }

        [DisplayName("VOUCHER DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? voucherDate { get; set; }

        [Required]
        [DisplayName("REMARK")]
        public string remark { get; set; }

        [Required]
        [DisplayName("AMOUNT")]
        public decimal amount { get; set; }

        [DisplayName("PAYMENT MODE")]
        public string payMode { get; set; }

        [DisplayName("PAYMENT PERCENTAGE")]
        public decimal payPercentage { get; set; }
        [NotMapped]
        public Int64 serialNo { get; set; }
    }
    public class Authority
    {
        [Key]
        public Int64 Id { get; set; }

        [Required]
        public Int64 shipmentId { get; set; }

        [DisplayName("AUTHORITY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? authorityDate { get; set; }

        [Required]
        [DisplayName("NAME OF RE-PRESENTATIVE")]
        public string RName { get; set; }

        [Required]
        [DisplayName(" LICENCE NO")]
        public string licenceNo { get; set; }

        [DisplayName("TASS NAME")]
        public string? tassName { get; set; }

        [DisplayName(" TASS LOCATION")]
        public string? tassLocation { get; set; }

        [NotMapped]
        [DisplayName("CHASSIS NO")]
        public string? chasisNo { get; set; }

        [NotMapped]
        [DisplayName("DESTINATION")]
        public string? location { get; set; }

        [NotMapped]
        [DisplayName("MODEL DESC")]
        public string? modelDesc { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? status { get; set; }

    }
    public class MarchingGrid
    {
        public Int64 id { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("DEALER")]
        public string? dealerName { get; set; }

        [DisplayName("DESTINATION")]
        public string? dest { get; set; }

        [DisplayName("VC NO")]
        public string vcNo { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [DisplayName("PLANT CODE")]
        public string? plantCode { get; set; }

        [DisplayName("PLANT DESC")]
        public string plantDesc { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("INVOICE NO")]
        public string invoiceNo { get; set; }

        [DisplayName("INVOICE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime invoiceDate { get; set; }

        [DisplayName("EWAY BILL NO")]
        public string? ewayNo { get; set; }

        [DisplayName("EXP. DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? expDate { get; set; }

        [DisplayName("PLANTOUT. DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayName("ESTIMATED ARRIVAL DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? deliveryDate { get; set; }

        [DisplayName("STATUS")]
        public string status { get; set; }
    }
    public class IncidenceReport
    {
        public Int64 id { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string? shipmentNo { get; set; }

        [DisplayName("INCIDENT TYPE")]
        public string? type { get; set; }
        [DisplayName("TEMP REG.NO")]
        public string? tempRegNo { get; set; }
        [DisplayName("VC NO")]
        public string? vcNo { get; set; }
        [DisplayName("DESTINATION")]
        public string? destination { get; set; }
        [DisplayName("MODEL DESC")]
        public string? modelDesc { get; set; }
        [DisplayName("DRIVER NAME & NO")]
        public string? driverNameandNo { get; set; }
        [DisplayName("ENGINE NO")]
        public string? engineNo { get; set; }
        [DisplayName("INCIDENCE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? incidenceDate { get; set; }
        [DisplayName("INCIDENCE PLACE")]
        public string? incidencePlace { get; set; }
        [DisplayName("DEPOSITE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? depositDate { get; set; }
        [DisplayName("NATURE OF PROBLEM")]
        public string? incidenceNature { get; set; }
        [DisplayName("IMMEDIATE ACTION TAKEN")]
        public string? immediateAction { get; set; }
        [DisplayName("PUNCHANAME HELD")]
        public string? panchnamaHeld { get; set; }
        [DisplayName("DRIVER RELEASED")]
        public string? driverReleased { get; set; }
        [DisplayName("3RD PARTY INVOLVED")]
        public string? thirdPartyInvolved { get; set; }
        [DisplayName("VEHICLE HANDED OVER")]
        public string? vcHandedOver { get; set; }
        [DisplayName("POLICE STATION NAME ")]
        public string? pcStationName { get; set; }
        [DisplayName("CHASSIS RELEASED")]
        public string? ChassisReleased { get; set; }
        [DisplayName("ISURANCE SURVEY DONE")]
        public string? insSurveyDone { get; set; }
        [DisplayName("NEAREST DEALER")]
        public string? nearestDealer { get; set; }
        [DisplayName("COMPLAINT NO GIVEN")]
        public string? complaintNo { get; set; }
        [DisplayName("CHASSIS NO")]
        public string? chassisNo { get; set; }
        [DisplayName("INVOICE NO")]
        public string? invoiceNo { get; set; }
        [DisplayName("INVOICE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? invoiceDate { get; set; }
        [DisplayName("DRIVING LISENCE NO")]
        public string? dlNo { get; set; }
        /*[DisplayName("Attachments")]
        public string attachments { get; set; }*/
    }
    public class Attachments
    {
        public Attachments()
        {
            this.id = 0;
            this.driverId = 0;
        }
        [Key]
        public Int64 id { get; set; }
        public Int64 driverId { get; set; }
        public string aadharCard { get; set; }
        public string license { get; set; }
        public string? photo { get; set; }
        public string? bankDetails { get; set; }
    }
    public class FbvBillingModel{

        public FbvBillingModel() { 
            podAtttachedment= new List<IFormFile>();
        }
        public Int64 billId { get; set; }
        [DisplayName("BILL NO")]
        public string? billNo { get; set; }

        [DisplayName("SHIPMENT ID")]
        public Int64 shipmentId { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string? shipmentNo { get; set; }

        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [DisplayName("TOTAL FREIGHT")]
        public decimal totalFreight { get; set; }

        [DisplayName("RECEIVED AMOUNT")]
        public decimal receivedAmount { get; set; }

        [DisplayName("IRN NO")]
        public string irnNo { get; set; }

        [DisplayName("RECEIVED DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime billingDate { get; set; }

        [DisplayName("POD File (PDF/IMAGE)")]
        [NotMapped]
        public List<IFormFile?> podAtttachedment { get; set; }

        [DisplayName("POD File (PDF/IMAGE)")]
        public string? attachments { get; set; }
    }
    public class FbvBilling
    {

        public Int64? billId { get; set; }

        [DisplayName("BILL NO")]
        public string billNo { get; set; }

        [DisplayName("IRN NO")]
        public string irnNo { get; set; }

        [DisplayName("SHIPMENT ID")]
        public Int64 shipmentId { get; set; }

        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }
        
        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [DisplayName("VC NO")]
        public string vcNo { get; set; }

        [DisplayName("MODEL DESC")]
        public string modelDesc { get; set; }

        [DisplayName("MFG CODE")]
        public string mfgCode { get; set; }

        [DisplayName("PLANT CODE")]
        public string? plantCode { get; set; }

        [DisplayName("PLANT DESC")]
        public string plantDesc { get; set; }

        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [DisplayName("INVOICE NO")]
        public string invoiceNo { get; set; }

        [DisplayName("INVOICE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime invoiceDate { get; set; }

        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [DisplayName("FREIGHT VALUE")]
        public decimal totalFreight { get; set; }

        [DisplayName("RECEIVED AMOUNT")]
        public decimal receivedAmount { get; set; }
        
        [DisplayName("TRANSIT DAYS")]
        public int trasitDays { get; set; }

        [DisplayName("POD File")]
        public string? attachments { get; set; }
    }
    public class FbvInvoice
    {
        public string billNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime billingDate { get; set; }
        public Int64 plantCode { get; set; }
        public string plantDesc { get; set; }
        public string chasisNo { get; set; }
        public string modelDesc { get; set; }
        public string? irnNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal epcAmount { get; set; }
        public decimal enRoute { get; set; }
        public decimal taxableAmount { get; set; }
        public decimal gstAmount { get; set; }
        public decimal totalFreight { get; set; }
        public Int64 destId { get; set; }
        public string tmlGst { get; set; }
        public string destination { get; set; }
        public string dealerName { get; set; }
        public string? amountInWords { get; set; }
    }

    #region "IB/QUOTATION BILLING"
    public class IbBilling
    {
        [Key]
        public string id { get; set; }
        public string? invoiceNo { get; set; }
        public DateTime? invoiceDate { get; set; }
        public string? poNumber { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal gst { get; set; }
        public decimal totalFreight { get; set; }
        public string shipments { get; set; }
        public string quoteId { get; set; }
        public DateTime? createdDate { get; set; }
        public string? createdBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? updatedBy { get; set; }
        
        [DisplayName("POD File")]
        public string? attachments { get; set; }

        [NotMapped]
        public string? amountInWord { get; set; }

        [NotMapped]
        public List<QuoteDetail> list = new List<QuoteDetail>();
        [NotMapped]
        public List<QuoteShipment> spList = new List<QuoteShipment>();
    }

    public class IbBillingModel
    {
        public IbBillingModel()
        {
            podAtttachedment = new List<IFormFile>();
        }

        public string id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("INVOICE DATE")]
        public DateTime? invoiceDate { get; set; }

        [DisplayName("INVOICE NO")]
        public string? invoiceNo { get; set; }

        [DisplayName("PO NUMBER")]
        public string? poNumber { get; set; }

        [DisplayName("SHIPMENT IDs")]
        public string? shipmentNos { get; set; }
        
        [DisplayName("POD File (PDF/IMAGE)")]
        [NotMapped]
        public List<IFormFile?> podAtttachedment { get; set; }

        [DisplayName("POD File")]
        public string? attachments { get; set; }

        [NotMapped]
        public List<IbBillingShipment> shipmentList = new List<IbBillingShipment>();
    }

    public class IbQuoteDetails {
        public string vcNo { set; get; }
        public Int64 destId { set; get; }
        public string modelDesc { set; get; }
        public Int64 qty { set; get; }
    }

    public class QuoteShipment
    {
        public Int64 id { get; set; }
        public string tptCode { get; set; }
        public string tptName { get; set; }
        public string tradePlateNo { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string chassisNo { get; set; }
        public string model { get; set; }
        public string stmNo { get; set; }
        public DateTime stmDate { get; set; }
        public DateTime gpDate { get; set; }
        public DateTime marchDate { get; set; }
        public DateTime reachDate { get; set; }
        public decimal basic { get; set; }
        public decimal enRoute { get; set; }
        public string vcNo { get; set; }
    }

    public class IbBillingShipment
    {
        public string quoteId { get; set; }
        public string quoteNo { get; set; }
        public Int64 shipmentId { get; set; }
        public string shipmentNo { get; set; }
        public DateTime shipmentDate { get; set; }
        public string vcNo { get; set; }
        public string modelDesc { get; set; }
        public string chasisNo { get; set; }
        public string invoiceNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public int trasitDays { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal totalFreight { get; set; }
    }
    #endregion "IB/QUOTATION BILLING"

    public class DieselPayment {
        
        [DisplayName("VOUCHER NO")]
        [Key]
        public long voucherId { get; set; }

        [DisplayName("VOUCHER DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime voucherDate { get; set; }
        
        public long pumpId { get; set; }
        
        [DisplayName("REF / REMARK")]
        public string? remark { get; set; }

        [DisplayName("PREV BALANCE")]
        public decimal prevBalance { get; set; }

        [DisplayName("CURRENT BILL")]
        public decimal currentAmount { get; set; }
        
        [DisplayName("TOTAL PAYABLE")]
        public decimal payableAmount { get; set; }

        [DisplayName("PAYMENT AMOUNT")]
        public decimal paidAmount { get; set; }
        [DisplayName("BALANCE AMOUNT")]
        public decimal balanceAmount { get; set; }

        public decimal? spdQty { get; set; }
        public decimal? spdRate { get; set; }
        public decimal? spdAmount { get; set; }
        public string? vouchers { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [DisplayName("Created DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime createdDate { get; set; }

        [DisplayName("Updated DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime updatedDate { get; set; }


        [NotMapped]
        [DisplayName("PUMP NAME")]
        public string? pumpName { get; set; }
    }

    public class DieselPaymentCalc {
        public DieselPaymentCalc() {
            this.prevBalance = 0;
            this.currentAmount = 0;
            this.payableAmount = 0;
            this.paidAmount = 0;
            this.balanceAmount = 0;
            this.spdQty = 0;
            this.spdRate = 0;
            this.spdAmount = 0;
        }
        public decimal prevBalance { get; set; }
        public decimal currentAmount { get; set; }
        public decimal payableAmount { get; set; }
        public decimal paidAmount { get; set; }
        public decimal balanceAmount { get; set; }
        public decimal spdQty { get; set; }
        public decimal spdRate { get; set; }
        public decimal spdAmount { get; set; }
    }

    public class InchargePayment
    {
        public InchargePayment() {
            this.balanceAmount = 0;
            this.receivedAmount = 0;
            this.paidAmount = 0;
        }

        [DisplayName("VOUCHER NO")]
        [Key]
        public Int64 voucherId { get; set; }

        [DisplayName("VOUCHER DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime voucherDate { get; set; }
        
        [DisplayName("REFERENCE NO")]
        public Int64 inchargeId { get; set; }

        [DisplayName("REFERENCE NO")]
        public Int64 referenceNo { get; set; }

        [DisplayName("REMARK")]
        public string? remark { get; set; }

        [DisplayName("CURRENT BILL")]
        public decimal receivedAmount { get; set; }
        
        [DisplayName("PAYMENT AMOUNT")]
        public decimal paidAmount { get; set; }

        [DisplayName("BALANCE AMOUNT")]
        public decimal balanceAmount { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }

        [NotMapped]
        public string? inchargeName { get; set; }

        [NotMapped]
        public string? driverName { get; set; }

        [NotMapped]
        public string payMode { get; set; }

        [NotMapped]
        public List<DropDown> inchargeList = new List<DropDown>();
    }
}
