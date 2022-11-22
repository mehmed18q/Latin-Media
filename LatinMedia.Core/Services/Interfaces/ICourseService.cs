using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Entities.Company;
using LatinMedia.DataLayer.Entities.Course;
using LatinMedia.DataLayer.Entities.Teacher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LatinMedia.Core.Services.Interfaces
{
    public interface ICourseService
    {
        #region Groups
        List<CourseGroup> GetAllGroups();
        List<SelectListItem> GetGroupsForManageCourse();
        List<SelectListItem> GetSubGroupsForManageCourse(int groupId);
        List<SelectListItem> GetSecondSubGroupsForManageCourse(int subGroupId);
        void AddCourseGroup(CourseGroup @group);
        void UpdateCourseGroup(CourseGroup @group);
        CourseGroup GetGroupById(int groupId);

        #endregion

        #region Teacher

        List<Teacher> GetAllTeachers();
        List<SelectListItem> GetTeachersForManageCourse();
        List<ShowAllTeachersViewModel> ShowAllTeachers();
        int GetTeachersCount();
        #endregion

        #region Levels

        List<SelectListItem> GetLevelsForManageCourse();

        #endregion

        #region Company

        List<Company> GetAllCompanies();
        List<SelectListItem> GetCompaniesForManageCourse();
        List<ShowAllCompaniesViewModel> ShowAllCompanies();
        int GetCompaniesCount();
        int GetSumTimeCourses();

        #endregion

        #region Course Types

        List<SelectListItem> GetCourseTypesForManageCourse();
        List<CourseType> GetAllCourseTypes();

        #endregion

        #region Course

        List<ShowCourseForAdminViewModel> GetCoursesForAdmin();
        int AddCourse(Course course, IFormFile imgCourseUp, IFormFile courseFile);
        Course GetCourseById(int courseId);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseFile);

        Tuple<List<ShowCourseListItemViewModel>,int> GetCourses(int pageId = 1, string filter = "", int type = 0,
            int minPrice = 0, int maxPrice = 0, List<int> selectedGroups=null,int take=0,int companyId=0,int teacherId=0);

        List<ShowCourseListItemViewModel> GetSpecialCourses();

        List<ShowCourseListItemViewModel> GetPopularCourses();

        Course GetCourseForShow(int id);
        int CourseCount();
        #endregion

        #region Comment

        void AddComment(CourseComment comment);

        Tuple<List<CourseComment>,int> GetCourseComments(int courseId, int pageId=1);
        int CourseCommentCount();

        #endregion

    }
}
