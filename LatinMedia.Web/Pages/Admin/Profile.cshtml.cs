using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LatinMedia.Web.Pages.Admin
{
    [UserRoleChecker]
    public class ProfileModel : PageModel
    {
        private IUserService _userService;

        public ProfileModel(IUserService userService)
        {
            _userService = userService;
        }

        public InformationUserViewModel InformationUserViewModel { get; set; }

        public void OnGet()
        {
            InformationUserViewModel = _userService.GetUserInformation(User.Identity.GetEmail());
        }
    }
}