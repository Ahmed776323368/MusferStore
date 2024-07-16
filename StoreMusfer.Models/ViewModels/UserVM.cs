using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.Models.ViewModels
{
    public class UserVM
    {
        public ApplicationUser user { get; set; }
        public IEnumerable<SelectListItem>? roleList { get; set; }
        public IEnumerable<SelectListItem>? companyList { get; set; }
       

    }
}
