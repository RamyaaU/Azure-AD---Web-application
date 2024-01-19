var builder = WebApplication.CreateBuilder(args);

//application id - 3145f25f-fecb-4ea2-b291-7a035ce927d2
//oauthid - https://azureeADLearning.b2clogin.com/azureeADLearning.onmicrosoft.com/<policy-name>/oauth2/v2.0/token
// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
