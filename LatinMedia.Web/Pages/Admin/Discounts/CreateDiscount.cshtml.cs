using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.DataLayer.Entities.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LatinMedia.Web.Pages.Admin.Discounts
{
    [PermissionChecker(15)]
    public class CreateDiscountModel : PageModel
    {
        private IOrderService _orderService;

        public CreateDiscountModel(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [BindProperty]
        public  Discount Discount { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string stDate = "", string endDate = "")
        {
            PersianCalendar pc=new PersianCalendar();
            
            if (!string.IsNullOrEmpty(stDate))
            {
                string[] std = stDate.Split("/");
                DateTime dt=new DateTime(int.Parse(std[2]),int.Parse(std[0]),int.Parse(std[1]),pc);
                Discount.StartDate = Convert.ToDateTime(dt.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                string[] endd = endDate.Split("/");
                DateTime dt = new DateTime(int.Parse(endd[2]), int.Parse(endd[0]), int.Parse(endd[1]), pc);
                Discount.EndDate = Convert.ToDateTime(dt.ToString(CultureInfo.InvariantCulture));
            }

            if (!ModelState.IsValid && _orderService.IsExistDiscount(Discount.DiscountCode))
                return Page();

            _orderService.AddDiscount(Discount);
            return RedirectToPage("Index");

        }

        //-- /Admin/Discounts/CreateDiscount?handler?CheckCode=code
        //  /Admin/Discounts/CreateDiscount/CheckCode
        public IActionResult OnGetCheckCode(string code)
        {
            return Content(_orderService.IsExistDiscount(code).ToString()); 
        }
    }
}