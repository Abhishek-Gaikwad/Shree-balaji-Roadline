using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Trans9.Models;

namespace Trans9.DataAccess
{
    public class StoredProcedureDbContext : DbContext
    {
        public StoredProcedureDbContext(DbContextOptions<StoredProcedureDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .LogTo(Console.WriteLine)
        .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //SHIPMENT REPORT ENTITY
            builder.Entity<ShipmentReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.dealerName);
                entity.Property(e => e.destination);
                entity.Property(e => e.region);
                entity.Property(e => e.vcNo);
                entity.Property(e => e.modelDescription);
                entity.Property(e => e.chassisNo);
                entity.Property(e => e.ewayBillNo);
                entity.Property(e => e.invoiceDate);
                entity.Property(e => e.invoiceNo);
                entity.Property(e => e.currentStatus);
                entity.Property(e => e.plantOutDate);
                entity.Property(e => e.standardTransitTime);
                entity.Property(e => e.lastEstimatedDate);
                entity.Property(e => e.deliveryDate);
                entity.Property(e => e.reachDate);
                entity.Property(e => e.plantCode);
                entity.Property(e => e.plantDescription);
                entity.Property(e => e.createdDate);
                entity.Property(e => e.epodNo);
                entity.Property(e => e.epodDate);
                entity.Property(e => e.tempRegNo);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.ewayExpDate);
            });

            //LOADING CHARGES REPORT ENTITY
            builder.Entity<LoadingChargesReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.destination);
                entity.Property(e => e.plantCode);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.chassisNo);
                entity.Property(e => e.plantOutDate);
                entity.Property(e => e.charges);
            });

            //BILLING REPORT ENTITY
            builder.Entity<BillingReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.billNo);
                entity.Property(e => e.billDate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.discount);
                entity.Property(e => e.taxableValue);
                entity.Property(e => e.gstAmount);
                entity.Property(e => e.tmlGstNo);
                entity.Property(e => e.invoiceAmount);
                entity.Property(e => e.invoiceStatus);
                entity.Property(e => e.receivedAmount);
                entity.Property(e => e.tdsAmount);
                entity.Property(e => e.irnNo);
                entity.Property(e => e.referenceNo);
                entity.Property(e => e.paymentReceivedDate);
            });

            //FBV BILLING REPORT ENTITY
            builder.Entity<FbvBilling>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.billId);
                entity.Property(e => e.billNo);
                entity.Property(e => e.shipmentId);
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.vcNo);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.mfgCode);
                entity.Property(e => e.plantCode);
                entity.Property(e => e.plantDesc);
                entity.Property(e => e.chasisNo);
                entity.Property(e => e.invoiceNo);
                entity.Property(e => e.invoiceDate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.trasitDays);
                entity.Property(e => e.receivedAmount);
                entity.Property(e => e.attachments).HasColumnName("attachments").HasConversion<string?>();
            });

            //FBV INVOICE REPORT ENTITY
            builder.Entity<FbvInvoice>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.billNo);
                entity.Property(e => e.billingDate);
                entity.Property(e => e.plantCode);
                entity.Property(e => e.plantDesc);
                entity.Property(e => e.chasisNo);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.irnNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.epcAmount);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.taxableAmount);
                entity.Property(e => e.gstAmount);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.destination);
                entity.Property(e => e.amountInWords);
            });

            //EWAY EXPIRY REPORT ENTITY
            builder.Entity<EwayExpiryReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.tempRegNo);
                entity.Property(e => e.driverIncharge);
                entity.Property(e => e.driverName);
                entity.Property(e => e.mobileNo);
                entity.Property(e => e.ewayNo);
                entity.Property(e => e.expDate);
                entity.Property(e => e.ewayId);
            });

            builder.Entity<EstimatedDateReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.tempRegNo);
                entity.Property(e => e.inchargeId);
                entity.Property(e => e.driverName);
                entity.Property(e => e.mobileNo);
                entity.Property(e => e.expDate);
                entity.Property(e => e.datediff);
            });

            //QueryResult ENTITY
            builder.Entity<QueryResult>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.errorCode);
                entity.Property(e => e.id);
                entity.Property(e => e.Message);
            });

            builder.Entity<QuoteReport>(entity =>
            {
                entity.HasNoKey();
            });

            //QueryResult ENTITY
            builder.Entity<QuoteDetailDto>(entity =>
            {
                entity.HasKey("Id");
                entity.Property(e => e.Id);
                entity.Property(e => e.source);
                entity.Property(e => e.destination);
                entity.Property(e => e.totalKms);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.tmlRate);
                entity.Property(e => e.sblRate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.qty);
                entity.Property(e => e.quoteId);
                entity.Property(e => e.userid);
            });

            //QueryResult ENTITY
            builder.Entity<QuoteDetailShow>(entity =>
            {
                entity.HasKey("Id");
                entity.Property(e => e.Id);
                entity.Property(e => e.source);
                entity.Property(e => e.destination);
                entity.Property(e => e.totalKms);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.tmlRate);
                entity.Property(e => e.sblRate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.qty);
                entity.Property(e => e.quoteId);
                entity.Property(e => e.userid);
            });

            //QueryResult ENTITY
            builder.Entity<Quotes>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.quoteId);
                entity.Property(e => e.quoteNo);
                entity.Property(e => e.quoteFor);
                entity.Property(e => e.quoteDate);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.qty);
            });

            //QueryResult ENTITY
            builder.Entity<VcInfo>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.quoteId);
                entity.Property(e => e.vcNo);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.mfgCode);
                entity.Property(e => e.plantCode);
                entity.Property(e => e.plantDesc);
                entity.Property(e => e.qty);
                entity.Property(e => e.balance);
            });

            //QueryResult ENTITY
            builder.Entity<IbBilling>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id);
                entity.Property(e => e.invoiceNo);
                entity.Property(e => e.invoiceDate);
                entity.Property(e => e.poNumber);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.gst);
                entity.Property(e => e.totalFreight);
                entity.Property(e => e.shipments);
                entity.Property(e => e.createdDate);
                entity.Property(e => e.createdBy);
                entity.Property(e => e.updatedDate);
                entity.Property(e => e.updatedBy);
                entity.Property(e => e.attachments).HasColumnName("attachments").HasConversion<string?>();
            });

            //QueryResult ENTITY
            builder.Entity<QuoteShipment>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id);
                entity.Property(e => e.tptCode);
                entity.Property(e => e.tptName);
                entity.Property(e => e.tradePlateNo);
                entity.Property(e => e.source);
                entity.Property(e => e.destination);
                entity.Property(e => e.chassisNo);
                entity.Property(e => e.model);
                entity.Property(e => e.stmNo);
                entity.Property(e => e.stmDate);
                entity.Property(e => e.gpDate);
                entity.Property(e => e.marchDate);
                entity.Property(e => e.reachDate);
                entity.Property(e => e.basic);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.vcNo);
            });

            builder.Entity<IbBillingShipment>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.quoteId);
                entity.Property(e => e.quoteNo);
                entity.Property(e => e.shipmentId);
                entity.Property(e => e.shipmentNo);
                entity.Property(e => e.shipmentDate);
                entity.Property(e => e.vcNo);
                entity.Property(e => e.modelDesc);
                entity.Property(e => e.chasisNo);
                entity.Property(e => e.invoiceNo);
                entity.Property(e => e.invoiceDate);
                entity.Property(e => e.trasitDays);
                entity.Property(e => e.basicFreight);
                entity.Property(e => e.enRoute);
                entity.Property(e => e.totalFreight);
            });

            builder.Entity<DieselPaymentCalc>(entity =>
            {
                entity.HasNoKey();
                entity.Property(x => x.prevBalance).HasPrecision(16, 2);
                entity.Property(x => x.currentAmount).HasPrecision(16, 2);
                entity.Property(x => x.payableAmount).HasPrecision(16, 2);
                entity.Property(x => x.paidAmount).HasPrecision(16, 2);
                entity.Property(x => x.balanceAmount).HasPrecision(16, 2);
                entity.Property(x => x.spdQty).HasPrecision(16, 2);
                entity.Property(x => x.spdRate).HasPrecision(16, 2);
                entity.Property(x => x.spdAmount).HasPrecision(16, 2);
            });

            builder.Entity<PayslipRes>(entity =>
            {
                entity.HasNoKey();
                entity.Property(x => x.payId);
                entity.Property(x => x.employeeId);
                entity.Property(x => x.employeeName);
                entity.Property(x => x.month);
                entity.Property(x => x.year);
                entity.Property(x => x.paymentDate);
                entity.Property(x => x.earning).HasPrecision(16, 2);
                entity.Property(x => x.deduction).HasPrecision(16, 2);
                entity.Property(x => x.workedDays).HasPrecision(16, 2);
                entity.Property(x => x.netSalary).HasPrecision(16, 2);
            });
        }

        public virtual DbSet<ShipmentReport> ShipmentReport { get; set; } = null!;
        public virtual DbSet<LoadingChargesReport> LoadingChargesReport { get; set; } = null!;
        public virtual DbSet<EwayExpiryReport> EwayExpiryReport { get; set; } = null!;
        public virtual DbSet<EstimatedDateReport> EstimatedDateReport { get; set; } = null!;
        public virtual DbSet<BillingReport> BillingReport { get; set; } = null!;
        public virtual DbSet<FbvBilling> FbvBilling { get; set; } = null!;
        public virtual DbSet<FbvInvoice> FbvInvoice { get; set; } = null!;
        public virtual DbSet<QueryResult> QueryResult { get; set; } = null!;
        public DbSet<QuoteDetailDto> TempQuoteDetails { get; set; } = null!;
        public DbSet<Quotes> Quotes { get; set; } = null!;
        public DbSet<QuoteReport> QuoteReport { get; set; } = null!;
        public DbSet<QuoteDetailShow> QuoteDetailShow { get; set; } = null!;
        public DbSet<VcInfo> VcInfo { get; set; } = null!;
        public DbSet<IbBilling> IbBilling { get; set; } = null!;
        public DbSet<QuoteShipment> QuoteShipments { get; set; } = null!;
        public DbSet<IbBillingShipment> IbBillingShipments { get; set; } = null!;
        public DbSet<DieselPaymentCalc> DieselPaymentBill { get; set; } = null!;

        public DbSet<PayslipRes> PayslipRes { get; set; } = null!;
    }
}
