using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LatinMedia.Web.Pages.Admin.Users
{
    public class ListDeleteUsersModel : PageModel
    {
        private IUserService _userService;

        public ListDeleteUsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public UsersForAdminViewModel UsersForAdminViewModel { get; set; }
        public void OnGet(int pageId = 1, int take = 5, string filterByEmail = "", string filterByMobile = "")
        {
            if (pageId > 1)
            {
                ViewData["Take"] = (pageId - 1) * take + 1;
            }
            else
            {
                ViewData["Take"] = take;
            }

            ViewData["FilterEmail"] = filterByEmail;
            ViewData["FilterMobile"] = filterByMobile;
            ViewData["PageID"] = (pageId - 1) * take + 1;
            UsersForAdminViewModel = _userService.GetDeleteUsers(pageId, take, filterByEmail, filterByMobile);

        }
    }
}