using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

//configuration for microsoft graph api
builder.Services.AddSingleton(_=>
{
    var cred = new ClientSecretCredential(builder.Configuration["AzureAD:TenantId"], builder.Configuration["AzureAD:ClientId"], builder.Configuration["AzureAD:SecretId"]);
    return new GraphServiceClient(cred);
});

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
    options.Authority = "https://azureadb2clearnning.b2clogin.com/AzureADB2CLearnning.onmicrosoft.com/B2C_1_SignIn_Up/v2.0";
    options.ClientId = "fe61dbca-1d21-4dd0-9e42-d8a711947ab9";
    //options.ResponseType = "id_token";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.ClientSecret = "Xy88Q~3d_5rWReMFBBjBhzS0f3UF9W8PdhVwzcLc";
    options.Scope.Add(options.ClientId);
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
