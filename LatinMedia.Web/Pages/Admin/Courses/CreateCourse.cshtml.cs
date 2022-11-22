using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.DataLayer.Entities.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LatinMedia.Web.Pages.Admin.Courses
{
    [PermissionChecker(11)]
    public class CreateCourseModel : PageModel
    {
        private ICourseService _courseService;

        public CreateCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }


        [BindProperty]
        public Course Course { get; set; }
        public void OnGet()
        {
            var groups = _courseService.GetGroupsForManageCourse();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text");

            var subGroups = _courseService.GetSubGroupsForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGroups"]=new SelectList(subGroups,"Value","Text");

            var secondSubGroups = _courseService.GetSecondSubGroupsForManageCourse(int.Parse(subGroups.First().Value));
            ViewData["SecondSubGroups"] = new SelectList(secondSubGroups, "Value", "Text");

            var levels = _courseService.GetLevelsForManageCourse();
            ViewData["Levels"]=new SelectList(levels,"Value","Text");

            var types = _courseService.GetCourseTypesForManageCourse();
            ViewData["Types"] = new SelectList(types, "Value", "Text");

            var teachers = _courseService.GetTeachersForManageCourse();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

            var companies = _courseService.GetCompaniesForManageCourse();
            ViewData["Companies"] = new SelectList(companies, "Value", "Text");

        }

        public IActionResult OnPost(IFormFile courseFile,IFormFile imgCourseUp)
        {
         
            if (!ModelState.IsValid)
            {
                var groups = _courseService.GetGroupsForManageCourse();
                ViewData["Groups"] = new SelectList(groups, "Value", "Text");

                var subGroups = _courseService.GetSubGroupsForManageCourse(int.Parse(groups.First().Value));
                ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text");

                var secondSubGroups = _courseService.GetSecondSubGroupsForManageCourse(int.Parse(subGroups.First().Value));
                ViewData["SecondSubGroups"] = new SelectList(secondSubGroups, "Value", "Text");

                var levels = _courseService.GetLevelsForManageCourse();
                ViewData["Levels"] = new SelectList(levels, "Value", "Text");

                var types = _courseService.GetCourseTypesForManageCourse();
                ViewData["Types"] = new SelectList(types, "Value", "Text");

                var teachers = _courseService.GetTeachersForManageCourse();
                ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

                var companies = _courseService.GetCompaniesForManageCourse();
                ViewData["Companies"] = new SelectList(companies, "Value", "Text");
                return Page();
            }

            _courseService.AddCourse(Course, imgCourseUp, courseFile);
            TempData["InsertCourse"] = true;
            return RedirectToPage("Index");
        }

    }
}