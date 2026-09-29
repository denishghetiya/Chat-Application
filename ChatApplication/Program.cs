using Chat.Helper;
using Chat.Middleware;
using Chat.Services;
using ChatApplication.DBContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ChatContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<EncryptionHelper>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        //policy.SetIsOriginAllowed(origin => true)
//        policy.WithOrigins("http://localhost:5047")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials(); 
//    });
//});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "AuthCookie";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//app.UseCors();
app.UseNoCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

