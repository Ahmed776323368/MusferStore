using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.Models.ViewModels
{
    public class ShoppingCartVM
    {
        [ValidateNever]
        public IEnumerable<ShoppingCard> ShoppingCardList { get; set; }
        public OrderHeader orderHeader { get; set; }
        
    }
}
