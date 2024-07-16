using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreMusfer.DateAccsse.Date;
using StoreMusfer.DateAccsse.Repository;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Models.ViewModels;
using StoreMusfer.Utility;
using System.Collections.Immutable;

namespace Store_Musfer_Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly  unitOfWork;

        
        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RoleManagment(string id)
        {
            var User = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
                User.Role = _userManager.GetRolesAsync(User).GetAwaiter().GetResult().FirstOrDefault();

            IEnumerable<SelectListItem> RoleList = _roleManager.Roles.Select(r => new SelectListItem()
            {
                Text = r.Name,
                Value = r.Name,
                Selected = (r.Name ==User.Role)

            });
            IEnumerable<SelectListItem> copmanies = _unitOfWork.company.GetAll().Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
                Selected = (c.Id == User.CompanyId)

            });

            UserVM UserVW = new()
            {
                user = User,
                roleList = RoleList,
                companyList = copmanies

            };


            return View(UserVW);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleManagment(UserVM userVM)
        {
            var User = _unitOfWork.ApplicationUser.Get(u => u.Id == userVM.user.Id);
            var Newrole = userVM.user.Role;
            var Oldrole = _userManager.GetRolesAsync(User).GetAwaiter().GetResult().FirstOrDefault(); 
            if (Oldrole!=Newrole)
            {

              
                    if ( Newrole== SD.Role_Company)
                    {
                       User.CompanyId = userVM.user.CompanyId;
                    }
                    else
                    {
                       
                        if (User.CompanyId != null)
                        {
                            User.CompanyId = null;
                        }
                    }

                _unitOfWork.ApplicationUser.update(User);
                _unitOfWork.Save();
                _userManager.RemoveFromRoleAsync(User, Oldrole).GetAwaiter().GetResult() ;
                _userManager.AddToRoleAsync(User,Newrole).GetAwaiter().GetResult();
               
              
            }
            else
            {
                if (Oldrole == SD.Role_Company)
                {
                    User.CompanyId = userVM.user.CompanyId;
                    _unitOfWork.ApplicationUser.update(User);
                    _unitOfWork.Save();
                }
              
            }

            return RedirectToAction(nameof(Index));

        }
        




        #region Api Calls
        [HttpGet]  
        public IActionResult GetAll()
        {
            List<ApplicationUser> Users = _unitOfWork.ApplicationUser.GetAll(includeProperties: "company").ToList();
        ;
          
            foreach (var user in Users)
            {
              
                    user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.company == null)
                {
                    user.company = new () {Name=""};
                }
            }



            return Json(new {data= Users }  );
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false,message="Error while Locking/Unlocking" });
            }
            else
            { 
                if(user.LockoutEnd!=null&& user.LockoutEnd > DateTime.Now)
                {
                    user.LockoutEnd = DateTime.Now;
                }
                else
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                }
                _unitOfWork.ApplicationUser.update(user);
                _unitOfWork.Save();
            }
            return Json(new { success = true, message = "Opertion Successful" });
        }

       #endregion

    }
}
