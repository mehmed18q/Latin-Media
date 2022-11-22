using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LatinMedia.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class MyOrdersController : Controller
    {
        private IOrderService _orderService;
        private ICourseService _courseService;
        private IHostingEnvironment _environment;

        public MyOrdersController(IOrderService orderService, ICourseService courseService, IHostingEnvironment environment)
        {
            _orderService = orderService;
            _courseService = courseService;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View(_orderService.GetUserOrders(User.Identity.GetEmail()));
        }

        public IActionResult ShowOrder(int id,bool finaly=false,string type="")
        {
            var order = _orderService.GetOrdersForUserPanel(User.Identity.GetEmail(), id);
            if(order==null)
            {
                return NotFound();
            }

            ViewBag.Finaly = finaly;
            ViewBag.DiscountType = type;
            return View(order);
        }


        public IActionResult FinalyOrder(int id)
        {
            if (_orderService.FinalyOrder(User.Identity.GetEmail(), id))
            {
                return Redirect("/UserPanel/MyOrders/ShowOrder/" + id + "?finaly=true");
            }

            return BadRequest();
        }

        public IActionResult UseDiscount(int orderId, string code)
        {
            DiscountUseType type = _orderService.UseDiscount(orderId, code);
            return Redirect("/UserPanel/MyOrders/ShowOrder/" + orderId + "?type="+type);
        }

        public IActionResult UserCourses()
        {
            return View(_orderService.GetCoursesForDownload(User.Identity.GetEmail()));
        }

        [Route("DownloadFile/{courseId}")]
        public IActionResult DownloadFile(int courseId)
        {
            var course = _courseService.GetCourseById(courseId);
            if (course != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (_orderService.IsUserInCourse(User.Identity.GetEmail(), course.CourseId))
                    {
                        string filePath = Path.Combine(_environment.WebRootPath, "course/download",
                            course.CourseFileName);
                        string fileName = course.CourseFileName;
                        byte[] file = System.IO.File.ReadAllBytes(filePath);
                        return File(file, "application/force-download", fileName);

                    }
                }
            }

            return Forbid();
        }
    }
}