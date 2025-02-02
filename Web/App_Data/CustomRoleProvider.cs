﻿using AppReadyGo.Common;
using AppReadyGo.Core.Queries.Users;
using System;
using System.Linq;
using System.Web.Security;

public class CustomRoleProvider : RoleProvider
{
    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
        throw new NotImplementedException();
    }

    public override string ApplicationName
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public override void CreateRole(string roleName)
    {
        throw new NotImplementedException();
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
        throw new NotImplementedException();
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
        throw new NotImplementedException();
    }

    public override string[] GetAllRoles()
    {
        throw new NotImplementedException();
    }

    public override string[] GetRolesForUser(string username)
    {
        int userId = 0;
        if (int.TryParse(username, out userId))
        {
            return ObjectContainer.Instance.RunQuery(new GetUserRolesQuery(userId)).Select(r => r.ToString()).ToArray();
        }
        return new string[] { };
    }

    public override string[] GetUsersInRole(string roleName)
    {
        throw new NotImplementedException();
    }

    public override bool IsUserInRole(string username, string roleName)
    {
        int userId = 0;
        if (int.TryParse(username, out userId))
        {
            return ObjectContainer.Instance.RunQuery(new GetUserRolesQuery(userId)).Any(r => r.ToString() == roleName);
        }
        return false;
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
        throw new NotImplementedException();
    }

    public override bool RoleExists(string roleName)
    {
        throw new NotImplementedException();
    }
}
