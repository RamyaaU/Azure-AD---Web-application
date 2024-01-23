using AzureADWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace AzureADWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            var scheme = OpenIdConnectDefaults.AuthenticationScheme;

            //to specify where the user should be redirected after a successful authentication or authorization process.
            var redirectURL = Url.ActionContext.HttpContext.Request.Scheme + "://" + Url.ActionContext.HttpContext.Request.Host;

            //like teh response status, challenge too returns the value
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectURL
            }, scheme);
        }

        public IActionResult SignOut()
        {
            var scheme = OpenIdConnectDefaults.AuthenticationScheme;
            //cookies- because it needs to be cleared after every session
            //basically this trggers the sign out functionality 
            return SignOut (new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme, scheme);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public async Task<IActionResult> CallAPI()
        {
            //retrieves teh access token from current http context process
            var accessToken = await HttpContext.GetTokenAsync("access_token");
        
            //create a client
            var client =_httpClientFactory.CreateClient();
        
            //send the request with Get method 
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7159/WeatherForecast");
            //var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44301/WeatherForecast");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        
            var response = await client.SendAsync(request);
        
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //some issue
            }
        
            //returns the content of http response 
            return Content(response.ToString());
        }
    }
}
