using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using Trans9.Models;

namespace Trans9.DataAccess
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .LogTo(Console.WriteLine)
        .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Shipment>(entity =>
            {
                entity.HasIndex(u => u.shipmentNo).IsUnique();
                //entity.HasIndex(u => u.chasisNo).IsUnique();
                entity.HasIndex(u => u.invoiceNo).IsUnique();

                entity.Property(e => e.epodNo).HasColumnName("epodNo").HasConversion<string?>();
                entity.Property(e => e.epodDate).HasColumnName("epodDate").HasConversion<DateTime?>();
                entity.Property(e => e.reTransitDate).HasColumnName("reTransitDate").HasConversion<DateTime?>();
                entity.Property(e => e.incidanceDate).HasColumnName("incidanceDate").HasConversion<DateTime?>();
            });

            builder.Entity<PlantOut>(entity =>
            {
                entity.Property(e => e.ewayNo).HasColumnName("ewayNo").HasConversion<string?>();
                entity.Property(e => e.expDate).HasColumnName("expDate").HasConversion<DateTime?>();
                entity.Property(e => e.createdDate).HasColumnName("createdDate").HasConversion<DateTime?>();
                entity.Property(e => e.reason).HasColumnName("reason").HasConversion<string?>();
                entity.Property(e => e.updatedBy).HasColumnName("updatedBy").HasConversion<string?>();
            });

            builder.Entity<Driver>(entity =>
            {
                entity.HasIndex(u => u.dlNo).IsUnique();
                entity.HasIndex(u => u.aadharNo).IsUnique();
                //entity.HasIndex(u => u.mobileNo).IsUnique();
            });

            builder.Entity<Company>(entity =>
            {
                entity.HasIndex(u => u.cid).IsUnique();
            });

            builder.Entity<Quotation>(entity =>
            {
                entity.HasIndex(u => u.quoteId).IsUnique();
            });

            builder.Entity<MfgRoute>(entity =>
            {
                entity.HasIndex(u => u.mfgRoute).IsUnique();
                entity.Property(x => x.basicRate).HasPrecision(16, 2);
                entity.Property(x => x.inroute).HasPrecision(16, 2);
                entity.Property(x => x.tollExp).HasPrecision(16, 2);
                entity.Property(x => x.totalExp).HasPrecision(16, 2);
            });

            builder.Entity<VcMaster>(entity =>
            {
                entity.HasIndex(u => u.vcNo).IsUnique();
                entity.Property(x => x.tKmpl).HasPrecision(16, 2);
                entity.Property(x => x.sblKmpl).HasPrecision(16, 2);
            });

            builder.Entity<Voucher>(entity =>
            {
                entity.Property(x => x.amount).HasPrecision(16, 2);
                entity.Property(x => x.payPercentage).HasPrecision(16, 0);
            });

            builder.Entity<PumpMaster>(entity =>
            {
                entity.Property(x => x.rate).HasPrecision(16, 2);
            });

            builder.Entity<Billing>(entity =>
            {
                entity.Property(x => x.taxableAmount).HasPrecision(16, 2);
                entity.Property(x => x.gstAmount).HasPrecision(16, 2);
                entity.Property(x => x.tdsAmount).HasPrecision(16, 2);
                //entity.Property(x => x.epcAmount).HasPrecision(16, 2);
                entity.Property(x => x.payableAmount).HasPrecision(16, 2);
                entity.Property(x => x.attachments).HasColumnName("attachments").HasConversion<string?>();
            });

            builder.Entity<Incidence>(entity =>
            {
                entity.Property(e => e.tempRegNo).HasColumnName("tempRegNo").HasConversion<string?>();
                entity.Property(e => e.engineNo).HasColumnName("engineNo").HasConversion<string?>();
                entity.Property(e => e.immediateAction).HasColumnName("immediateAction").HasConversion<string?>();
                entity.Property(e => e.panchnamaHeld).HasColumnName("panchnamaHeld").HasConversion<string?>();
                entity.Property(e => e.driverReleased).HasColumnName("driverReleased").HasConversion<string?>();
                entity.Property(e => e.thirdPartyInvolved).HasColumnName("thirdPartyInvolved").HasConversion<string?>();
                entity.Property(e => e.vcHandedOver).HasColumnName("vcHandedOver").HasConversion<string?>();
                entity.Property(e => e.pcStationName).HasColumnName("pcStationName").HasConversion<string?>();
                entity.Property(e => e.ChassisReleased).HasColumnName("ChassisReleased").HasConversion<string?>();
                entity.Property(e => e.insSurveyDone).HasColumnName("insSurveyDone").HasConversion<string?>();
                entity.Property(e => e.complaintNo).HasColumnName("complaintNo").HasConversion<string?>();
                entity.Property(e => e.attachments).HasColumnName("attachments").HasConversion<string?>();
            });
            
            builder.Entity<Authority>(entity =>
            {
                entity.Property(e => e.tassName).HasColumnName("tassName").HasConversion<string?>();
                entity.Property(e => e.tassLocation).HasColumnName("tassLocation").HasConversion<string?>();
            });

            builder.Entity<Marching>(entity =>
            {
                entity.Property(x => x.totalHsd).HasPrecision(16, 2);
                entity.Property(x => x.spdQty).HasPrecision(16, 2);
                entity.Property(x => x.spdRate).HasPrecision(16, 2);
                entity.Property(x => x.spdAmount).HasPrecision(16, 2);
                entity.Property(x => x.inRouteExp).HasPrecision(16, 2);
                entity.Property(x => x.tollExp).HasPrecision(16, 2);
                entity.Property(x => x.extraAmt).HasPrecision(16, 2);
                entity.Property(x => x.loadingCharge).HasPrecision(16, 2);
                entity.Property(x => x.driverPayment).HasPrecision(16, 2);
                entity.Property(x => x.totalExp).HasPrecision(16, 2);
                entity.Property(x => x.remainBalance).HasPrecision(16, 2);
                entity.Property(x => x.spdPaid).HasColumnName("spdPaid").HasConversion<int>();
            });

            builder.Entity<DieselPayment>(entity =>
            {
                entity.Property(x => x.prevBalance).HasPrecision(16, 2);
                entity.Property(x => x.currentAmount).HasPrecision(16, 2);
                entity.Property(x => x.payableAmount).HasPrecision(16, 2);
                entity.Property(x => x.paidAmount).HasPrecision(16, 2);
                entity.Property(x => x.balanceAmount).HasPrecision(16, 2);
                entity.Property(x => x.spdQty).HasPrecision(16, 2);
                entity.Property(x => x.spdRate).HasPrecision(16, 2);
                entity.Property(x => x.spdAmount).HasPrecision(16, 2);
                entity.Property(x => x.remark).HasColumnName("remark").HasConversion<string?>();
                entity.Property(x => x.vouchers).HasColumnName("vouchers").HasConversion<string?>();
            });

            builder.Entity<InchargePayment>(entity =>
            {
                entity.Property(x => x.receivedAmount).HasPrecision(16, 2);
                entity.Property(x => x.paidAmount).HasPrecision(16, 2);
                entity.Property(x => x.balanceAmount).HasPrecision(16, 2);
                entity.Property(x => x.remark).HasColumnName("remark").HasConversion<string?>();
                entity.Property(x => x.referenceNo).HasColumnName("referenceNo").HasConversion<int>();
            });

            builder.Entity<BillRow>().HasNoKey().ToView("tbl_billing");

            builder.Entity<ShipmentReport>().HasNoKey();

            builder.Entity<Employees>(entity =>
            {
                entity.Property(e => e.dob).HasColumnName("dob").HasConversion<DateTime?>();
                entity.Property(e => e.gender).HasColumnName("gender").HasConversion<string?>();
                entity.Property(e => e.email).HasColumnName("email").HasConversion<string?>();
                entity.Property(e => e.phoneNo).HasColumnName("phoneNo").HasConversion<string?>();
                entity.Property(e => e.address).HasColumnName("address").HasConversion<string?>();
                entity.Property(e => e.city).HasColumnName("city").HasConversion<string?>();
                entity.Property(e => e.state).HasColumnName("state").HasConversion<string?>();
                entity.Property(e => e.postalCode).HasColumnName("postalCode").HasConversion<string?>();
                entity.Property(e => e.country).HasColumnName("country").HasConversion<string?>();
                entity.Property(e => e.emergencyContactName).HasColumnName("emergencyContactName").HasConversion<string?>();
                entity.Property(e => e.emergencyContactPhone).HasColumnName("emergencyContactPhone").HasConversion<string?>();
            });

            builder.Entity<Payroll>(entity =>
            {
                entity.Property(x => x.basic).HasPrecision(16, 2);
                entity.Property(x => x.hra).HasPrecision(16, 2);
                entity.Property(x => x.lta).HasPrecision(16, 2);
                entity.Property(x => x.conveniance).HasPrecision(16, 2);
                entity.Property(x => x.oa).HasPrecision(16, 2);
                entity.Property(x => x.hic).HasPrecision(16, 2);
                entity.Property(x => x.pt).HasPrecision(16, 2);
                entity.Property(x => x.pf).HasPrecision(16, 2);
                entity.Property(x => x.od).HasPrecision(16, 2);
            });

            builder.Entity<Payslip>(entity =>
            {
                entity.Property(x => x.basic).HasPrecision(16, 2);
                entity.Property(x => x.hra).HasPrecision(16, 2);
                entity.Property(x => x.lta).HasPrecision(16, 2);
                entity.Property(x => x.conveniance).HasPrecision(16, 2);
                entity.Property(x => x.oa).HasPrecision(16, 2);
                entity.Property(x => x.hic).HasPrecision(16, 2);
                entity.Property(x => x.pt).HasPrecision(16, 2);
                entity.Property(x => x.pf).HasPrecision(16, 2);
                entity.Property(x => x.od).HasPrecision(16, 2);
                entity.Property(x => x.taxable).HasPrecision(16, 2);
                entity.Property(x => x.netSalary).HasPrecision(16, 2);
                entity.Property(x => x.taxRate).HasPrecision(16, 2);
                entity.Property(x => x.workedDays).HasPrecision(16, 2);
            });

        }

        public DbSet<Dealer> Dealer { get; set; } = null!;
        public DbSet<Destination> Destination { get; set; } = null!;
        public DbSet<Driver> Driver { get; set; } = null!;
        public DbSet<MfgRoute> MfgRoute { get; set; } = null!;
        public DbSet<Shipment> Shipment { get; set; } = null!;
        public DbSet<PlantOut> PlantOut { get; set; } = null!;
        public DbSet<VcMaster> VcMaster { get; set; } = null!;
        public DbSet<Voucher> Voucher { get; set; } = null!;
        public DbSet<PumpMaster> PumpMaster { get; set; } = null!;
        public DbSet<Billing> Billing { get; set; } = null!;
        public DbSet<Marching> Marching { get; set; } = null!;
        public DbSet<Incidence> Incidence { get; set; } = null!;
        public DbSet<Attachments> Attachments { get; set; } = null!;
        public DbSet<Authority> tbl_authority { get; set; } = null!;
        public DbSet<BillRow> BillRows { get; set; } = null!;
        public DbSet<Company> Company { get; set; } = null!;
        public DbSet<Quotation> Quotations { get; set; } = null!;
        public DbSet<QuoteDetail> QuoteDetails { get; set; } = null!;
        public DbSet<QuoteDetailDto> TempQuoteDetail { get; set; } = null!;
        public DbSet<IbBilling> IbBilling { get; set; } = null!;
        public DbSet<DieselPayment> DieselPayment { get; set; } = null!;
        public DbSet<InchargePayment> InchargePayment { get; set; } = null!;
        public DbSet<Employees> Employees { get; set; } = null!;
        public DbSet<Payroll> Payroll { get; set; } = null!;
        public DbSet<Payslip> Payslip { get; set; } = null!;
    }
}
