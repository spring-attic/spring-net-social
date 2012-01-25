using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Spring.Social.OAuth2;
using Spring.Social.Facebook.Api;

namespace Spring.OAuth1MvcQuickStart.Controllers
{
    public class FacebookController : Controller
    {
        // Register your own Facebook app at https://developers.facebook.com/apps
        // Configure the Callback URL with 'http://localhost/Facebook/Callback'
        // Set your application id & secret here
        private const string FacebookApiId = TODO;
        private const string FacebookApiSecret = TODO;

        IOAuth2ServiceProvider<IFacebook> facebookProvider =
            new FacebookServiceProvider(FacebookApiId, FacebookApiSecret);

        // GET: /Facebook/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Facebook/SignIn
        public ActionResult SignIn()
        {
            OAuth2Parameters parameters = new OAuth2Parameters()
            {
                RedirectUrl = "http://localhost/Facebook/Callback", 
                Scope = "publish_stream"
            };
            return Redirect(facebookProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.AuthorizationCode, parameters));
        }

        // GET: /Facebook/Callback
        public ActionResult Callback(string code)
        {
            AccessGrant accessGrant = facebookProvider.OAuthOperations.ExchangeForAccessAsync(
                code, "http://localhost/Facebook/Callback", null).Result;

            Session["AccessGrant"] = accessGrant;

            return View();
        }

        // GET: /Facebook/UpdateStatus
        public ActionResult UpdateStatus()
        {
            return View();
        }

        // POST: /Facebook/UpdateStatus
        [HttpPost]
        public ActionResult UpdateStatus(string status)
        {
            AccessGrant accessGrant = Session["AccessGrant"] as AccessGrant;
            IFacebook facebookClient = facebookProvider.GetApi(accessGrant.AccessToken);

            facebookClient.UpdateStatusAsync(status).Wait();

            return View();
        }
    }
}
