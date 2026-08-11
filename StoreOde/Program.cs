using StoreOde.Extensions;
using StoreOde.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStoreOdeServices(
    builder.Configuration,
    builder.Environment);

builder.Services.AddStoreOdeRateLimiting();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

await AdminRoleInitializer.InitializeAsync(
    app.Services,
    app.Configuration);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();