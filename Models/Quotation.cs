using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{
    public partial class Quotation
    {

        [Key]
        public string quoteId { get; set; }
        public string quoteNo { get; set; }
        public string quoteFor { get; set; }
        public DateTime quoteDate { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public DateTime updatedDate { get; set; }
        public string updatedBy { get; set; }
        public List<QuoteDetail> QuoteDetails = new List<QuoteDetail>();
    }

    public partial class QuoteDetail
    {
        [Key]
        public string Id { get; set; }
        [DisplayName("FROM")]
        public string source { get; set; }

        [DisplayName("DESTINATION")]
        public string destination { get; set; }

        [DisplayName("TOTAL KMS")]
        public decimal totalkms { get; set; }

        [DisplayName("MODEL DESCRIPTION")]
        public string modelDesc { get; set; }
        [DisplayName("TML APPROVED RATE")]
        public decimal tmlRate { get; set; }

        [DisplayName("UNITS")]
        public decimal qty { get; set; }

        [DisplayName("SBL DISCOUNTED RATE")]
        public decimal sblRate { get; set; }

        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [DisplayName("EN-ROUTE")]
        public decimal enRoute { get; set; }

        [DisplayName("TOTAL FREIGHT")]
        public decimal totalFreight { get; set; }
        public string quoteId { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public DateTime updatedDate { get; set; }
        public string updatedBy { get; set; }
        //public virtual Quotation Quotation { get; set; }
    }

    #region "Quotation List"
    public class Quotes {
        public string quoteId { get; set; }
        [DisplayName("QUOTE NO")]
        public string quoteNo { get; set; }
        [DisplayName("QUOTE FOR")]
        public string quoteFor { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("QUOTE DATE")]
        public DateTime quoteDate { get; set; }

        [DisplayName("QTY")]
        public decimal qty { get; set; }
        [DisplayName("BASIC FREIGHT")]
        public decimal basicFreight { get; set; }

        [DisplayName("ENROUTE")]
        public decimal enRoute { get; set; }

        [DisplayName("TOTAL FREIGHT")]
        public decimal totalFreight { get; set; }
    }
    #endregion "Quotation List"


    #region "Quotation DTO"
    [Table("tempquotedetails")]
    public class QuoteDetailDto
    {
        [Key]
        public string? Id { get; set; }
        public string quoteId { get; set; }
        public string source { get; set; }
        public Int64 destination { get; set; }
        public decimal totalKms { get; set; }
        public decimal qty { get; set; }
        public string modelDesc { get; set; }
        public decimal tmlRate { get; set; }
        public decimal sblRate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal totalFreight { get; set; }
        public string? userid { get; set; }
    }

    public class QuoteDetailShow
    {
        [Key]
        public string? Id { get; set; }
        public string quoteId { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public decimal totalKms { get; set; }
        public decimal qty { get; set; }
        public string modelDesc { get; set; }
        public decimal tmlRate { get; set; }
        public decimal sblRate { get; set; }
        public decimal basicFreight { get; set; }
        public decimal enRoute { get; set; }
        public decimal totalFreight { get; set; }
        public string? userid { get; set; }
    }

    public class QuoteDto
    {
        public QuoteDto()
        {
            this.quoteDate = DateTime.Now.Date;
        }
        public string quoteId { get; set; }
        public string quoteNo { get; set; }
        public string quoteFor { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime quoteDate { get; set; }

        public List<QuoteDetailShow> details = new List<QuoteDetailShow>();

        public List<DropDown> destinationList = new List<DropDown>();

        public List<DropDown> vcList = new List<DropDown>();
    }

    #endregion

    #region "REPORT"
    public class QuoteReport {
        public Int64 SR_NO { get; set; }
        public string FROM { get; set; }
        public string DESTINATION { get; set; }
        public Int64 TOTAL_KMS { get; set; }
        public string MODEL_DESCRIPTION { get; set; }
        public decimal TML_APPROVED_RATE { get; set; }
        public decimal SBL_DISCOUNTED_RATE { get; set; }
        public decimal BASIC_FREIGHT { get; set; }
        public decimal ENROUTE { get; set; }
        public decimal TOTAL_FREIGHT_CHARGES { get; set; }
    }
    #endregion



    #region "VCLIST FOR ALL LOCATION"
    public class VcInfo
    {
        public string? quoteId { get; set; }
        public string vcNo { get; set; }
        public string modelDesc { get; set; }
        public string mfgCode { get; set; }
        public string plantCode { get; set; }
        public string plantDesc { get; set; }
        public string? qty { get; set; }
        public string? balance { get; set; }
    }
    #endregion
}
