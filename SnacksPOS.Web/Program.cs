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
    o.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    o.ExpireTimeSpan = TimeSpan.FromDays(365*100);
    o.SlidingExpiration = false;
    o.LoginPath = "/SignIn";
    o.LogoutPath = "/SignOut";
});
services.AddCors(options => options.AddPolicy("cafeteria", b =>
    b.WithOrigins("https://cafeteria.internal").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
await DataSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("cafeteria");
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/api/cart/checkout", async (HttpContext http, IMediator med, [FromBody] List<CartItem> items) =>
{
    try
    {
        var userId = http.User.Identity!.Name!;
        await med.Send(new CheckoutCommand(userId, items));
        return Results.Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
}).RequireAuthorization();

app.MapGet("/api/user/balance", async (HttpContext http, IMediator med) =>
{
    try
    {
        var userId = http.User.Identity!.Name!;
        var result = await med.Send(new GetBalanceQuery(userId));
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization();

app.Run();
