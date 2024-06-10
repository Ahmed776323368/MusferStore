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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            var products = _UnitOfWork.product.GetAll(includeProperties:"category").ToList();
            return View(products);
        }
         public IActionResult Upsert(int? id )
        {
            ProductVM productVM = new()
            {
                categoryList = _UnitOfWork.category.GetAll().Select(
                ca => new SelectListItem
                {
                    Text = ca.Name,
                    Value = ca.Id.ToString()
                }
               ),
                product = new Product()

            };
            if (id == null || id == 0)
            {
                //created
                return View(productVM);
            }
            else
            {
                //Update
                productVM.product= _UnitOfWork.product.Get(pro => pro.Id == id);
                return View(productVM);
            }



        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM ,IFormFile? file)
        {
            if (ModelState.IsValid) {
                string wwwrootWep = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                   
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string ProductPath = Path.Combine(wwwrootWep, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.product.ImageUrl))
                    {
                        var oldImagPath = Path.Combine(wwwrootWep, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagPath))
                        {
                            System.IO.File.Delete(oldImagPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(ProductPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.product.ImageUrl = @"\images\product\"+fileName;
                }
                if (productVM.product.Id == 0)
                {
                    _UnitOfWork.product.Add(productVM.product);
                }
                else
                {
                    _UnitOfWork.product.update(productVM.product);
                }
                
                _UnitOfWork.Save();
                TempData["success"] = "Product  Created Successfuly";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.categoryList = _UnitOfWork.category.GetAll().Select(
                 ca => new SelectListItem
                 {
                     Text = ca.Name,
                     Value = ca.Id.ToString(),
                      Selected = ca.Id == productVM.product.Id
                 }
                );

                return View(productVM);
            }

                
          
        }
        

           
    

        #region Api Calls
        [HttpGet]  
        public IActionResult GetAll()
        {
            var products = _UnitOfWork.product.GetAll(includeProperties:"category").ToList();
            return Json(new {data= products }  );
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductToBeDeleted = _UnitOfWork.product.Get(pro => pro.Id == id);
            if (ProductToBeDeleted == null)
            {
                return Json(new { success = false, message = "Erorr while deleting " });

            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, ProductToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _UnitOfWork.product.Remove(ProductToBeDeleted);
            _UnitOfWork.Save();
            return Json(new {success =true , message= "Product Deleted Successfuly" });
        }

       #endregion

    }
}
