using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

//application id - 3145f25f-fecb-4ea2-b291-7a035ce927d2
//client id - e4b72a16-7c0b-449e-b7ee-f7e510c2daab
//oauthid - https://azureeADLearning.b2clogin.com/azureeADLearning.onmicrosoft.com/<policy-name>/oauth2/v2.0/token
// Add services to the container.
builder.Services.AddControllersWithViews();
//calling the add authentication middleware 
builder.Services.AddAuthentication(options =>
{
    //changin the default to cookie auth defaults
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //seting the default challenge scheme to open id connect default
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//configuring the open id connect
//addpenidconnect - is a handlerf that will be responsoble for creating the auth request and 
//manipulating teh handler

//basically the below line will make sure when the authentication is required in app, it will use openidconnectdefault
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    //signin scheme is cookie auth 
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = "https://login.microsoftonline.com/3145f25f-fecb-4ea2-b291-7a035ce927d2/v2.0";
    options.ClientId = "e4b72a16-7c0b-449e-b7ee-f7e510c2daab";
    //options.ResponseType = "id_token";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.ClientSecret = "6h68Q~N0VVx7mUeVz-o5xYXS_wiDIhBoI5mZ0c-x";
});

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

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
