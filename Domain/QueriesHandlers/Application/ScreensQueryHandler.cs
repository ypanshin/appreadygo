﻿using System.Linq;
using AppReadyGo.Core;
using AppReadyGo.Core.Queries.Admin;
using AppReadyGo.Core.Queries.Analytics;
using AppReadyGo.Core.QueryResults.Applications;
using AppReadyGo.Core.QueryResults.Users;
using AppReadyGo.Domain.Model.Users;
using NHibernate;
using NHibernate.Linq;
using AppReadyGo.Domain.Model;

namespace AppReadyGo.Domain.Queries.Application
{
    public class ScreensQueryHandler : IQueryHandler<ScreensQuery, ScreensDataResult>
    {
        public ScreensDataResult Run(ISession session, ScreensQuery query)
        {
            var res = session.Query<Model.Application>()
                       .Where(a => a.Id == query.ApplicationId)
                        .Select(a => new ScreensDataResult
                        {
                            ApplicationId = a.Id,
                            ApplicationName = a.Name
                        })
                        .Single();

            var screensQuery = session.Query<Screen>()
                        .Where(s => s.Application.Id == query.ApplicationId);

            if (!string.IsNullOrEmpty(query.SearchStr))
            {
                screensQuery = screensQuery.Where(s => s.Path.ToLower().Contains(query.SearchStr.ToLower()));
            }

            res.Count = screensQuery.Count();
            res.TotalPages = (res.Count + query.PageSize - 1) / query.PageSize;
            res.CurPage = query.CurPage > res.TotalPages ? res.TotalPages : query.CurPage;
            res.PageSize = query.PageSize;


            var screens = screensQuery.Select(s => new ScreenDataItemResult
            {
                Id = s.Id,
                Width = s.Width,
                Height = s.Height,
                Path = s.Path,
                FileExtension = s.FileExtension
            });

            if (query.OrderBy == ScreensQuery.OrderByColumn.Width)
            {
                screens = query.ASC ? screens.OrderBy(s => s.Width) : screens.OrderByDescending(s => s.Width);
            }
            else if (query.OrderBy == ScreensQuery.OrderByColumn.Height)
            {
                screens = query.ASC ? screens.OrderBy(s => s.Height) : screens.OrderByDescending(s => s.Height);
            }
            else if (query.OrderBy == ScreensQuery.OrderByColumn.Path)
            {
                screens = query.ASC ? screens.OrderBy(s => s.Path) : screens.OrderByDescending(s => s.Path);
            }

            res.Screens = screens.Skip(res.PageSize * (res.CurPage - 1))
                            .Take(res.PageSize)
                            .ToArray();

            return res;
        }
    }
}
