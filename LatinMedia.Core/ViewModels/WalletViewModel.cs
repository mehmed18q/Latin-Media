using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LatinMedia.Core.ViewModels
{
   public class ChargeWalletViewModel
    {
        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Amount { get; set; } 

    }

   public class WalletInfoViewModel

   {
       public int Amount { get; set; }
       public int Type { get; set; }
       public DateTime DateTime { get; set; }
       public string Description { get; set; }

   }


}
