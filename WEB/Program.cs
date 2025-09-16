using ApplicationCore.UserEntites.Concrete;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Business.AutoMapper;
using Business.Manager.Concrete;
using Business.Manager.Interface;
using Business.Validation;
using DataAccess.DbContext;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WEB.Autofac;
using WEB.AutoMapper;   
using WEB.Infrastructure;     
using WEB.ServiceExtensions;
using static WEB.Infrastructure.NullOrEmailSender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(opt =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();
    opt.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});


//Validation için
builder.Services.AddFluentValidationAutoValidation();     
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDTOValidation>();

//Autofac için
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacModule());
            });

builder.Services.AddAutoMapper(typeof(AccountBusinessMapping).Assembly, typeof(AccountMapping).Assembly);


//SQL-Database için
builder.Services.AddDbContext<BlogDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//Þifre için 
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<BlogDbContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Account/Login";
    opt.AccessDeniedPath = "/Account/AccessDenied";
    opt.LogoutPath = "/Account/Logout";
    opt.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});


builder.Services.AddScoped<IUserManager, UserManagerFacade>();


builder.Services.AddSingleton<IEmailSender, NullEmailSender>();

//Bunu silebilirim belki
builder.Services.AddCors(o => o.AddPolicy("spa", p =>
    p.WithOrigins("http://localhost:7218")  
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));


var app = builder.Build();

await app.SeedIdentityAsync();

//Bunu eðer map'leme sýrasýnda problem yaþanýrsa onu anlamak için
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    try
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
    catch (AutoMapperConfigurationException ex)
    {
       
        System.Diagnostics.Debug.WriteLine(ex.ToString());

        foreach (var err in ex.Errors)
        {
            var src = err.TypeMap?.SourceType?.FullName ?? "?";
            var dest = err.TypeMap?.DestinationType?.FullName ?? "?";
            System.Diagnostics.Debug.WriteLine($"MAP ERROR: {src} -> {dest}");

            if (err.UnmappedPropertyNames != null && err.UnmappedPropertyNames.Any())
            {
                System.Diagnostics.Debug.WriteLine("  Unmapped: " + string.Join(", ", err.UnmappedPropertyNames));
            }
        }

        throw; 
    }

}




app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { error = "Server error", detail = ex.Message });
    }
});



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Silme ihtimalim olan þey için
app.UseCors("spa");



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Program içi seedleme 
await app.SeedIdentityAsync();
await app.SeedDemoDataAsync(); 




app.Run();



