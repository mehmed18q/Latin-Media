using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LatinMedia.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IHostingEnvironment environment;
        private ICourseService _courseService;

        public HomeController(IUserService userService, IHostingEnvironment environment, ICourseService courseService)
        {
            _userService = userService;
            this.environment = environment;
            _courseService = courseService;
        }
        public IActionResult Index()
        {
            var popular = _courseService.GetPopularCourses();
            ViewBag.PopularCourses = popular;

            return View(_courseService.GetCourses().Item1);
        }


        [Route("OnlinePayment/{id}")]
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"].ToString() != ""
               && HttpContext.Request.Query["Status"].ToString().ToLower() == "ok"
               && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];
                var wallet = _userService.GetWalletByWalletId(id);
                var payment=new ZarinpalSandbox.Payment(wallet.Amount);
                var response = payment.Verification(authority).Result;
                if (response.Status == 100) // pay is success
                {
                    ViewBag.Code = response.RefId;
                    ViewBag.IsSuccess = true;
                    wallet.IsPay = true;
                    _userService.UpdateWallet(wallet);
                }
            }
            return View();
        }

        [HttpPost]
        [Route("file-upload")]
        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            var path = Path.Combine(
                environment.WebRootPath, "upload",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);

            }



            var url = $"{"/upload/"}{fileName}";


            return Json(new { uploaded = true, url });
        }

        public IActionResult GetSubGroups(int id)
        {
            List<SelectListItem> list=new List<SelectListItem>()
            {
                new SelectListItem(){Text = "انتخاب کنید" , Value = ""}
            };
            list.AddRange(_courseService.GetSubGroupsForManageCourse(id));
            return Json(new SelectList(list, "Value", "Text"));
        }

        public IActionResult GetSecondSubGroups(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "انتخاب کنید" , Value = ""}
            };
            list.AddRange(_courseService.GetSecondSubGroupsForManageCourse(id));
            return Json(new SelectList(list, "Value", "Text"));
        }

        [Route("Companies")]
        public IActionResult Companies()
        {
            return View(_courseService.ShowAllCompanies());
        }

        [Route("Teachers")]
        public IActionResult Teachers()
        {
            return View(_courseService.ShowAllTeachers());
        }


    }
}