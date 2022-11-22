using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Context;
using LatinMedia.DataLayer.Entities.Course;
using LatinMedia.DataLayer.Entities.Order;
using LatinMedia.DataLayer.Entities.User;
using LatinMedia.DataLayer.Entities.Wallet;
using Microsoft.EntityFrameworkCore;

namespace LatinMedia.Core.Services
{
    public class OrderService : IOrderService
    {
        private LatinMediaContext _context;

        private IUserService _userService;

        public OrderService(LatinMediaContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public int AddOrder(string email, int courseId)
        {
            int userId = _context.Users.Single(u => u.Email == email).UserId;

            Order order = _context.Orders
                .FirstOrDefault(o => o.UserId == userId && !o.IsFinaly);
            var course = _context.Courses.Find(courseId);
            if (order == null) // اگر فاکتور بازی نداشت یک فاکتور جدید ایجاد کنه
            {
                order = new Order()
                {
                    IsFinaly = false,
                    OrderDate = DateTime.Now,
                    OrderSum = course.CoursePrice,
                    UserId = userId,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId = course.CourseId,
                            CoursePrice = course.CoursePrice,
                            OrderCount = 1,
                        }
                    }
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            else  // اگر فاکتور بازی را پیدا کرد
            {
                OrderDetail detail = _context.OrderDetails
                    .FirstOrDefault(d => d.OrderId == order.OrderId && d.CourseId == course.CourseId);
                if (detail != null)
                {
                    detail.OrderCount += 1;
                    _context.OrderDetails.Update(detail);
                }
                else
                {
                    detail = new OrderDetail()
                    {
                        CourseId = course.CourseId,
                        CoursePrice = course.CoursePrice,
                        OrderCount = 1,
                        OrderId = order.OrderId
                    };
                    _context.OrderDetails.Add(detail);
                }

                _context.SaveChanges();
                UpdateOrderPrice(order.OrderId);
            }

            return order.OrderId;


        }

        public void UpdateOrderPrice(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            order.OrderSum = _context.OrderDetails.Where(d => d.OrderId == order.OrderId)
                .Sum(d => d.CoursePrice);

            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public Order GetOrdersForUserPanel(string email, int orderId)
        {
            int userId = _context.Users.Single(u => u.Email == email).UserId;
            return _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Find(orderId);
        }

        public bool FinalyOrder(string email, int orderId)
        {
            int userId = _context.Users.Single(u => u.Email == email).UserId;
            var order = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null || order.IsFinaly)
            {
                return false;
            }

            if (_userService.BalanceWalletUser(email) >= order.OrderSum)
            {
                order.IsFinaly = true;
                _userService.AddWallet(new Wallet()
                {
                    Amount = order.OrderSum,
                    CreateDate = DateTime.Now,
                    Description = "فاکتور شماره # " + order.OrderId,
                    IsPay = true,
                    TypeId = 2,
                    UserId = userId,
                });
                _context.Orders.Update(order);
                foreach (var detail in order.OrderDetails)
                {
                    _context.UserCourses.Add(new UserCourse()
                    {
                        CourseId = detail.CourseId,
                        UserId = userId,
                    });
                }
                _context.SaveChanges();
                return true;
            }

            return false;

        }

        public List<Order> GetUserOrders(string email)
        {
            int userId = _context.Users.Single(u => u.Email == email).UserId;
            return _context.Orders.Where(o => o.UserId == userId).ToList();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public bool IsUserInCourse(string email, int courseId)
        {
            int userId = _userService.GetUserIdByEmail(email);

            return _context.UserCourses.Any(c => c.UserId == userId && c.CourseId == courseId);
        }

        public List<UserCourse> GetCoursesForDownload(string email)
        {
            int userId = _userService.GetUserIdByEmail(email);
            return _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Course)
                .ToList();

        }

        public List<Order> GetAllOrdersForAdminPanel()
        {
            return _context.Orders.Include(c => c.User)
                .Include(c => c.OrderDetails)
                .ThenInclude(od => od.Course)
                .OrderByDescending(c => c.OrderDate)
                .ToList();
        }

        public List<OrderDetail> GetOrderDetailsById(int orderId)
        {
            return _context.OrderDetails.Include(od => od.Course)
                .Where(od => od.OrderId == orderId)
                .ToList();

        }

        public int GetNotFinalyOrderCount()
        {
            return _context.Orders.Count(o => o.IsFinaly == false);
        }

        public int GetSumOrderCourses()
        {
            return _context.Orders.Where(o => o.IsFinaly).Sum(o => o.OrderSum);
        }

        public List<Course> GetFullOrderItems()
        {
            return _context.Courses.Include(c=> c.OrderDetails).ThenInclude(od=> od.Order)
                .Take(10)
                .ToList();

        }

        public DiscountUseType UseDiscount(int orderId, string code)
        {
            var discount = _context.Discounts.SingleOrDefault(d => d.DiscountCode == code);

            if (discount == null)
                return DiscountUseType.NotFound;

            if (discount.StartDate != null && discount.StartDate > DateTime.Now)
                return DiscountUseType.ExpireDate;

            if (discount.EndDate != null && discount.EndDate < DateTime.Now)
                return DiscountUseType.ExpireDate;

            if (discount.UsableCount != null && discount.UsableCount < 1)
                return DiscountUseType.Finished;

            var order = GetOrderById(orderId);

            if (_context.UserDiscountCodes.Any(d => d.UserId == order.UserId && d.DiscountId == discount.DiscountId))
            {
                return DiscountUseType.Used;
            }

            int percent = (order.OrderSum * discount.DiscountPercent) / 100;
            order.OrderSum = order.OrderSum - percent;
            UpdateOrder(order);

            if (discount.UsableCount != null)
            {
                discount.UsableCount -= 1;
            }

            _context.Discounts.Update(discount);

            _context.UserDiscountCodes.Add(new UserDiscountCode()
            {
                DiscountId = discount.DiscountId,
                UserId = order.UserId

            });
            _context.SaveChanges();

            return DiscountUseType.Success;
        }

        public void AddDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            _context.SaveChanges();
        }

        public void UpdateDiscount(Discount discount)
        {
            _context.Discounts.Update(discount);
            _context.SaveChanges();
        }

        public Discount GetDiscountById(int discountId)
        {
            return _context.Discounts.Find(discountId);
        }

        public List<Discount> GetAllDiscounts()
        {
            return _context.Discounts.ToList();
        }

        public bool IsExistDiscount(string code)
        {
            return _context.Discounts.Any(d => d.DiscountCode == code.ToLower());
        }

        public bool IsExistDiscountCodeForEdit(int discountId, string code)
        {
            return _context.Discounts.Any(d => d.DiscountCode == code.ToLower() && d.DiscountId != discountId);
        }
    }
}
