using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Utility;

namespace Store_Musfer_Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            var Cateories = _UnitOfWork.category.GetAll().ToList();
            return View(Cateories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.category.Add(category);
                _UnitOfWork.Save();
                TempData["success"] = "Category Created Successfuly";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public IActionResult Edit(int id)
        {
            Category category = _UnitOfWork.category.Get(ca => ca.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category Newcategory)
        {

            if (ModelState.IsValid)
            {
                // Oldcategory.Name = Newcategory.Name;
                // Oldcategory.DisplayOrder = Newcategory.DisplayOrder;
                _UnitOfWork.category.update(Newcategory);
                _UnitOfWork.Save();
                TempData["success"] = "Category  Updated Successfuly";
                return RedirectToAction("Index");
            }
            else
            {
                return View(Newcategory);
            }

        }
        public IActionResult Delete(int id)
        {
            Category category = _UnitOfWork.category.Get(ca => ca.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            Category DeleteCategory = _UnitOfWork.category.Get(ca => ca.Id == category.Id);

            if (DeleteCategory == null)
            {
                return NotFound();
            }
            else
            {
                _UnitOfWork.category.Remove(DeleteCategory);
                _UnitOfWork.Save();
                TempData["success"] = "Category Deleted Successfuly";
                return RedirectToAction("Index");
            }
        }
    }
}
