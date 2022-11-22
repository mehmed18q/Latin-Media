using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.DataLayer.Entities.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LatinMedia.Web.Pages.Admin.Orders
{
    public class OrderDetailsModel : PageModel
    {
        private IOrderService _orderService;

        public OrderDetailsModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<OrderDetail> OrderDetailsList { get; set; }
        public void OnGet(int id)
        {
            OrderDetailsList = _orderService.GetOrderDetailsById(id);
        }
    }
}