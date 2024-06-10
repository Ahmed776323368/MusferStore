using Microsoft.AspNetCore.Mvc;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Utility;
using System.Security.Claims;

namespace Store_Musfer_Wep.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
      public  ShoppingCartViewComponent (IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (Claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _unitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == Claim.Value).Count());
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
