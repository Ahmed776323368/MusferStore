using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//

namespace StoreMusfer.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; }
        public IEnumerable<SelectListItem>? categoryList { get; set; }

    }
}
