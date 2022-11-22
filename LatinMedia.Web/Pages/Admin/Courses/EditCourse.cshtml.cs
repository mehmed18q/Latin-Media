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
    [PermissionChecker(12)]
    public class EditCourseModel : PageModel
    {
        private ICourseService _courseService;

        public EditCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

      

        public void OnGet(int id)
        {
            Course = _courseService.GetCourseById(id);

            var groups = _courseService.GetGroupsForManageCourse();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text",Course.GroupId);

            List<SelectListItem> subGroupsList = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "انتخاب کنید",Value = ""}
            };

            subGroupsList.AddRange(_courseService.GetSubGroupsForManageCourse(Course.GroupId));
            string selectedSubGroup = null;
            if (Course.SubGroup != null)
            {
                selectedSubGroup = Course.SubGroup.ToString();
            }
            ViewData["SubGroups"] = new SelectList(subGroupsList, "Value", "Text", selectedSubGroup);


            List<SelectListItem> secondSubGroupsList = new List<SelectListItem>()
            {
                new SelectListItem(){Text = "انتخاب کنید",Value = ""}
            };

            secondSubGroupsList.AddRange(_courseService.GetSecondSubGroupsForManageCourse(Course.SubGroupId ?? 0));
            string selectedSecondSubGroup = null;
            if (Course.SecondSubGroup != null)
            {
                selectedSecondSubGroup = Course.SecondSubGroup.ToString();
            }

            ViewData["SecondSubGroups"] = new SelectList(secondSubGroupsList, "Value", "Text", selectedSecondSubGroup);

            var levels = _courseService.GetLevelsForManageCourse();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text",Course.LevelId);

            var types = _courseService.GetCourseTypesForManageCourse();
            ViewData["Types"] = new SelectList(types, "Value", "Text",Course.TypeId);

            var teachers = _courseService.GetTeachersForManageCourse();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text",Course.TeacherId);

            var companies = _courseService.GetCompaniesForManageCourse();
            ViewData["Companies"] = new SelectList(companies, "Value", "Text",Course.CompanyId);
        }

        public IActionResult OnPost(IFormFile courseFile, IFormFile imgCourseUp)
        {
            if (!ModelState.IsValid)
            {
                var groups = _courseService.GetGroupsForManageCourse();
                ViewData["Groups"] = new SelectList(groups, "Value", "Text", Course.GroupId);

                var subGroups = _courseService.GetSubGroupsForManageCourse(int.Parse(groups.First().Value));
                ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text", Course.SubGroupId ?? 0);

                var secondSubGroups = _courseService.GetSecondSubGroupsForManageCourse(int.Parse(subGroups.First().Value));
                ViewData["SecondSubGroups"] = new SelectList(secondSubGroups, "Value", "Text", Course.SecondSubGroupId ?? 0);

                var levels = _courseService.GetLevelsForManageCourse();
                ViewData["Levels"] = new SelectList(levels, "Value", "Text", Course.LevelId);

                var types = _courseService.GetCourseTypesForManageCourse();
                ViewData["Types"] = new SelectList(types, "Value", "Text", Course.TypeId);

                var teachers = _courseService.GetTeachersForManageCourse();
                ViewData["Teachers"] = new SelectList(teachers, "Value", "Text", Course.TeacherId);

                var companies = _courseService.GetCompaniesForManageCourse();
                ViewData["Companies"] = new SelectList(companies, "Value", "Text", Course.CompanyId);
                return Page();
            }

          
            _courseService.UpdateCourse(Course,imgCourseUp,courseFile);
            TempData["UpdateCourse"]=true;
            return RedirectToPage("Index");
        }
    }
}