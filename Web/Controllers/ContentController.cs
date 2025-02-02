﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using AppReadyGo.Model.Master;
using AppReadyGo.Model.Pages.Home;
using AppReadyGo.Core;
using System.Reflection;
using AppReadyGo.Core.Logger;
using AppReadyGo.Common.Mails;
using AppReadyGo.Common;
using AppReadyGo.Core.Queries.Content;
using AppReadyGo.Core.QueryResults.Content;
using AppReadyGo.Core.Entities;
using AppReadyGo.Model;

namespace AppReadyGo.Controllers
{
    public class ContentController : Controller
    {
        private static readonly ApplicationLogging log = new ApplicationLogging(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult MailContent(string urlPart1, string urlPart2, string urlPart3)
        {
            /*
            string path = "mails/";
            BasicMailContentModel.MailTemplate template = BasicMailContentModel.MailTemplate.Basic;
            if (!Enum.TryParse<BasicMailContentModel.MailTemplate>(urlPart1, true, out template))
            {
                template = BasicMailContentModel.MailTemplate.Basic;
                path += urlPart1;
            }

            if (!string.IsNullOrEmpty(urlPart2))
            {
                path += "/" + urlPart2;
            }
            if (!string.IsNullOrEmpty(urlPart3))
            {
                path += "/" + urlPart3;
            }

            bool isMailGenerator = isMail.HasValue && isMail.Value;

            var keys = ObjectContainer.Instance.RunQuery(new GetKeyContent(path.ToLower()));
            if (!keys.Any())
            {
                return View("404", new PricingModel { }, BeforeLoginMasterModel.MenuItem.None);
            }
            else
            {
                string rootUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                MailContentModel model = null;
                switch (template)
                {
                    case BasicMailContentModel.MailTemplate.Basic:
                        model = new BasicMailContentModel { Template = template, Email = "support@mobillify.com", ThisEmailUrl = string.Format("{0}/{1}", rootUrl, path), Content = keys["body"], SiteRootUrl = rootUrl, Subject = keys["subject"], Facebook = "", Twitter = "", IsMail = isMailGenerator };
                        break;
                }
                return View("Mails/" + template.ToString(), model);
            }
            */
            try
            {
                var email = new PromotionEmail(urlPart1, urlPart2, false);
                return View(email.EmailPagePath, email.Model);
            }
            catch
            {
                return View("404", new BeforeLoginMasterModel(BeforeLoginMasterModel.MenuItem.None));
            }
        }

        public ActionResult ErrorPage()
        {
            try
            {
                if (ObjectContainer.Instance.CurrentUserDetails != null)
                {
                    return View("AuthenticatedError", new AuthenticatedContentModel(AfterLoginMasterModel.MenuItem.None));
                }
                else
                {
                    return View("PublicError", new ContentModel(BeforeLoginMasterModel.MenuItem.None));
                }
            }
            catch (Exception exp)
            {
                log.WriteFatalError(exp, "Error to redirect to error page, show static error page.");
                return Redirect("/fatal-error.html");
            }
        }

        public ActionResult ErrorPage404()
        {
            if (ObjectContainer.Instance.CurrentUserDetails != null)
            {
                return View("Authenticated404", new AuthenticatedContentModel(AfterLoginMasterModel.MenuItem.None));
            }
            else
            {
                return View("Public404", new ContentModel(BeforeLoginMasterModel.MenuItem.None));
            }
        }

        public ActionResult PageContent(string urlPart1, string urlPart2, string urlPart3)
        {
            string path = urlPart1;
            if (!string.IsNullOrEmpty(urlPart2))
            {
                path += "/" + urlPart2;
            }
            if (!string.IsNullOrEmpty(urlPart3))
            {
                path += "/" + urlPart3;
            }

            var page = ObjectContainer.Instance.RunQuery(new GetPageQuery(path.ToLower()));
            if (page == null)
            {
                return ErrorPage404();
            }
            else
            {
                if (ObjectContainer.Instance.CurrentUserDetails != null)
                {
                    return AuthenticatedPageContent(urlPart1, page);
                }
                else
                {
                    return PublicPageContent(urlPart1, page);
                }
            }
        }

        private ActionResult AuthenticatedPageContent(string urlPart1, PageResult page)
        {
            AfterLoginMasterModel.MenuItem selectedItem = AfterLoginMasterModel.MenuItem.None;
            if (!Enum.TryParse<AfterLoginMasterModel.MenuItem>(urlPart1, true, out selectedItem))
            {
                selectedItem = AfterLoginMasterModel.MenuItem.None;
            }

            foreach (var key in Enum.GetValues(typeof(ContentPredefinedKeys)).Cast<ContentPredefinedKeys>())
            {
                page.Title = page.Title.Replace(string.Format("{{{0}}}", key), key.GetContent());
                page.Content = page.Content.Replace(string.Format("{{{0}}}", key), key.GetContent());
            }

            return View("AuthenticatedPageContent", new AuthenticatedContentModel(selectedItem) { Title = page.Title, Content = page.Content });
        }

        private ActionResult PublicPageContent(string urlPart1, PageResult page)
        {
            BeforeLoginMasterModel.MenuItem selectedItem = BeforeLoginMasterModel.MenuItem.None;
            if (!Enum.TryParse<BeforeLoginMasterModel.MenuItem>(urlPart1, true, out selectedItem))
            {
                selectedItem = BeforeLoginMasterModel.MenuItem.None;
            }
            return View("PublicPageContent", new ContentModel(selectedItem) { Title = page.Title, Content = page.Content });
        }

        //private ActionResult View<TViewModel>(string view, TViewModel viewModel, BeforeLoginMasterModel.MenuItem selectedItem)
        //{
        //    var model = new ViewModelWrapper<MainMasterModel, BeforeLoginMasterModel, TViewModel>(new MainMasterModel(), new BeforeLoginMasterModel(selectedItem), viewModel);

        //    return base.View(view, model);
        //}

        //private ActionResult View<TViewModel>(string view, TViewModel viewModel, AfterLoginMasterModel.MenuItem selectedItem)
        //{
        //    var masterModel = new AfterLoginMasterModel(this, selectedItem);
        //    masterModel.IsAdmin = User.IsInRole(StaffRole.Administrator.ToString());
        //    var model = new ViewModelWrapper<AfterLoginMasterModel, TViewModel>(masterModel, viewModel);
        //    return base.View(view, model);
        //}

        public ActionResult css(string path1, string path2, string path3, string path4, string path5, string path6, string path7)
        {
            var paths = new string[] { path1, path2, path3, path4, path5, path6, path7 }.Where(p => !string.IsNullOrEmpty(p));


            var sb = new StringBuilder();
            foreach (var p in paths)
            {
                string path = p;
                if (path.EndsWith(".css"))
                {
                    path = path.Substring(0, path.Length - 4);
                }

                var dir = Server.MapPath("~/Content/");
                path = path.Replace("_", "\\");
                sb.AppendLine("/*" + path + "*/");
                var fileContent = System.IO.File.ReadAllText(dir + path + ".css");

                sb.Append(fileContent);
            }
            byte[] byteArray = Encoding.ASCII.GetBytes(RemoveWhiteSpaceFromStylesheets(sb.ToString()));
            return base.File(byteArray, "text/css");
        }

        public ActionResult Cache(string version, string file)
        {
            var dir = Server.MapPath("~/Content/New/");
            var fileContent = System.IO.File.ReadAllText(dir + file);
            return File(fileContent, "text/css");
        }

        private string RemoveWhiteSpaceFromStylesheets(string body)
        {
            body = Regex.Replace(body, @"[a-zA-Z]+#", "#");
            body = Regex.Replace(body, @"[\n\r]+\s*", string.Empty);
            body = Regex.Replace(body, @"\s+", " ");
            body = Regex.Replace(body, @"\s?([:,;{}])\s?", "$1");
            body = body.Replace(";}", "}");
            body = Regex.Replace(body, @"([\s:]0)(px|pt|%|em)", "$1");

            // Remove comments from CSS
            body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);

            return body;
        }
    }
}
