using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Wallet
{
   public class WalletType
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeTitle { get; set; }


        #region Relations

        public List<Wallet> Wallets { get; set; }

        #endregion
    }
}
