using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lesson_33_MVC.Data;
using Lesson_33_MVC.Data.Models;
using Lesson_33_MVC.DTO;
using Lesson_33_MVC.Controllers.Api;
using Lesson_33_MVC;
using Lesson_33_MVC.Services.Interfaces;
using Lesson_33_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(string.Empty, (services, client) => {
    var contextAccessor = services.GetRequiredService<IHttpContextAccessor>();
    client.BaseAddress = new Uri($"{contextAccessor.HttpContext?.Request.Scheme}://{contextAccessor.HttpContext?.Request.Host.Value}");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddDbContext<AppDbContext>();

builder.Services
    .AddAutoMapper(config => {
        config.CreateMap<Contact, GetContactDto>();
        config.CreateMap<CreateContactDto, Contact>();
        config.CreateMap<EditContactDto, Contact>();
    });


builder.Services.AddScoped<IContactsBookService, DefaultContactsBookService>();

// one, lonely, single – 
// registers a singleton service without configuration 
builder.Services.AddSingleton<IDateTimeService, DateTimeWithLoggerService>();

// registers a singleton service directly from the object instance
// builder.Services.AddSingleton<IDateTimeService>(new DateTimeService());
// builder.Services.AddSingleton<IDateTimeService>(services => {
//     return new DateTimeService();
// });

// builder.Services.AddScoped<IDateTimeService, DateTimeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.Use(async (request, next) => {
//     // var logger = request.Request.HttpContext.RequestServices.GetRequiredService<ILogger>();
//     await next();
// });

// app.Use(async (request, next) => {
//     // var logger = request.Request.HttpContext.RequestServices.GetRequiredService<ILogger>();
//     try
//     {
//         await next();
//     }
//     catch (ContactNotFoundException ex)
//     {
//         // logger.LogError("Some very important error", ex);

//         request.Response.StatusCode =  500;
//         object? response = app.Environment.IsDevelopment()
//             ? new {
//                 error = "Super neat error",
//                 traceId = request.TraceIdentifier,
//                 ex = ex.Message
//             } : new {
//                 error = "Super neat error",
//             };
        
//         await request.Response.WriteAsJsonAsync(response);
//     }
//     catch (Exception ex)
//     {

//     }
// });

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




// http://miy_sayt.com.ua/ --> DNS --> 45.234.12.98 --> 45.234.12.98:80/index.html --> ./wwwroot/index.html --> 
// (in client's browser) http://127.0.0.1:3456/ --> [HW] http://127.0.0.1:3456/ --> [OS] -->
//   --> [asp.net] --> [UseDefaultFiles] --> http://127.0.0.1:3456/index.html --> [PhysicalFileProvider] --> ./wwwroot/index.html --> back to the client
