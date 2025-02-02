﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppReadyGo.Core.Queries.Analytics;
using NHibernate.Linq;
using NHibernate;
using AppReadyGo.Domain.Model;
using AppReadyGo.Core;
using AppReadyGo.Core.QueryResults.Analytics.QueryResults;
using AppReadyGo.Core.Commands;
using AppReadyGo.Core.QueryResults.Analytics;

namespace AppReadyGo.Domain.Queries.Analytics
{
    public class ABCompareViewDataQueryHandler : FilterBaseQueryHandler, IQueryHandler<ABCompareViewDataQuery, ABCompareViewDataResult>
    {
        private IRepository repository;
        private ISecurityContext securityContext;

        public ABCompareViewDataQueryHandler(IRepository repository, ISecurityContext securityContext)
        {
            this.repository = repository;
            this.securityContext = securityContext;
        }

        public ABCompareViewDataResult Run(ISession session, ABCompareViewDataQuery query)
        {
            var data = GetResult<ABCompareViewDataResult>(session, this.securityContext.CurrentUser.Id, query);

            data.SelectedSecondPath = string.IsNullOrEmpty(query.SecondPath) ? data.SelectedPath : query.SecondPath;
            if (!string.IsNullOrWhiteSpace(data.SelectedPath) && data.SelectedScreenSize.HasValue)
            {
                data.SecondFilteredClicks = session.Query<Click>()
                                            .Where(s => s.PageView.Application.Id == data.SelectedApplicationId &&
                                                        s.PageView.Path.ToLower() == data.SelectedSecondPath.ToLower() &&
                                                        s.PageView.ScreenWidth == data.SelectedScreenSize.Value.Width &&
                                                        s.PageView.ScreenHeight == data.SelectedScreenSize.Value.Height &&
                                                        s.PageView.Date >= query.From && s.PageView.Date <= query.To)
                                                        .Select(s => s.Id)
                                                        .ToArray()
                                                        .Count();

                data.SecondHasClicks = session.Query<Click>()
                                                        .Where(s => s.PageView.Application.Id == data.SelectedApplicationId &&
                                                                    s.PageView.Path.ToLower() == data.SelectedSecondPath.ToLower() &&
                                                                    s.PageView.ScreenWidth == data.SelectedScreenSize.Value.Width &&
                                                                    s.PageView.ScreenHeight == data.SelectedScreenSize.Value.Height)
                                                        .Any();

                data.SecondFilteredVisits = session.Query<PageView>()
                                            .Where(p => p.Application.Id == data.SelectedApplicationId &&
                                                        p.Path.ToLower() == data.SelectedSecondPath.ToLower() &&
                                                        p.ScreenWidth == data.SelectedScreenSize.Value.Width &&
                                                        p.ScreenHeight == data.SelectedScreenSize.Value.Height &&
                                                        p.Date >= query.From && p.Date <= query.To)
                                                        .Select(s => s.Id)
                                                        .ToArray()
                                                        .Count();

                data.SecondFilteredScrolls = session.Query<Scroll>()
                                            .Where(s => s.PageView.Application.Id == data.SelectedApplicationId &&
                                                        s.PageView.Path.ToLower() == data.SelectedSecondPath.ToLower() &&
                                                        s.PageView.ScreenWidth == data.SelectedScreenSize.Value.Width &&
                                                        s.PageView.ScreenHeight == data.SelectedScreenSize.Value.Height &&
                                                        s.PageView.Date >= query.From && s.PageView.Date <= query.To)
                                                        .Select(s => s.Id)
                                                        .ToArray()
                                                        .Count();
            }

            return data;
        }
    }
}
