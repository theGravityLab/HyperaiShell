using Hyperai.Relations;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class AuthorizationExtensions
    {
        private static readonly IAuthorizationService service;

        static AuthorizationExtensions()
        {
            service = Shared.Application.Provider.GetRequiredService<IAuthorizationService>();
        }

        public static void GrantLimited(this RelationModel model, string name, int count)
        {
            service.PutLimited(model, name, count);
        }

        public static void GrantExpiry(this RelationModel model, string name, DateTime expiration)
        {
            service.PutExpiry(model, name, expiration);
        }

        public static void Grant(this RelationModel model, string name)
        {
            service.PutNormal(model, name);
        }

        public static bool CheckPermission(this RelationModel model, string name)
        {
            return service.CheckTicket(model, name);
        }

        public static void RevokePermission(this RelationModel model, string name)
        {
            service.RemoveTicket(model, name);
        }

        public static IEnumerable<string> GetPermissions(this RelationModel model)
        {
            return service.GetTickets(model).Select(x => x.Name);
        }
    }
}