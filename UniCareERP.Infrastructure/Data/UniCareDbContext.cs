using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities;
using UniCareERP.Domain.Entities.Patients;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Domain.Entities.HR;
using UniCareERP.Domain.Entities.Inventory;

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

        // Finance Management
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<GeneralLedgerAccount> GeneralLedgerAccounts { get; set; } = null!;
        public DbSet<GeneralLedgerTransaction> GeneralLedgerTransactions { get; set; } = null!;

        // HR Management
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;

        // Inventory Management
        public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;


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
        }
    }
}
