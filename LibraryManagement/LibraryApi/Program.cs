using LibraryApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Repositories.Implementations;
using LibraryApi.Services.Interfaces;
using LibraryApi.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using LibraryApi.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddDataProtection();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>() // Indispensable pour RoleManager
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();


builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Books.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Books.View")));

    options.AddPolicy("Books.Create",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Books.Create")));

    options.AddPolicy("Books.Edit",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Books.Edit")));

    options.AddPolicy("Books.Delete",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Books.Delete")));

    options.AddPolicy("Students.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Students.View")));

    options.AddPolicy("Students.Create", policy => policy.Requirements.Add(new PermissionRequirement("Students.Create")));
    options.AddPolicy("Students.Edit", policy => policy.Requirements.Add(new PermissionRequirement("Students.Edit")));
    options.AddPolicy("Students.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Students.Delete")));

    options.AddPolicy("Authors.View", policy => policy.Requirements.Add(new PermissionRequirement("Authors.View")));
    options.AddPolicy("Authors.Create", policy => policy.Requirements.Add(new PermissionRequirement("Authors.Create")));
    options.AddPolicy("Authors.Edit", policy => policy.Requirements.Add(new PermissionRequirement("Authors.Edit")));
    options.AddPolicy("Authors.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Authors.Delete")));

    options.AddPolicy("Categories.View", policy => policy.Requirements.Add(new PermissionRequirement("Categories.View")));
    options.AddPolicy("Categories.Create", policy => policy.Requirements.Add(new PermissionRequirement("Categories.Create")));
    options.AddPolicy("Categories.Edit", policy => policy.Requirements.Add(new PermissionRequirement("Categories.Edit")));
    options.AddPolicy("Categories.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Categories.Delete")));

    options.AddPolicy("Employees.Create", policy => policy.Requirements.Add(new PermissionRequirement("Employees.Create")));
    options.AddPolicy("Employees.Edit", policy => policy.Requirements.Add(new PermissionRequirement("Employees.Edit")));
    options.AddPolicy("Employees.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Employees.Delete")));

    options.AddPolicy("Loans.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Loans.View")));

    options.AddPolicy("Loans.Create",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Loans.Create")));

    options.AddPolicy("Loans.Return",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Loans.Return")));

    options.AddPolicy("Subscriptions.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Subscriptions.View")));

    options.AddPolicy("Employees.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Employees.View")));

    options.AddPolicy("Reports.View",
        policy => policy.Requirements.Add(
            new PermissionRequirement("Reports.View")));
});

builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();



builder.Services.AddHttpClient<
    ISupabaseStorageService,
    SupabaseStorageService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await PermissionSeeder.SeedAsync(context);
}

if (args.Contains("--seed-demo-data", StringComparer.OrdinalIgnoreCase))
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Le jeu de données de démonstration ne peut être créé qu'en environnement Development.");

    using var scope = app.Services.CreateScope();
    var summary = await DemoDataSeeder.SeedAsync(scope.ServiceProvider);
    Console.WriteLine(
        $"Données vérifiées dans SQL Server : {summary.Categories} catégories, {summary.Authors} auteurs, {summary.Books} livres, {summary.Students} étudiants, {summary.Employees} employés ({summary.EmployeeAccounts} comptes Identity), {summary.EmployeePermissions} attributions de permissions, {summary.Subscriptions} abonnements ({summary.ActiveSubscriptions} actifs), {summary.SubscriptionPayments} paiements, {summary.Loans} emprunts ({summary.OpenLoans} en cours, {summary.OverdueLoans} en retard, {summary.ReturnedLoans} retournés), {summary.DemoPenalties} pénalités ({summary.PaidPenalties} payées, {summary.UnpaidPenalties} impayées).");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
