﻿using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AppReadyGo.Common;
using AppReadyGo.Common.Mails;
using AppReadyGo.Core;
using AppReadyGo.Core.Commands.Admin;
using AppReadyGo.Core.Commands.Applications;
using AppReadyGo.Core.Commands.Content;
using AppReadyGo.Core.Commands.Users;
using AppReadyGo.Core.Logger;
using AppReadyGo.Core.Queries.Admin;
using AppReadyGo.Core.Queries.Content;
using AppReadyGo.Core.QueryResults.Admin;
using AppReadyGo.Model;
using AppReadyGo.Web.Model.Pages.Admin;

namespace AppReadyGo.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private static readonly ApplicationLogging log = new ApplicationLogging(MethodBase.GetCurrentMethod().DeclaringType);
        
        public ActionResult Index()
        {
            return RedirectToAction("Logs");
        }

        public ActionResult Membership()
        {
            //var membership = adminService.GetMembership();
            //ViewBag.Applications = membership.Applications;
            //ViewBag.Roles = membership.Roles;
            //ViewBag.Users = membership.Users;
            return View(new AdminMasterModel(AdminMasterModel.MenuItem.Members));
        }

        public ActionResult UserEdit(Guid id)
        {
            //SystemUser curUser = adminService.Get<SystemUser>(id);
            //var userModel = new UserModel()
            //{
            //    Id = curUser.Id,
            //    Name = curUser.Name,
            //    Email = curUser.Membership.Email,
            //    Roles = curUser.Roles.Select(curItem => curItem.Id).ToList()
            //};
            //IList<SystemRole> roles = adminService.GetAll<SystemRole>();
            //ViewBag.Roles = roles;
            return null;// View(userModel);
        }

        [HttpPost]
        public ActionResult UserEdit(UserModel userModel)
        {
            //IList<SystemRole> roles = adminService.GetAll<SystemRole>();
            //ViewBag.Roles = roles;
            //if (ModelState.IsValid)
            //{
            //    var user = adminService.Get<SystemUser>(userModel.Id);
            //    user.Name = userModel.Name;
            //    user.Membership.Email = userModel.Email;
            //    if (userModel.Roles != null)
            //    {
            //        user.Roles = roles.Where(curItem => userModel.Roles.Contains(curItem.Id)).ToList();
            //    }
            //    else
            //    {
            //        user.Roles = null;
            //    }
            //    adminService.Edit<SystemUser>(user);
            //    return RedirectToAction("Membership");
            //}
            return View(userModel);
        }

        public ActionResult Staff(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            bool asc = string.IsNullOrEmpty(orderby) ? true : order.Equals("asc", StringComparison.OrdinalIgnoreCase);
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllStaffQuery.OrderByColumn.Email : (GetAllStaffQuery.OrderByColumn)Enum.Parse(typeof(GetAllStaffQuery.OrderByColumn), orderby, true);
            var data = ObjectContainer.Instance.RunQuery(new GetAllStaffQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));
            var model = new StaffPagingModel() 
            {
                IsOnePage = data.TotalPages == 1,
                Count = data.Count,
                PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                TotalPages = data.TotalPages,
                CurPage = data.CurPage,
                UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                SearchStrUrlPart = searchStrUrlPart,
                SearchStr = srch,
                EmailOrder = orderBy == GetAllStaffQuery.OrderByColumn.Email && asc ? "desc" : "asc",
                NameOrder = orderBy == GetAllStaffQuery.OrderByColumn.Name && asc ? "desc" : "asc",
                Users = data.Users.Select((u, i) => new StaffDetailsModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = string.IsNullOrEmpty(u.FirstName + u.LastName) ? string.Empty : string.Concat(u.FirstName, " ", u.LastName),
                    Roles = string.Join(", ", u.Roles.Select(r => r.ToString())),
                    Activated = u.Activated,
                    Index = i,
                    IsAlternative = i % 2 != 0,
                    LastAccess = u.LastAccessDate.HasValue ? u.LastAccessDate.Value.ToString("dd MMM yyyy") : string.Empty
                }).ToArray()
            };
            return View(model);
        }

        public ActionResult Members(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllMembersQuery.OrderByColumn.CreateDate : (GetAllMembersQuery.OrderByColumn)Enum.Parse(typeof(GetAllMembersQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllMembersQuery.OrderByColumn.CreateDate) ? false : true) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);
            var data = ObjectContainer.Instance.RunQuery(new GetAllMembersQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));
            var model = new MembersPagingModel
            {
                IsOnePage = data.TotalPages == 1,
                Count = data.Count,
                PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                TotalPages = data.TotalPages,
                CurPage = data.CurPage,
                UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                SearchStrUrlPart = searchStrUrlPart,
                SearchStr = srch,
                EmailOrder = orderBy == GetAllMembersQuery.OrderByColumn.Email && asc ? "desc" : "asc",
                NameOrder = orderBy == GetAllMembersQuery.OrderByColumn.Name && asc ? "desc" : "asc",
                CreateDateOrder = orderBy == GetAllMembersQuery.OrderByColumn.CreateDate && asc ? "desc" : "asc",
                Users = data.Users.Select((u, i) => new MemberDetailsModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = string.IsNullOrEmpty(u.FirstName + u.LastName) ? string.Empty : string.Concat(u.FirstName, " ", u.LastName),
                    Activated = u.Activated,
                    SpecialAccess = u.SpecialAccess,
                    Index = i,
                    IsAlternative = i % 2 != 0,
                    LastAccess = u.LastAccessDate.HasValue ? u.LastAccessDate.Value.ToString("dd MMM yyyy") : string.Empty,
                    Registred = u.CreateDate.ToString("dd MMM yyyy"),
                    AgeDescription = u.getAgeRangeDescription(),
                    GenderDescription = u.getGenderDescription(),
                    CountryName = u.CountryName
                }).ToArray()
            };
            return View(model);
        }

        public ActionResult ApiMembers(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllApiMembersQuery.OrderByColumn.CreateDate : (GetAllApiMembersQuery.OrderByColumn)Enum.Parse(typeof(GetAllApiMembersQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllApiMembersQuery.OrderByColumn.CreateDate) ? false : true) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);
            var data = ObjectContainer.Instance.RunQuery(new GetAllApiMembersQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));
            var model = new ApiMembersPagingModel
            {
                IsOnePage = data.TotalPages == 1,
                Count = data.Count,
                PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                TotalPages = data.TotalPages,
                CurPage = data.CurPage,
                UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                SearchStrUrlPart = searchStrUrlPart,
                SearchStr = srch,
                EmailOrder = orderBy == GetAllApiMembersQuery.OrderByColumn.Email && asc ? "desc" : "asc",
                NameOrder = orderBy == GetAllApiMembersQuery.OrderByColumn.Name && asc ? "desc" : "asc",
                CreateDateOrder = orderBy == GetAllApiMembersQuery.OrderByColumn.CreateDate && asc ? "desc" : "asc",
                Users = data.Users.Select((u, i) => new MemberDetailsModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = string.IsNullOrEmpty(u.FirstName + u.LastName) ? string.Empty : string.Concat(u.FirstName, "xxxxxxx ", u.LastName),
                    Activated = u.Activated,
                    SpecialAccess = u.SpecialAccess,
                    Index = i,
                    IsAlternative = i % 2 != 0,
                    LastAccess = u.LastAccessDate.HasValue ? u.LastAccessDate.Value.ToString("dd MMM yyyy") : string.Empty,
                    Registred = u.CreateDate.ToString("dd MMM yyyy"),
                    AgeDescription = u.getAgeRangeDescription(),
                    GenderDescription = u.getGenderDescription(),
                    CountryName = u.CountryName
                }).ToArray()
            };
            return View(model);
        }

        public ActionResult Applications(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllApplicationsQuery.OrderByColumn.Name : (GetAllApplicationsQuery.OrderByColumn)Enum.Parse(typeof(GetAllApplicationsQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllApplicationsQuery.OrderByColumn.CreateDate) ? false : true) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);
            var data = ObjectContainer.Instance.RunQuery(new GetAllApplicationsQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));
            var model = new ApplicationsPagingModel
            {
                IsOnePage = data.TotalPages == 1,
                Count = data.Count,
                PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                TotalPages = data.TotalPages,
                CurPage = data.CurPage,
                UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                SearchStrUrlPart = searchStrUrlPart,
                SearchStr = srch,
                NameOrder = orderBy == GetAllApplicationsQuery.OrderByColumn.Name && asc ? "desc" : "asc",
                TypeOrder = orderBy == GetAllApplicationsQuery.OrderByColumn.Type && asc ? "desc" : "asc",
                CreateDateOrder = orderBy == GetAllApplicationsQuery.OrderByColumn.CreateDate && asc ? "desc" : "asc",
                Applications = data.Applications.Select((a, i) => new ApplicationDetailsModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    UserId = a.UserId,
                    Index = i,
                    IsAlternative = i % 2 != 0,
                    CreateDate = a.CreateDate.ToString("dd MMM yyyy"),
                    Screenshots = a.Screenshots,
                    Screens = a.Screens,
                    Visits = a.Visits,
                    Type = a.Type,
                    IsActive = a.IsActive,
                    Description = a.Description,
                    IconExt = a.IconExt,
                    Published = a.Published,
                    PackageFileName = a.PackageFileName,
                    Downloaded = a.Downloaded,
                    UserEmail = a.UserEmail
                }).ToArray()
            };
            return View(model);
        }

        public ActionResult DeleteApplication(int id)
        {
            var result = ObjectContainer.Instance.Dispatch(new RemoveApplicationCommand(id));
            return RedirectToAction("Applications");
        }

        public ActionResult DeleteMember(int id)
        {
            var result = ObjectContainer.Instance.Dispatch(new RemoveUserCommand(id));
            return RedirectToAction("Members");
        }

        public ActionResult DeleteAPIMember(int id)
        {
            var result = ObjectContainer.Instance.Dispatch(new RemoveUserCommand(id));
            return RedirectToAction("APIMembers");
        }

        public ActionResult Activate(string email)
        {
            var result = ObjectContainer.Instance.Dispatch(new ActivateUserCommand(email, UserType.Member, UserType.Staff));
            return RedirectToAction("Members");
        }

        public ActionResult Deactivate(int id)
        {
            var result = ObjectContainer.Instance.Dispatch(new DeactivateUserCommand(id));
            return RedirectToAction("Members");
        }

        public ActionResult ResendEmail(string email)
        {
            //resend welcome email
            //todo: use this:
            //var result = ObjectContainer.Instance.Dispatch(new ResendEmailCommand(id));
            try
            {
                new WebActivationEmail(email).Send();
            }
            finally
            {
                
            }
            return RedirectToAction("Members");
        }

        public ActionResult SpecialAccess(int id)
        {
            try
            {
                var result = ObjectContainer.Instance.Dispatch(new GrantSpecialAccessCommand(id, true));
            }
            finally {}
            return RedirectToAction("Members");
        }

        public ActionResult SendSpecialAccessMail(string email)
        {
            //send Special Access email
            //todo: use this:
            //var result = ObjectContainer.Instance.Dispatch(new ResendEmailCommand(id));
            try
            {
                new SpecialAccessEmail(email).Send();
            }
            finally
            {

            }
            return RedirectToAction("Members");
        }

        public ActionResult Logs()
        {
            var result = ObjectContainer.Instance.RunQuery(new LogDataQuery());

            ViewBag.Logs = string.Join("\n", result.Log.Select(a => a.ToString()));
            var categories = result.Categories.OrderBy(c => c.Value).Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
            categories.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });
            ViewBag.Categories = categories;
            var severities = result.Severities.OrderBy(s => s).Select(s => new SelectListItem { Text = s, Value = s }).ToList();
            severities.Insert(0, new SelectListItem { Text = "All", Value = string.Empty, Selected = true });
            ViewBag.Severities = severities;

            return View(new LogsModel());
        }

        public ActionResult ClearLogs()
        {
            var result = ObjectContainer.Instance.Dispatch(new ClearLogCommand());
            return RedirectToAction("Logs");
        }

        [HttpPost]
        public ActionResult Logs(LogsModel model)
        {
            int? categoryId = model.CategoryId == 0 ? null : (int?)model.CategoryId;
            model.Severity = model.Severity == "0" ? null : model.Severity;
            var result = ObjectContainer.Instance.RunQuery(new LogDataQuery(model.SearchStr, model.FromDate, model.ToDate, categoryId, model.Severity, model.ProcessId, model.ThreadId));

            ViewBag.Logs = string.Join("\n", result.Log.Select(a => a.ToString()));
            var categories = result.Categories.OrderBy(c => c.Value).Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
            categories.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });
            ViewBag.Categories = categories;
            var severities = result.Severities.OrderBy(s => s).Select(s => new SelectListItem { Text = s, Value = s }).ToList();
            severities.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });
            ViewBag.Severities = severities;

            return View(model);
        }

        public ActionResult ContentManager()
        {
            return View(new ContentManagerModel());
        }

        public ActionResult ContentItems(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllKeysQuery.OrderByColumn.Url : (GetAllKeysQuery.OrderByColumn)Enum.Parse(typeof(GetAllKeysQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllKeysQuery.OrderByColumn.Url) ? true : false) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);

            var data = ObjectContainer.Instance.RunQuery(new GetAllKeysQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));

            var model = new ContentItemsModel
            {
                Paging = new PagingModel
                {
                    IsOnePage = data.TotalPages == 1,
                    Count = data.Count,
                    PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                    NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                    TotalPages = data.TotalPages,
                    CurPage = data.CurPage,
                    UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                    SearchStrUrlPart = searchStrUrlPart,
                    SearchStr = srch
                },
                IdOrder = orderBy == GetAllKeysQuery.OrderByColumn.Id && asc ? "desc" : "asc",
                UrlOrder = orderBy == GetAllKeysQuery.OrderByColumn.Url && asc ? "desc" : "asc",
                Keys = data.Keys.Select((k, i) => new ContentItemsModel.ContentItemsKeyModel
                {
                    Id = k.Id,
                    Url = k.Url,
                    IsAlternative = i % 2 != 0,
                    ItemsCount = k.ItemsCount
                })
            };
            return View(model);
        }

        public ActionResult ContentItem(int id, int itemType, string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllKeyItemsQuery.OrderByColumn.SubKey : (GetAllKeyItemsQuery.OrderByColumn)Enum.Parse(typeof(GetAllKeyItemsQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllKeyItemsQuery.OrderByColumn.SubKey) ? true : false) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);

            GetAllKeyItemsQueryResult data = null;
            if (itemType == 1)
            {
                data = ObjectContainer.Instance.RunQuery(new GetAllKeyItemsQuery(id, srch, orderBy, asc, cp, 15));
            }
            else
            {
                data = ObjectContainer.Instance.RunQuery(new GetAllPageItemsQuery(id, srch, orderBy, asc, cp, 15));
            }

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));

            var model = new ContentItemModel
            {
                KeyId = id,
                KeyUrl = data.KeyUrl,
                ItemType = itemType,
                Paging = new PagingModel
                {
                    IsOnePage = data.TotalPages == 1,
                    Count = data.Count,
                    PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                    NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                    TotalPages = data.TotalPages,
                    CurPage = data.CurPage,
                    UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                    SearchStrUrlPart = searchStrUrlPart,
                    SearchStr = srch
                },
                IdOrder = orderBy == GetAllKeyItemsQuery.OrderByColumn.Id && asc ? "desc" : "asc",
                SubKeyOrder = orderBy == GetAllKeyItemsQuery.OrderByColumn.SubKey && asc ? "desc" : "asc",
                IsHtmlOrder = orderBy == GetAllKeyItemsQuery.OrderByColumn.IsHtml && asc ? "desc" : "asc",
                Items = data.Items.Select((item, i) => new ContentItemModel.ContentItemsItemModel
                {
                    Id = item.Id,
                    SubKey = item.SubKey,
                    IsHtml = item.IsHTML,
                    IsAlternative = i % 2 != 0
                })
            };
            return View(model);
        }

        public ActionResult EditContentItem(int id, int itemType)
        {
            var data = ObjectContainer.Instance.RunQuery(new GetItemQuery(id));

            return View(new ContentItemDetailsModel
                {
                    Id = data.Id,
                    IsHtml = data.IsHTML,
                    SubKey = data.SubKey,
                    ParentId = data.ParentId,
                    ParentUrl = data.ParentUrl,
                    Value = data.Value,
                    ItemType = itemType
                });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditContentItem(ContentItemDetailsModel model, int itemType)
        {
            var res = ObjectContainer.Instance.Dispatch(new UpdateItemCommand(model.Id, model.Value));
            var data = ObjectContainer.Instance.RunQuery(new GetItemQuery(model.Id));
            return View(new ContentItemDetailsModel
            {
                Id = data.Id,
                IsHtml = data.IsHTML,
                SubKey = data.SubKey,
                ParentId = data.ParentId,
                ParentUrl = data.ParentUrl,
                Value = data.Value,
                ItemType = itemType
            });
        }

        public ActionResult ContentPages(string srch = "", int scol = 1, int cp = 1, string orderby = "", string order = "")
        {
            var orderBy = string.IsNullOrEmpty(orderby) ? GetAllPagesQuery.OrderByColumn.Url : (GetAllPagesQuery.OrderByColumn)Enum.Parse(typeof(GetAllPagesQuery.OrderByColumn), orderby, true);
            bool asc = string.IsNullOrEmpty(orderby) ? ((orderBy == GetAllPagesQuery.OrderByColumn.Url) ? true : false) : order.Equals("asc", StringComparison.OrdinalIgnoreCase);

            var data = ObjectContainer.Instance.RunQuery(new GetAllPagesQuery(srch, orderBy, asc, cp, 15));

            var searchStrUrlPart = string.IsNullOrEmpty(srch) ? string.Empty : string.Concat("&srch=", HttpUtility.UrlEncode(srch));

            var model = new ContentPagesModel
            {
                Paging = new PagingModel
                {
                    IsOnePage = data.TotalPages == 1,
                    Count = data.Count,
                    PreviousPage = data.CurPage == 1 ? null : (int?)(data.CurPage - 1),
                    NextPage = data.CurPage == data.TotalPages ? null : (int?)(data.CurPage + 1),
                    TotalPages = data.TotalPages,
                    CurPage = data.CurPage,
                    UrlPart = string.Concat(searchStrUrlPart, string.IsNullOrEmpty(orderby) ? string.Empty : string.Concat("&orderby=", orderby), string.IsNullOrEmpty(order) ? string.Empty : string.Concat("&order=", order)),
                    SearchStrUrlPart = searchStrUrlPart,
                    SearchStr = srch
                },
                IdOrder = orderBy == GetAllPagesQuery.OrderByColumn.Id && asc ? "desc" : "asc",
                UrlOrder = orderBy == GetAllPagesQuery.OrderByColumn.Url && asc ? "desc" : "asc",
                ThemeOrder = orderBy == GetAllPagesQuery.OrderByColumn.Theme && asc ? "desc" : "asc",
                Pages = data.Pages.Select((k, i) => new ContentPagesModel.ContentPagesItemModel
                {
                    Id = k.Id,
                    Url = k.Url,
                    Theme = k.Theme,
                    IsAlternative = i % 2 != 0,
                    ItemsCount = k.ItemsCount
                })
            };
            return View(model);
        }


        public ActionResult ContentMails()
        {
            return View(new ContentMailModel());
        }



        /*
                 
        public ActionResult Elmah(string query)
        {
            return new ElmahResult(query);
        }

         */
    }


/*
    class ElmahResult : ActionResult
    {
        private string _resouceType;

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();

            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "/" + _resouceType;
                context.HttpContext.RewritePath(FilePath(context), pathInfo, context.HttpContext.Request.QueryString.ToString());
            }

            var currentApplication = (HttpApplication)context.HttpContext.GetService(typeof(HttpApplication));
            var currentContext = currentApplication.Context;

            var httpHandler = factory.GetHandler(currentContext, null, null, null);
            if (httpHandler is IHttpAsyncHandler)
            {
                var asyncHttpHandler = (IHttpAsyncHandler)httpHandler;
                asyncHttpHandler.BeginProcessRequest(currentContext, (r) => { }, null);
            }
            else
            {
                httpHandler.ProcessRequest(currentContext);
            }
        }

        private string FilePath(ControllerContext context)
        {
            return _resouceType != "stylesheet" ? context.HttpContext.Request.Path.Replace(String.Format("/{0}", _resouceType), string.Empty) : context.HttpContext.Request.Path;
        }
    }
 */
}
