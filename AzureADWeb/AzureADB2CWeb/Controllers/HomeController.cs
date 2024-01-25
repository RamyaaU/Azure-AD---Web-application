using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using AzureADB2CWeb.Models;
using System.Text;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.Graph;

namespace AzureADWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GraphServiceClient _graphServiceClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory,
            GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _graphServiceClient = graphServiceClient;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Index()
        {
            try
            {
                // fetch the groups of the user
                var user = await _graphServiceClient.Users["6ce40d8f-0427-4932-bebe-6d0673ee1d93"].MemberOf.Request().GetAsync();
                var groupInfoStringBuilder = new StringBuilder();

                foreach (var directoryObject in user.CurrentPage)
                {
                    if (directoryObject is Microsoft.Graph.Group group)
                    {
                        groupInfoStringBuilder.AppendLine();

                        groupInfoStringBuilder.AppendLine($"Group Name: {group.DisplayName}")
                                           .AppendLine($"Group ID: {group.Id}");

                        var groupMembers = await _graphServiceClient.Groups[group.Id].Members.Request().GetAsync();

                        if (groupMembers.CurrentPage.Count > 0)
                        {
                            groupInfoStringBuilder.AppendLine("Members:");

                            foreach (var member in groupMembers.CurrentPage)
                            {
                                if (member is Microsoft.Graph.User groupMember)
                                {
                                    string memberDisplayName = groupMember.DisplayName;

                                    groupInfoStringBuilder.AppendLine($" - {memberDisplayName}");
                                }
                            }
                        }
                        else
                        {
                            groupInfoStringBuilder.AppendLine("Members: None");
                        }

                        groupInfoStringBuilder.AppendLine("-----------");
                    }
                }

                ViewBag.GroupInfo = groupInfoStringBuilder.ToString().Replace(Environment.NewLine, "<br />");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error fetching group information: " + ex.Message;
            }

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
            return SignOut(new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme, scheme);
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
            var client = _httpClientFactory.CreateClient();

            //send the request with Get method 
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7159/WeatherForecast");
            //var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44301/WeatherForecast");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

            var response = await client.SendAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //some issue
            }

            //returns the content of http response 
            return Content(response.ToString());
        }
    }
}
