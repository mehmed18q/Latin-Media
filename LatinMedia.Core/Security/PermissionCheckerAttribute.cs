using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;

namespace LatinMedia.Core.Security
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private int _permissionId;
        private IPermissionService _permissionService;

        public PermissionCheckerAttribute(int permissionId)
        {
            _permissionId = permissionId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _permissionService =
                (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string email = context.HttpContext.User.Identity.GetEmail();
                if (!_permissionService.CheckPermission(_permissionId, email))
                {
                    context.Result = new RedirectResult("/Login?permission=false");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Login");
            }
        }
    }
}
