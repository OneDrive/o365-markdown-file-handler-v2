/*
 * Markdown File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace MarkdownFileHandler.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Threading.Tasks;
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