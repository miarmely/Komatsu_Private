using System.Reflection;
using Temsa_Web.Extensions;

#region add extensions
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAddControllersWithView();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.ConfigureServices();
builder.Services.ConfigureManagers();
builder.Services.ConfigureCookie();
builder.Services.ConfigureConfigModels(builder.Configuration);

var app = builder.Build();
#endregion

#region set production or staging mode 
if (app.Environment.IsProduction())
    app.UseHsts();
#endregion

#region add pipelines
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=authentication}/{action=login}/{id?}");
#endregion

app.Run();