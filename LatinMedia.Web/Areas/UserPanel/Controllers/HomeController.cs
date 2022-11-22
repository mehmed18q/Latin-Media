using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Convertors;
using LatinMedia.Core.Security;
using LatinMedia.Core.Senders;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LatinMedia.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IViewRenderService _viewRender;

        public HomeController(IUserService userService, IViewRenderService viewRender)
        {
            _userService = userService;
            _viewRender = viewRender;
        }

        public IActionResult Index()
        {
            return View(_userService.GetUserInformation(User.Identity.GetEmail()));
        }

        #region Edit Profile
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userService.GetDataForEditProfileUser(User.Identity.GetEmail()));
        }

        [HttpPost]
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            _userService.EditProfile(User.Identity.GetEmail(), profile);

            //--------LogOut User-------------//

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login?EditProfile=true");
        }

        #endregion

        #region Change Password

        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel change)
        {
            string currentUser = User.Identity.GetEmail();
            if (!ModelState.IsValid)
                return View(change);

            if (!_userService.CompareOldPassword(currentUser, change.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی صحیح نمی باشد");
                return View(change);
            }

            _userService.ChangeUserPassword(currentUser, change.Password);
            ViewBag.IsSuccess = true;
            return View();
        }

        #endregion

        #region Change Mail

        [Route("UserPanel/ChangeMail")]
        public IActionResult ChangeMail()
        {
            return View();
        }

        [HttpPost]
        [Route("UserPanel/ChangeMail")]
        public IActionResult ChangeMail(ChangeMailViewModel mail)
        {
            if (!ModelState.IsValid)
            {
                return View(mail);
            }

            if (_userService.IsExsitEmail(mail.Email))
            {
                ModelState.AddModelError("Email", "ایمیل وارد شده تکراری است");
                return View(mail);
            }

            var user = _userService.GetUserByEmail(User.Identity.GetEmail());
            ChangeMailViewModel viewModel = new ChangeMailViewModel();
            string email = mail.Email;
            viewModel.Email = EncryptData.Encrypt(mail.Email);
            viewModel.Token = user.ActiveCode;
            viewModel.UserId = user.UserId;
           
            #region Send Email For Change Mail

            string bodyEmail = _viewRender.RenderToStringAsync("_ChangeMail", viewModel);
            SendEmail.Send(email,"تغییر ایمیل کاربری",bodyEmail);
            #endregion


            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        #endregion



    }
}