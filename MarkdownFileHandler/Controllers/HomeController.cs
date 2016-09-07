using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace MarkdownFileHandler.Controllers
{
    
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            Models.HomeModel model = new Models.HomeModel();

            model.SignInName = AuthHelper.GetUserId();
            if (!string.IsNullOrEmpty(model.SignInName))
            {
                try
                {
                    var accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(SettingsHelper.AADGraphResourceId);
                    // Make an API request to get display name
                    var response = await FileHandlerActions.Directory.UserInfo.GetUserInfoAsync(SettingsHelper.AADGraphResourceId, model.SignInName, accessToken);
                    if (null != response)
                    {
                        model.DisplayName = response.DisplayName;
                    }
                    else
                    {
                        model.DisplayName = "Error getting data from Microsoft Graph.";
                    }
                } catch (Exception ex)
                {
                    model.DisplayName = ex.ToString();
                }
            }


            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error(string msg)
        {
            ViewBag.Message = msg;

            return View();
        }
    }
}