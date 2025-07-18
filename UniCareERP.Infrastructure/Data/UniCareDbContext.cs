using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Domain.Entities.Inventory;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.Procedures;

namespace UniCareERP.Infrastructure.Data
{
    public class UniCareDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options)
            : base(options)
        {
        }

        // Patient & Clinical Management
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!;
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<ProcedureCharge> ProcedureCharges { get; set; } = null!;

        // Finance Management
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<GeneralLedgerAccount> GeneralLedgerAccounts { get; set; } = null!;
        public DbSet<PatientPayment> PatientPayments { get; set; } = null!;
        public DbSet<PatientRefund> PatientRefunds { get; set; } = null!;
        public DbSet<PatientAccount> PatientAccounts { get; set; } = null!;

        // HR Management
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
        public DbSet<Payroll> Payrolls { get; set; } = null!;
        public DbSet<Payslip> Payslips { get; set; } = null!;
        public DbSet<SalaryStructure> SalaryStructures { get; set; } = null!;

        // Inventory Management
        public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;


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
                 .HasForeignKey(pc => pc.ProcedureId);

                b.HasOne(pc => pc.Invoice)
                 .WithMany(i => i.ProcedureCharges)
                 .HasForeignKey(pc => pc.InvoiceId)
                 .IsRequired(false);
            });
        }
    }
}
