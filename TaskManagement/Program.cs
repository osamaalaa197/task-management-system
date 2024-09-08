using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Reflection;
using TaskManagement.Core;
using TaskManagement.Core.Repositories;
using TaskManagement.Data;
using TaskManagement.EF;
using TaskManagement.EF.Repositories;
using TaskManagement.Hubs;
using TaskManagement.Mapping;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly(typeof(TaskManagementDbContext).Assembly.FullName)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    })
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<TaskManagementDbContext>();
builder.Services.AddControllersWithViews();


// implment automupper 
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
builder.Services.AddSignalR();

//dataprodection
builder.Services.AddDataProtection().SetApplicationName(nameof(TaskManagement));

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileService,FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages(async context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.BadRequest)
        response.Redirect("/Errors/_400");

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
        response.Redirect("/Errors/_401");

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/Errors/_403");

    if (response.StatusCode == (int)HttpStatusCode.NotFound)
        response.Redirect("/Errors/_404");

    if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
        response.Redirect("/Errors/_500");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<TaskCountHub>("/hub/TaskCountHub");
app.MapHub<SendMessageHub>("/hub/SendMessageHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
