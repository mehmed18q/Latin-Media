using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.DataLayer.Entities.Course;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LatinMedia.Web.Pages.Admin.CourseGroups
{
    [PermissionChecker(20)]
    public class EditGroupModel : PageModel
    {
        private ICourseService _courseService;

        public EditGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public  CourseGroup CourseGroup { get; set; }
        public void OnGet(int id)
        {

            CourseGroup = _courseService.GetGroupById(id);
        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _courseService.UpdateCourseGroup(CourseGroup);

            return RedirectToPage("Index");
        }
    }
}