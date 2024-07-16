using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Store_Musfer_Wep.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork UnitOfWork)
        {
            _logger = logger;
            _UnitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> prducts = _UnitOfWork.product.GetAll(includeProperties: "category,productImages");
            return View(prducts);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCard Card = new()
            {
                product = _UnitOfWork.product.Get(pro => pro.Id == ProductId, includeProperties: "category,productImages"),
                Count = 1,
                ProductId=ProductId
            };

          
            return View(Card);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCard card)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            card.ApplicationUserId = UserId;
            ShoppingCard GetFromDb = _UnitOfWork.shoppingCard.Get(sh => sh.ApplicationUserId == UserId && sh.ProductId == card.ProductId);
            if (GetFromDb != null)
            {
                GetFromDb.Count += card.Count;
                _UnitOfWork.shoppingCard.update(GetFromDb);

                _UnitOfWork.Save();
            }
            else
            {
                _UnitOfWork.shoppingCard.Add(card);
                _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _UnitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == UserId).Count());
            }
            
            TempData["success"] = "Card Updated Successfuly";

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}