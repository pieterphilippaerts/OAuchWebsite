using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
//using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OAuch.Database;
using OAuch.Database.Entities;
using OAuch.Helpers;
using OAuch.Protocols.Http;
using OAuch.Shared;
using OAuch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OAuch.Controllers {
    // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/social-without-identity?view=aspnetcore-5.0
    public class LoginController : Controller {
        public IActionResult WithLink() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateLink([Bind(Prefix = "g-recaptcha-response")] string? recaptchaResponse) {
            var secrets = ServiceLocator.Resolve<Secrets>()!;
            bool captchaSucceeded = false;
            if (recaptchaResponse != null) {
                try {
                    var http = HttpHelper.CreateTransient();
                    var captchaRequest = HttpRequest.CreatePost("https://www.google.com/recaptcha/api/siteverify");
                    captchaRequest.AllowAutoRedirect = true;
                    captchaRequest.Content = Encoding.UTF8.GetBytes($"secret={HttpUtility.UrlEncode(secrets.CaptchaSiteSecret)}&response={HttpUtility.UrlEncode(recaptchaResponse)}");
                    captchaRequest.Headers[HttpRequestHeaders.ContentType] = "application/x-www-form-urlencoded";
                    var response = await http.SendRequest(captchaRequest);
                    if (response.StatusCode == HttpStatusCode.OK) {
                        var responseJson = Encoding.UTF8.GetString(response.Content);
                        var captchaResponse = JsonConvert.DeserializeObject<CaptchaVerificationResult>(responseJson);
                        captchaSucceeded = captchaResponse.Success && captchaResponse.Hostname?.ToLower() == "oauch.io";
                    }
                } catch { /* captcha verification failed */ }
            }
            if (!captchaSucceeded) {
                return RedirectToAction("WithLink");
            }

            // captcha succeeded
            UserSession? session = null;
            Guid? internalUserId = null;
            using (var db = new OAuchDbContext()) {
                var oauchClaim = this.User?.FindFirst(OAuchInternalIdClaimType);
                if (oauchClaim != null && Guid.TryParseExact(oauchClaim.Value, "N", out var internalId)) { // user is already logged in; reuse the previously generated login link (if it exists)
                    internalUserId = internalId;
                    session = db.UserSessions.Where(s => s.InternalId == internalId && s.Scheme == OAuchLoginLinkScheme).FirstOrDefault();
                }
                if (internalUserId == null) {
                    internalUserId = Guid.NewGuid();
                }
                if (session == null) {
                    session = new UserSession()
                    {
                        Scheme = OAuchLoginLinkScheme,
                        LoginId = Guid.NewGuid().ToString("N"),
                        InternalId = internalUserId.Value,
                        RemoteIp = this.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        LoggedInAt = DateTime.Now
                    };
                    db.UserSessions.Add(session);
                    await db.SaveChangesAsync();

                    SignIn(session.InternalId);
                }
            }
            // show the link to the user
            var model = new LoginLinkViewModel();
            model.LinkId = session.LoginId;
            return View(model);
        }
        public IActionResult Link(string id) {
            if (!Guid.TryParseExact(id, "N", out var guid)) {
                return NotFound();
            }
            using (var db = new OAuchDbContext()) {
                var session = db.UserSessions.Where(s => s.Scheme == OAuchLoginLinkScheme && s.LoginId == id).FirstOrDefault();
                if (session == null)
                    return NotFound();
                SignIn(session.InternalId);
                session.LoggedInAt = DateTime.Now;
                session.RemoteIp = this.HttpContext.Connection.RemoteIpAddress?.ToString();
                db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public async Task WithGoogle() {
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow,
                RedirectUri = "https://oauch.io/Login/ProcessGoogle"
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, authProperties);
        }
        public Task<IActionResult> ProcessGoogle() => Process(GoogleDefaults.AuthenticationScheme);

        //public async Task WithFacebook() {
        //    var authProperties = new AuthenticationProperties
        //    {
        //        AllowRefresh = true,
        //        IsPersistent = true,
        //        IssuedUtc = DateTimeOffset.UtcNow,
        //        RedirectUri = "https://oauch.io/Login/ProcessFacebook"
        //    };
        //    await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme, authProperties);
        //}
        //public Task<IActionResult> ProcessFacebook() => Process(FacebookDefaults.AuthenticationScheme);

        public async Task WithMicrosoft() {
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow,
                RedirectUri = "https://oauch.io/Login/ProcessMicrosoft"
            };
            await HttpContext.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme, authProperties);
        }
        public Task<IActionResult> ProcessMicrosoft() => Process(MicrosoftAccountDefaults.AuthenticationScheme);

        //public async Task WithTwitter() {
        //    var authProperties = new AuthenticationProperties
        //    {
        //        AllowRefresh = true,
        //        IsPersistent = true,
        //        IssuedUtc = DateTimeOffset.UtcNow,
        //        RedirectUri = "https://oauch.io/Login/ProcessTwitter"
        //    };
        //    await HttpContext.ChallengeAsync(TwitterDefaults.AuthenticationScheme, authProperties);
        //}
        //public Task<IActionResult> ProcessTwitter() => Process(TwitterDefaults.AuthenticationScheme);


        [NonAction]
        private async Task<IActionResult> Process(string scheme) {
            var result = await HttpContext.AuthenticateAsync(scheme);
            var internalId = result.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (!result.Succeeded || internalId == null) {
                // authentication failed (or user canceled)
                return RedirectToAction("Index", "Dashboard");
            }
            UserSession? userSession;
            using (var db = new OAuchDbContext()) {
                userSession = db.UserSessions.Where(c => c.Scheme == scheme && c.LoginId == internalId.Value).FirstOrDefault();
                if (userSession == null) {
                    userSession = new UserSession()
                    {
                        Scheme = scheme,
                        LoginId = internalId.Value,
                        InternalId = Guid.NewGuid(), // OAuch internal ID
                        RemoteIp = this.HttpContext.Connection.RemoteIpAddress?.ToString()
                    };
                    db.UserSessions.Add(userSession);
                }
                userSession.LoggedInAt = DateTime.Now;
                userSession.RemoteIp = this.HttpContext.Connection.RemoteIpAddress?.ToString();
                await db.SaveChangesAsync();
            }
            // create authentication cookie
            SignIn(userSession.InternalId);
            return RedirectToAction("Index", "Dashboard");
        }

        [NonAction]
        private async void SignIn(Guid internalId) {
            var oauchIdentity = new ClaimsIdentity(new Claim[] {
                        new Claim(OAuchInternalIdClaimType, internalId.ToString("N"))
                    }, "OAuchAuthentication");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(oauchIdentity), new AuthenticationProperties
            {
                IsPersistent = true
            });
        }

        public async Task<IActionResult> Invalidate() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        public const string OAuchInternalIdClaimType = "https://oauch.io/internalid";
        public const string OAuchLoginLinkScheme = "OAuchLoginLink";

        private class CaptchaVerificationResult {
            [JsonProperty("success")]
            public bool Success { get; set; }
            [JsonProperty("challenge_ts")]
            public DateTime ChallengeTs { get; set; }
            [JsonProperty("hostname")]
            public string? Hostname { get; set; }
        }
    }
}