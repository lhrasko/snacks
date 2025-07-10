using MediatR;
using Microsoft.AspNetCore.Mvc;
using SnacksPOS.Application.Ledger;
using SnacksPOS.Application;
using SnacksPOS.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SnacksPOS.Domain;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddRazorPages();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);
services.ConfigureApplicationCookie(o =>
{
    o.Cookie.IsEssential = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.ExpireTimeSpan = TimeSpan.FromDays(365*100);
    o.SlidingExpiration = false;
});
services.AddCors(options => options.AddPolicy("cafeteria", b =>
    b.WithOrigins("https://cafeteria.internal").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
await DataSeeder.SeedAsync(app.Services);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("cafeteria");
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/api/cart/checkout", async (HttpContext http, IMediator med, [FromBody] List<CartItem> items) =>
{
    var userId = http.User.Identity!.Name!;
    await med.Send(new CheckoutCommand(userId, items));
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/user/balance", async (HttpContext http, IMediator med) =>
{
    var userId = http.User.Identity!.Name!;
    var result = await med.Send(new GetBalanceQuery(userId));
    return Results.Ok(result);
}).RequireAuthorization();

app.Run();
