using MediatR;
using Microsoft.AspNetCore.Mvc;
using SnacksPOS.Application.Ledger;
using SnacksPOS.Application.Products;
using SnacksPOS.Application.Reports;
using SnacksPOS.Application.Users;
using SnacksPOS.Application;
using SnacksPOS.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SnacksPOS.Domain;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddRazorPages(options =>
{
    // Add global no-cache filter to all Razor Pages
    options.Conventions.ConfigureFilter(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute
    {
        Duration = 0,
        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.None,
        NoStore = true
    });
});
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

// Add API documentation
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SnacksPOS API", Version = "v1" });
});

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

// Configure Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnacksPOS API v1");
        c.RoutePrefix = "api"; // Access at /api instead of /swagger
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Add middleware to set no-cache headers for all responses
app.Use(async (context, next) =>
{
    // Set no-cache headers for all responses
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    
    await next();
});

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        // For development, disable caching on static files too
        if (app.Environment.IsDevelopment())
        {
            context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Context.Response.Headers["Pragma"] = "no-cache";
            context.Context.Response.Headers["Expires"] = "0";
        }
    }
});
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

// Product Management API Endpoints
app.MapGet("/api/products", async (IMediator med) =>
{
    try
    {
        var products = await med.Send(new GetProductsQuery());
        return Results.Ok(products);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).WithName("GetProducts")
  .WithSummary("Get all active products")
  .WithDescription("Retrieves a list of all active products in the system");

app.MapGet("/api/products/{id:int}", async (int id, IMediator med) =>
{
    try
    {
        var product = await med.Send(new GetProductByIdQuery(id));
        return product != null ? Results.Ok(product) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).WithName("GetProductById")
  .WithSummary("Get product by ID")
  .WithDescription("Retrieves a specific product by its ID");

app.MapPost("/api/products", async ([FromBody] CreateProductRequest request, IMediator med) =>
{
    try
    {
        var command = new CreateProductCommand(request.Name, request.Description, request.Price, request.ImageUrl);
        var product = await med.Send(command);
        return Results.Created($"/api/products/{product.Id}", product);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("CreateProduct")
  .WithSummary("Create a new product")
  .WithDescription("Creates a new product in the system");

app.MapPut("/api/products/{id:int}", async (int id, [FromBody] UpdateProductRequest request, IMediator med) =>
{
    try
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.ImageUrl, request.IsActive);
        var product = await med.Send(command);
        return product != null ? Results.Ok(product) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("UpdateProduct")
  .WithSummary("Update an existing product")
  .WithDescription("Updates an existing product with new information");

app.MapDelete("/api/products/{id:int}", async (int id, IMediator med) =>
{
    try
    {
        var success = await med.Send(new DeleteProductCommand(id));
        return success ? Results.Ok(new { message = "Product deleted successfully" }) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("DeleteProduct")
  .WithSummary("Delete a product")
  .WithDescription("Soft deletes a product by marking it as inactive");

// Sales Reporting API Endpoints
app.MapGet("/api/reports/sales", async (
    [FromQuery] DateTime? startDate, 
    [FromQuery] DateTime? endDate, 
    [FromQuery] string? userId, 
    IMediator med) =>
{
    try
    {
        var report = await med.Send(new GetSalesReportQuery(startDate, endDate, userId));
        return Results.Ok(report);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("GetSalesReport")
  .WithSummary("Get sales report")
  .WithDescription("Generate sales report with optional date range and user filtering");

// User Management API Endpoints
app.MapGet("/api/users", async (IMediator med, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20) =>
{
    try
    {
        var users = await med.Send(new GetUsersQuery(pageNumber, pageSize));
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("GetUsers")
  .WithSummary("Get users with pagination")
  .WithDescription("Retrieve a paginated list of users");

app.MapPost("/api/users", async ([FromBody] CreateUserRequest request, IMediator med) =>
{
    try
    {
        var command = new CreateUserCommand(request.Email, request.Password, request.Roles);
        var result = await med.Send(command);
        
        if (result.Success)
        {
            return Results.Created($"/api/users/{result.UserId}", new { userId = result.UserId });
        }
        else
        {
            return Results.BadRequest(new { errors = result.Errors });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("CreateUser")
  .WithSummary("Create a new user")
  .WithDescription("Create a new user account with specified roles");

app.MapPut("/api/users/{userId}", async (string userId, [FromBody] UpdateUserRequest request, IMediator med) =>
{
    try
    {
        var command = new UpdateUserCommand(userId, request.Email, request.Roles, request.IsActive);
        var result = await med.Send(command);
        
        if (result.Success)
        {
            return Results.Ok(new { message = "User updated successfully" });
        }
        else
        {
            return Results.BadRequest(new { errors = result.Errors });
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization()
  .WithName("UpdateUser")
  .WithSummary("Update an existing user")
  .WithDescription("Update user information and roles");

app.Run();

// Request DTOs for API endpoints
public record CreateProductRequest(string Name, string? Description, decimal Price, string? ImageUrl);
public record UpdateProductRequest(string Name, string? Description, decimal Price, string? ImageUrl, bool IsActive);
public record CreateUserRequest(string Email, string Password, List<string> Roles);
public record UpdateUserRequest(string Email, List<string> Roles, bool IsActive);
