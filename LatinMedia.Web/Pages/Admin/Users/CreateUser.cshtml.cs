using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Convertors;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace LatinMedia.Web.Pages.Admin.Users
{
    [PermissionChecker(3)]
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }
        public void OnGet()
        {
        
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> selectedRoles)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = _permissionService.GetRoles();
                return Page();
            }

            if (_userService.IsExsitEmail(FixedText.FixedEmail(CreateUserViewModel.Email)))
            {
                ModelState.AddModelError("CreateUserViewModel.Email","ایمیل وارد شده تکراری می باشد");
                ViewData["Roles"] = _permissionService.GetRoles();
                return Page();
            }

            if (_userService.IsExsitMobile(CreateUserViewModel.Mobile))
            {
                ModelState.AddModelError("CreateUserViewModel.Mobile", "شماره موبایل وارد شده تکراری می باشد");
                ViewData["Roles"] = _permissionService.GetRoles();
                return Page();
            }

            int userId = _userService.AddUserFromAdmin(CreateUserViewModel);
            _permissionService.AddRolesToUser(selectedRoles,userId);

            return RedirectToPage("Index");
        }
             

    
    }
}