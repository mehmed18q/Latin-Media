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
    [PermissionChecker(19)]
    public class CreateGroupModel : PageModel
    {
        private ICourseService _courseService;

        public CreateGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty] //two way binding
        public CourseGroup CourseGroup { get; set; }
        public void OnGet(int? id)
        {
           CourseGroup=new CourseGroup()
           {
               ParentId = id
           };
        }

        public IActionResult OnPost()
        {
         
            if (!ModelState.IsValid)
                return Page();

            _courseService.AddCourseGroup(CourseGroup);
            return RedirectToPage("Index");

        }
    }
}