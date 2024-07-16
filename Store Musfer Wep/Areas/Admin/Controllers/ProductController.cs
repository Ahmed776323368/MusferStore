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
                productVM.product= _UnitOfWork.product.Get(pro => pro.Id == id,includeProperties: "productImages");
                return View(productVM);
            }



        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM , List<IFormFile> files)
        {
            if (ModelState.IsValid) {

                if (productVM.product.Id == 0)
                {
                    _UnitOfWork.product.Add(productVM.product);
                }
                else
                {
                    _UnitOfWork.product.update(productVM.product);
                }
                _UnitOfWork.Save();
                string wwwrootWep = _webHostEnvironment.WebRootPath;
                
                if (files != null)
                 {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string prductPath = @"images\products\product-"+productVM.product.Id;
                        string finelPath = Path.Combine(wwwrootWep, prductPath);

                        if (!System.IO.Directory.Exists(finelPath))
                        {
                            System.IO.Directory.CreateDirectory(finelPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finelPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new ProductImage()
                        {
                            ImageUrl = @"\" + prductPath + @"\" + fileName,
                            ProductId = productVM.product.Id
                        };
                        if (productVM.product.productImages == null)
                        {
                            productVM.product.productImages = new List<ProductImage>();
                        }

                        productVM.product.productImages.Add(productImage);
                    }
                    _UnitOfWork.product.update(productVM.product);
                    _UnitOfWork.Save();


                 }



                TempData["success"] = "Product  Created/Update Successfuly";
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
        public IActionResult DeletImage(int ImageId)
        {
            var image = _UnitOfWork.productImage.Get(im => im.Id == ImageId);
            var productId = image.ProductId;
            if (image != null)
            {
                string wwwrootWep = _webHostEnvironment.WebRootPath;
                string finelPath = Path.Combine(wwwrootWep, image.ImageUrl.Substring(1));

                if (System.IO.File.Exists(finelPath))
                {
                    System.IO.File.Delete(finelPath);
                }

                _UnitOfWork.productImage.Remove(image);
                _UnitOfWork.Save();
                TempData["success"] = "Delete Successfuly";
               
            }
            return RedirectToAction(nameof(Upsert), new { id = productId });

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
            string finelPath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\products\product-"+id);

            if (System.IO.Directory.Exists(finelPath))
            {
                string[] filePathes = Directory.GetFiles(finelPath);
                foreach(var filePath in filePathes)
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.Directory.Delete(finelPath);
            }

            _UnitOfWork.product.Remove(ProductToBeDeleted);
            _UnitOfWork.Save();
            return Json(new {success =true , message= "Product Deleted Successfuly" });
        }

       #endregion

    }
}
