using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.Procedures;
using UniCareERP.Domain.Entities.Lab;
using UniCareERP.Domain.Entities.Radiology;

namespace UniCareERP.Infrastructure.Data
{
    public class UniCareDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options)
            : base(options)
        {
        }

        // Radiology Management
        public virtual DbSet<RadiologyOrder> RadiologyOrders { get; set; } = null!;
        public virtual DbSet<RadiologyTest> RadiologyTests { get; set; } = null!;
        public virtual DbSet<RadiologyImage> RadiologyImages { get; set; } = null!;
        public virtual DbSet<RadiologyReport> RadiologyReports { get; set; } = null!;

        // Patient & Clinical Management
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;
        public virtual DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!;
        public virtual DbSet<Procedure> Procedures { get; set; } = null!;
        public virtual DbSet<ProcedureCharge> ProcedureCharges { get; set; } = null!;

        // Finance Management
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<GeneralLedgerAccount> GeneralLedgerAccounts { get; set; } = null!;
        public virtual DbSet<PatientPayment> PatientPayments { get; set; } = null!;
        public virtual DbSet<PatientRefund> PatientRefunds { get; set; } = null!;
        public virtual DbSet<PatientAccount> PatientAccounts { get; set; } = null!;

        // HR Management
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public virtual DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
        public virtual DbSet<Payroll> Payrolls { get; set; } = null!;
        public virtual DbSet<Payslip> Payslips { get; set; } = null!;
        public virtual DbSet<SalaryStructure> SalaryStructures { get; set; } = null!;

        // Inventory Management
        public virtual DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public virtual DbSet<StockTransaction> StockTransactions { get; set; } = null!;
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public virtual DbSet<Sale> Sales { get; set; } = null!;

        // Pharmacy Management
        public virtual DbSet<UniCareERP.Domain.Entities.Pharmacy.Drug> Drugs { get; set; } = null!;
        public virtual DbSet<UniCareERP.Domain.Entities.Pharmacy.Prescription> PharmacyPrescriptions { get; set; } = null!;
        public virtual DbSet<UniCareERP.Domain.Entities.Pharmacy.PrescriptionItem> PharmacyPrescriptionItems { get; set; } = null!;

        // Laboratory Management
        public virtual DbSet<LabTest> LabTests { get; set; } = null!;
        public virtual DbSet<LabOrder> LabOrders { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.FirstName).HasMaxLength(100);
                b.Property(u => u.LastName).HasMaxLength(100);
                b.Property(u => u.EmployeeId).HasMaxLength(50);
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.Property(r => r.Description).HasMaxLength(255);
            });

            // Configure decimal properties
            builder.Entity<GeneralLedgerAccount>().Property(p => p.Balance).HasPrecision(18, 2);
            builder.Entity<LeaveBalance>().Property(p => p.EntitledDays).HasPrecision(18, 2);
            builder.Entity<LeaveBalance>().Property(p => p.UsedDays).HasPrecision(18, 2);
            builder.Entity<Payroll>().Property(p => p.GrossSalary).HasPrecision(18, 2);
            builder.Entity<Payroll>().Property(p => p.NetSalary).HasPrecision(18, 2);
            builder.Entity<Payroll>().Property(p => p.TotalDeductions).HasPrecision(18, 2);
            builder.Entity<Payslip>().Property(p => p.Amount).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.BasicSalary).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.ConveyanceAllowance).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.HouseRentAllowance).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.MedicalAllowance).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.ProfessionalTax).HasPrecision(18, 2);
            builder.Entity<SalaryStructure>().Property(p => p.ProvidentFund).HasPrecision(18, 2);
            builder.Entity<InventoryItem>().Property(p => p.CostPrice).HasPrecision(18, 2);
            builder.Entity<InventoryItem>().Property(p => p.UnitPrice).HasPrecision(18, 2);

            builder.Entity<ProcedureCharge>(b =>
            {
                b.ToTable("ProcedureCharges");
                b.HasKey(pc => pc.Id);
                b.Property(pc => pc.Amount).HasColumnType("decimal(18,2)");

                b.HasOne(pc => pc.Procedure)
                 .WithMany(p => p.ProcedureCharges)
                 .HasForeignKey(pc => pc.ProcedureId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(pc => pc.Invoice)
                 .WithMany(i => i.ProcedureCharges)
                 .HasForeignKey(pc => pc.InvoiceId)
                 .IsRequired(false);

                // Add this to prevent cascade delete from Patient
                b.HasOne(pc => pc.Patient)
                 .WithMany() // No navigation property on Patient back to ProcedureCharge
                 .HasForeignKey(pc => pc.PatientId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
