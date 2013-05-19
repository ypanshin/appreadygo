﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using System.Text.RegularExpressions;
using AppReadyGo.Core;
using AppReadyGo.Domain.Model.Users;

namespace AppReadyGo.Domain.CommandHandlers
{
    public class ValidationContext : IValidationContext
    {
        private ISession session = null;
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public ValidationContext(ISession session)
        {
            this.session = session;
        }
        public bool IsEmailExists(string email, int? userId = null)
        {
            var query = this.session.Query<User>()
                            .Where(u => u.Email.ToLower() == email.ToLower());
            if (userId.HasValue)
            {
                query = query.Where(u => userId.Value != u.Id);
            }
            return query.Any();
        }

        public bool IsCorrectEmail(string email)
        {
            return Regex.IsMatch(email, MatchEmailPattern);
        }

        public bool IsCorrectPassword(string password)
        {
            return true;
        }

        public bool IsExistsTag(string tag)
        {
            return false;
        }
    }
}
