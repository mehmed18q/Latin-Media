using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LatinMedia.Core.Convertors;
using LatinMedia.Core.Genertors;
using LatinMedia.Core.Security;
using LatinMedia.Core.Senders;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LatinMedia.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IViewRenderService _viewRender;

        public AccountController(IUserService userService, IViewRenderService viewRender)
        {
            _userService = userService;
            _viewRender = viewRender;
        }


        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            if (_userService.IsExsitMobile(register.Mobile))
            {
                ModelState.AddModelError("Mobile", "شماره موبایل وارد شده تکراری است");
                return View(register);
            }
            if (_userService.IsExsitEmail(FixedText.FixedEmail(register.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل وارد شده تکراری است");
                return View(register);
            }

            DataLayer.Entities.User.User user = new User()
            {
                Mobile = register.Mobile,
                Email = FixedText.FixedEmail(register.Email),
                LastName = register.LastName,
                Password = PasswordHelper.EncodePasswordMd5(register.Password),
                FirstName = register.FirstName,
                ActiveCode = GeneratorName.GenrateUniqeCode(),
                CreateDate = DateTime.Now,
                IsActive = false,
                UserAvatar = "default.png",
            };

            _userService.AddUser(user);

            #region Send Activation Email

            string body = _viewRender.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "ایمیل فعال سازی", body);

            #endregion

            return View("SuccessRegister", model: user);
        }

        #endregion

        #region Login

        [Route("Login")]
        public IActionResult Login(bool EditProfile=false,bool permission = true)
        {
            ViewBag.EditProfile = EditProfile;
            ViewBag.Permission = permission;
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = _userService.LoginUser(login);
            if (user != null)
            {
                if (user.IsActive)
                {
                    var claim = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                        new Claim(ClaimTypes.Name,user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.Email,user.Email),
                     
                    };
                    var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    };

                    HttpContext.SignInAsync(principal, properties);
                    ViewBag.IsSuccess = true;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Email", "حساب کاربری شام غیر فعال می باشد");
                }
            }
            ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
            return View();
        }

        #region LogOut
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }


        #endregion
        #endregion

        #region Active Account

        public IActionResult ActivateAccount(string id)
        {
            ViewBag.IsActive = _userService.ActiveAccount(id);
            return View();
        }


        #endregion

        #region ForgotPassword

        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!ModelState.IsValid)
                return View(forgot);
            string fixedEmail = FixedText.FixedEmail(forgot.Email);
            DataLayer.Entities.User.User user = _userService.GetUserByEmail(fixedEmail);
            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری یافت نشد");
                return View(forgot);
            }

            string bodyEmail = _viewRender.RenderToStringAsync("_ForgotPassword", user);
            SendEmail.Send(user.Email, "بازیابی کلمه عبور", bodyEmail);
            ViewBag.IsSuccess = true;
            return View();
        }
        #endregion

        #region Reset Password

       
        public IActionResult ResetPassword(string id)
        {
            return View(new ResetPasswordViewModel()
            {
                ActiveCode = id,
            });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel reset)
        {
            if (!ModelState.IsValid)
                return View(reset);

            DataLayer.Entities.User.User user = _userService.GetUserByActiveCode(reset.ActiveCode);
            if (user == null)
                return NotFound();

            string hashNewPassword = PasswordHelper.EncodePasswordMd5(reset.Password);
            user.Password = hashNewPassword;
            _userService.UpdateUser(user);
            return Redirect("/Login");


        }

        #endregion

        #region Change Mail Account

        public IActionResult ChangeMailAccount(string userId, string token, string mail)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (mail == null) throw new ArgumentNullException(nameof(mail));

            if (HttpContext.Request.Query["userId"] != ""
                && HttpContext.Request.Query["token"] != ""
                && HttpContext.Request.Query["mail"] != "")
            {
                int UserId = Convert.ToInt32(userId);
                token = HttpContext.Request.Query["token"];
                mail = HttpContext.Request.Query["mail"];

                if (_userService.ChangeUserEmail(UserId, token, mail))
                {
                    ViewBag.IsSuccess = true;
                }
                else
                {
                    ViewBag.IsSuccess = false;
                }
            }

            return View();
        }

        #endregion

    }
}