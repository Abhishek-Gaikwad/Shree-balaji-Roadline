using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{
    public class VcMaster
    {
        [Key]
        public Int64 _Id { get; set; }

        [Required]
        [DisplayName("VC NO")]
        public string vcNo { get; set; }

        [Required]
        [DisplayName("MODEL DESC")]
        public string modelDesc { get; set; }

        [Required]
        [DisplayName("MFG CODE")]
        public string mfgCode { get; set; }

        [Required]
        [DisplayName("PLANT CODE")]
        public string plantCode { get; set; }

        [Required]
        [DisplayName("PLANT DESC")]
        public string plantDesc { get; set; }

        [Required]
        [DisplayName("SBL KMPL")]
        public decimal sblKmpl { get; set; }

        [Required]
        [DisplayName("TATA KMPL")]
        public decimal tKmpl { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? status { get; set; }

    }
    public class Destination
    {
        [Key]
        public Int64 detsinationId { get; set; }

        [Required]
        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        [DisplayName("STATE")]
        public string? state { get; set; }

        [Required]
        [DisplayName("REGION")]
        public string region { get; set; }

        [Required]
        [DisplayName("ROUTE CODE")]
        public string routeCode { get; set; }

        [Required]
        [DisplayName("TRANSIT DAYS")]
        public int trasitDays { get; set; }

        [Required]
        [DisplayName("TATA KMS")]
        public decimal tKms { get; set; }

        [Required]
        [DisplayName("SBL KMS")]
        public decimal sblKms { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

    }
    public class Dealer
    {
        [Key]
        public Int64 dealerId { get; set; }
        [Required]
        [DisplayName("DEALER NAME")]
        public string dealerName { get; set; }
        [Required]
        [DisplayName("CITY")]
        public string city { get; set; }

        [DisplayName("STATE")]
        public string? state { get; set; }
        [Required]
        [DisplayName("REGION")]
        public string region { get; set; }
        [Required]
        [DisplayName("DEALER TYPE")]
        public string dealerType { get; set; }
        [Required]
        [DisplayName("EPOD PERSON")]
        public string epodPerson { get; set; }
        [Required]
        [DisplayName("EPOD CONTACT")]
        public string epodContact { get; set; }
        [Required]
        [DisplayName("EPOD EMAIL")]
        public string epodEmail { get; set; }
        [Required]
        [DisplayName("PDI PERSON")]
        public string pdiPerson { get; set; }
        [Required]
        [DisplayName("PDI CONTACT")]
        public string pdiContact { get; set; }
        [Required]
        [DisplayName("PDI EMAIL")]
        public string pdiEmail { get; set; }
        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        public List<DropDown> dealerTypes = new List<DropDown>();
    }
    public class Driver
    {
        public Driver()
        {
            this.aadharcard = new List<IFormFile>();
            this.licensecard = new List<IFormFile>();
            this.photo = new List<IFormFile>();
            this.bankdetail = new List<IFormFile>();
        }
        [Key]
        public Int64 driverId { get; set; }

        [Required]
        [DisplayName("DRIVER NAME")]
        public string driverName { get; set; }

        [Required]
        [DisplayName("MOBILE NO")]
        [StringLength(11, MinimumLength = 10)]
        public string mobileNo { get; set; }

        [Required]
        [DisplayName("DL NO")]
        public string dlNo { get; set; }

        [Required]
        [DisplayName("AADHAR NO")]
        [StringLength(12, MinimumLength = 5)]
        public string aadharNo { get; set; }

        [DisplayName("license Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? licenseExpDate { get; set; }

        public string? remark { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        [NotMapped]
        [DisplayName("AADHAR CARD")]
        public List<IFormFile?> aadharcard { get; set; }

        [NotMapped]
        [DisplayName("DRIVING LICENSE")]
        public List<IFormFile?> licensecard { get; set; }

        [NotMapped]
        [DisplayName("DRIVER PHOTO")]
        public List<IFormFile?> photo { get; set; }

        [NotMapped]
        public string? cameraPhoto { get; set; }

        [NotMapped]
        [DisplayName("CHEQUE/PASSBOOK/PHOTO")]
        public List<IFormFile?> bankdetail { get; set; }

        [NotMapped]
        public List<string> aadharDocs = new List<string>();
        [NotMapped]
        public List<string> licenseDocs = new List<string>();
        [NotMapped]
        public List<string> photos = new List<string>();
        [NotMapped]
        public List<string> bankDocs = new List<string>();

        [NotMapped]
        public List<DropDown> statusList = new List<DropDown>();
    }
    public class Shipment
    {
        public Shipment()
        {
            this.incidanceDelayed = 0;
            this.epodNo = "";
            this.shipmentDate = DateTime.Now.Date;
            this.invoiceDate = DateTime.Now.Date;
            this.selected = false;
        }

        [Key]
        public Int64 shipmentId { get; set; }

        [Required]
        [DisplayName("COMPANY")]
        public int cid { get; set; }

        [Required]
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [Required]
        [DisplayName("SHIPMENT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime shipmentDate { get; set; }

        [NotMapped]
        [DisplayName("DELIVERY DATE ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [ReadOnly(true)]
        public DateTime? estimatedDate { get; set; }


        [Required]
        [DisplayName("DEALER")]
        [ForeignKey("Dealer")]
        public Int64 dealerId { get; set; }


        [DisplayName("DEALER")]
        [NotMapped]
        public string? dealerName { get; set; }

        [Required]
        [DisplayName("DESTINATION")]
        [ForeignKey("Destination")]
        public Int64 detsinationId { get; set; }

        [DisplayName("DESTINATION")]
        [NotMapped]
        public string? dest { get; set; }


        [Required]
        [DisplayName("LOCATION")]
        public string location { get; set; }

        [Required]
        [DisplayName("TRANSIT DAYS")]
        public int trasitDays { get; set; }

        [Required]
        [DisplayName("REGION")]
        public string region { get; set; }

        [Required]
        [DisplayName("ROUTE CODE")]
        public string routeCode { get; set; }

        [Required]
        [DisplayName("VC NO")]
        [ForeignKey("VcMaster")]
        public string vcNo { get; set; }

        [Required]
        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }

        [Required]
        [DisplayName("MFG CODE")]
        public string mfgCode { get; set; }

        [DisplayName("PLANT CODE")]
        public string? plantCode { get; set; }

        [Required]
        [DisplayName("PLANT DESC")]
        public string plantDesc { get; set; }

        [Required]
        [DisplayName("MFG ROUTE")]
        [ForeignKey("MfgRoute")]
        public string mfgRoute { get; set; }

        [Required]
        [DisplayName("CHASSIS NO")]
        public string chasisNo { get; set; }

        [Required]
        [DisplayName("INVOICE NO")]
        public string invoiceNo { get; set; }

        [DisplayName("QUOTATION NO")]
        public string? quotationId { get; set; }


        [DisplayName("QUOTATION NO")]
        [NotMapped]
        public Int64 _Id { get; set; }


        [DisplayName("VC QTY")]
        [NotMapped]
        public decimal qty { get; set; }

        [Required]
        [DisplayName("INVOICE DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime invoiceDate { get; set; }

        [Required]
        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [Required]
        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [Required]
        [DisplayName("FREIGHT VALUE")]
        public decimal totalFreight { get; set; }

        [DisplayName("TEMP.REG.NO")]
        public string? tempRegNo { get; set; }

        [DisplayName("EPOD")]
        public string? epodNo { get; set; }

        [DisplayName("EPOD DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? epodDate { get; set; }
        public DateTime? incidanceDate { get; set; }
        public DateTime? reTransitDate { get; set; }
        //[NotMapped]
        //public DateTime? actualPlantOut { get; set; }
        public int incidanceDelayed { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? closingDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }

        public string? updatedBy { get; set; }
        [NotMapped]
        [Display(Name = "DRIVER NAME")]
        public string? driverName { get; set; }
        [NotMapped]
        [Display(Name = "MOBILE NO")]
        public string? driverMobile { get; set; }
        [NotMapped]
        public List<DropDown> dealerList = new List<DropDown>();

        [NotMapped]
        public List<DropDown> destinations = new List<DropDown>();

        [NotMapped]
        public List<DropDown> quotationList = new List<DropDown>();

        [NotMapped]
        public List<DropDown> vcList = new List<DropDown>();

        [NotMapped]
        public List<DropDown> companyList = new List<DropDown>();

        /// <summary>
        /// Added for checkbox
        /// </summary>
        [NotMapped]
        public bool selected { set; get; }

        [DisplayName("EWAY BILL NO")]
        [NotMapped]
        public string? ewayno { get; internal set; }
    }
    public class MfgRoute
    {
        [Key]
        public Int64 _routId { get; set; }

        [Required]
        [DisplayName("MFG ROUTE")]
        public string mfgRoute { get; set; }

        [Required]
        [DisplayName("BASIC RATE")]
        public decimal basicRate { get; set; }

        [Required]
        [DisplayName("EN-ROUTE")]
        public decimal inroute { get; set; }

        [DisplayName("TOLL EXP.")]
        public decimal tollExp { get; set; }

        public string? status { get; set; }
        [Required]
        [DisplayName("TOTAL EXP.")]
        public decimal totalExp { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

    }
    public class ShipmentClosed
    {

        public ShipmentClosed()
        {
            /*this.reason = string.Empty;
            this.status = string.Empty;
            this.epodNo = string.Empty;
            this.epodDate = null;*/
            this.statusList = new List<DropDown>();
        }

        [Required]
        public Int64 shipmentId { get; set; }

        [Required]
        [DisplayName("SHIPMENT NO")]
        public string shipmentNo { get; set; }

        [DisplayName("EPOD")]
        public string? epodNo { get; set; }

        [DisplayName("DELIVERY DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? epodDate { get; set; }

        [DisplayName("REMARK")]
        public string? reason { get; set; }

        [DisplayName("SHIPMENT STATUS")]
        public string? status { get; set; }
        [NotMapped]
        [DisplayName("PLANT OUT DATE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? plantoutDate { get; set; }

        [DisplayName("EXTRA/POST DELIVERY EXPENSE")]
        public decimal? expenses { get; set; }

        [DisplayName("NARRATION")]
        public string? narration { get; set; }
        [NotMapped]
        [DisplayName("ESTIMATED DATE")]
        [ReadOnly(true)]
        public string? estimatedDate { get; set; }

        [NotMapped]
        public List<DropDown> statusList { get; set; }

    }
    public class PumpMaster
    {
        [Key]
        public Int64 pumpId { get; set; }

        [DisplayName("PUMP NAME")]
        public string pumpName { get; set; }

        [DisplayName("LOCATION")]
        public string location { get; set; }

        [DisplayName("RATE")]
        public decimal rate { get; set; }

        [DisplayName("STATUS")]
        public string? status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? createdDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

    }
    public class BulkInsert
    {
        public BulkInsert()
        {
            this.createdDate = DateTime.Now.Date;
            this.updatedDate = DateTime.Now.Date;
            this.status = shipmentStatus.IN_PLANT.ToString();
        }
        public IFormFile dataFile { get; set; }
        public string? status { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
    }
    public class BulkShipment
    {
        public string shipmentNo { get; set; }
        public string companyId { get; set; }
        public string shipmentDate { get; set; }
        public string dealerName { get; set; }
        public string destination { get; set; }
        public string vcNo { get; set; }
        public string chasisNo { get; set; }
        public string invoiceNo { get; set; }
        public string tempRegNo { get; set; }
        public string remark { get; set; }
    }
    public class BillNumer
    {
        public string prefix { get; set; }
        public string plantCode { get; set; }
        public Int64 serial { get; set; }
    }
    public class QuotationNumer
    {
        public string prefix { get; set; }
        public string plantCode { get; set; }
        public Int64 serial { get; set; }
    }
    public class Company
    {
        [Key]
        public int cid { get; set; }
        public string companyName { get; set; }
    }
}