using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniCareERP.Domain.Entities;
using UniCareERP.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection; // Required for CreateScope
using Microsoft.Extensions.Logging; // Required for ILogger

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<UniCareDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Set to true for production if email confirmation is used
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<UniCareDbContext>()
    .AddDefaultTokenProviders(); // Required for features like password reset tokens

// Register application services
builder.Services.AddScoped<UniCareERP.Application.Services.IUserService, UniCareERP.Application.Services.UserService>();
builder.Services.AddScoped<UniCareERP.Application.Services.IRoleService, UniCareERP.Application.Services.RoleService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Patients.IPatientService, UniCareERP.Application.Services.Patients.PatientService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Appointments.IAppointmentService, UniCareERP.Application.Services.Appointments.AppointmentService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Finance.IInvoiceService, UniCareERP.Application.Services.Finance.InvoiceService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Finance.IGeneralLedgerService, UniCareERP.Application.Services.Finance.GeneralLedgerService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Inventory.IInventoryService, UniCareERP.Application.Services.Inventory.InventoryService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Inventory.ISaleService, UniCareERP.Application.Services.Inventory.SaleService>();
builder.Services.AddScoped<UniCareERP.Application.Services.HR.IEmployeeService, UniCareERP.Application.Services.HR.EmployeeService>();
builder.Services.AddScoped<UniCareERP.Application.Services.HR.ILeaveRequestService, UniCareERP.Application.Services.HR.LeaveRequestService>();
builder.Services.AddScoped<UniCareERP.Application.Services.HR.IPayrollService, UniCareERP.Application.Services.HR.PayrollService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Patients.IPrescriptionService, UniCareERP.Application.Services.Patients.PrescriptionService>(); // Added PrescriptionService
builder.Services.AddScoped<UniCareERP.Application.Services.Dashboard.IDashboardService, UniCareERP.Application.Services.Dashboard.DashboardService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Pharmacy.IPharmacyService, UniCareERP.Application.Services.Pharmacy.PharmacyService>();
builder.Services.AddScoped<UniCareERP.Infrastructure.Repositories.Pharmacy.IDrugRepository, UniCareERP.Infrastructure.Repositories.Pharmacy.DrugRepository>();
builder.Services.AddScoped<UniCareERP.Infrastructure.Repositories.Pharmacy.IPrescriptionRepository, UniCareERP.Infrastructure.Repositories.Pharmacy.PrescriptionRepository>();
builder.Services.AddScoped<UniCareERP.Application.Services.Lab.ILabService, UniCareERP.Application.Services.Lab.LabService>();
builder.Services.AddScoped<UniCareERP.Application.Services.Radiology.IRISService, UniCareERP.Application.Services.Radiology.RISService>();

builder.Services.AddControllersWithViews();

// Configure application cookie settings (important for login/logout behavior)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Path to the login page
    options.LogoutPath = "/Account/Logout"; // Path to the logout page
    options.AccessDeniedPath = "/Account/AccessDenied"; // Path if authorization fails
    options.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure UseAuthentication is added
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedDataLogger = services.GetRequiredService<ILogger<SeedDataRunner>>();
    var appLogger = services.GetRequiredService<ILogger<Program>>(); // For general app logging if needed elsewhere
    try
    {
        // Ensure database is created.
        // For production, use migrations. For development, EnsureCreated can be useful.
        // var dbContext = services.GetRequiredService<UniCareDbContext>();
        // await dbContext.Database.MigrateAsync(); // Apply migrations
        // await dbContext.Database.EnsureCreatedAsync(); // Or use this for quick dev setup without migrations

        await SeedData.Initialize(services, seedDataLogger);
        appLogger.LogInformation("Data seeding process completed.");
    }
    catch (Exception ex)
    {
        appLogger.LogError(ex, "An error occurred while seeding the database.");
    }
}


app.Run();
