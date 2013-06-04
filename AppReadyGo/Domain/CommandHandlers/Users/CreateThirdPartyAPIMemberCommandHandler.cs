﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using AppReadyGo.Core.Commands.Users;
using AppReadyGo.Domain.Model.Users;
using AppReadyGo.Domain.Model;

namespace AppReadyGo.Domain.CommandHandlers.Users
{
    public class CreateThirdPartyAPIMemberCommandHandler : ICommandHandler<CreateThirdPartyAPIMemberCommand, int>
    {
        public int Execute(ISession session, CreateThirdPartyAPIMemberCommand cmd)
        {
            var country = session.Get<Country>(cmd.CountryId);
            var appTypes = cmd.ApplicationTypes != null ? cmd.ApplicationTypes.Select(x => session.Get<ApplicationType>(x)).ToArray() : null;
            var user = new ApiMember(cmd.Email, cmd.FirstName, cmd.LastName, cmd.Gender, cmd.AgeRange, country, cmd.Zip, appTypes);
            session.Save(user);
            return user.Id;
        }
    }
}
