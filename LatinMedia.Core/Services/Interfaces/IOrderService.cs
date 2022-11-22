using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Entities.Course;
using LatinMedia.DataLayer.Entities.Order;

namespace LatinMedia.Core.Services.Interfaces
{
   public interface IOrderService
   {
        #region Order

        int AddOrder(string email, int courseId);
        void UpdateOrderPrice(int orderId);
        Order GetOrdersForUserPanel(string email, int orderId);
        Order GetOrderById(int orderId);
       
        bool FinalyOrder(string email, int orderId);

        List<Order> GetUserOrders(string email);
        void UpdateOrder(Order order);

        bool IsUserInCourse(string email, int courseId);

        List<UserCourse> GetCoursesForDownload(string email);


        List<Order> GetAllOrdersForAdminPanel();
        List<OrderDetail> GetOrderDetailsById(int orderId);

        int GetNotFinalyOrderCount();
        int GetSumOrderCourses();
        List<Course> GetFullOrderItems();
        #endregion

        #region Discount

        DiscountUseType UseDiscount(int orderId, string code);

        void AddDiscount(Discount discount);
        void UpdateDiscount(Discount discount);
        Discount GetDiscountById(int discountId);

        List<Discount> GetAllDiscounts();
        bool IsExistDiscount(string code);

        bool IsExistDiscountCodeForEdit(int discountId,string code);

        #endregion


    }
}
