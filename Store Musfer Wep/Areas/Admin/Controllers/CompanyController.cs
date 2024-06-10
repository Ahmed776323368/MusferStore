using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreMusfer.DateAccsse.Repository;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Models.ViewModels;
using StoreMusfer.Utility;

namespace Store_Musfer_Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
    
        public CompanyController(IUnitOfWork UnitOfWork)
        {
          
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            var Companys = _UnitOfWork.company.GetAll().ToList();
            return View(Companys);
        }
         public IActionResult Upsert(int? id )
        {
           
               

            
            if (id == null || id == 0)
            {
                //created
                Company company = new Company();
                return View(company);
            }
            else
            {
                //Update
                Company company = _UnitOfWork.company.Get(com => com.Id == id);
                return View(company);
            }



        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid) {
                
               
                if (company.Id == 0)
                {
                    _UnitOfWork.company.Add(company);
                }
                else
                {
                    _UnitOfWork.company.update(company);
                }
                
                _UnitOfWork.Save();
                TempData["success"] = "Company  Created Successfuly";
                return RedirectToAction("Index");
            }
            else
            {
              

                return View(company);
            }

                
          
        }
        

           
    

        #region Api Calls
        [HttpGet]  
        public IActionResult GetAll()
        {
            var Companys = _UnitOfWork.company.GetAll().ToList();
            return Json(new {data= Companys }  );
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _UnitOfWork.company.Get(com => com.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Erorr while deleting " });

            }

            
            _UnitOfWork.company.Remove(CompanyToBeDeleted);
            _UnitOfWork.Save();
            return Json(new {success =true , message= "Company Deleted Successfuly" });
        }

       #endregion

    }
}
