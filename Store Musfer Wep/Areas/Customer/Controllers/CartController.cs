using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreMusfer.DateAccsse.Repository;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Models.ViewModels;
using StoreMusfer.Utility;

using Stripe.Checkout;
using System.Security.Claims;

namespace Store_Musfer_Wep.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCardList = _unitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == UserId, includeProperties: "product"),
                orderHeader=new()
            };
            var productImages = _unitOfWork.productImage;
            foreach(var cart in ShoppingCartVM.ShoppingCardList)
            {
                cart.product.productImages = productImages.GetAll(im => im.ProductId == cart.product.Id).ToList();
                cart.Price = GetPriceBasedOnQuintity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price*cart.Count);

            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCardList = _unitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == UserId, includeProperties: "product"),
                orderHeader = new()
            };
            ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(us => us.Id == UserId);
            ShoppingCartVM.orderHeader.ApplicationUserId = ShoppingCartVM.orderHeader.ApplicationUser.Id;
            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
            ShoppingCartVM.orderHeader.StreetAdress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
            ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode; 
            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                cart.Price = GetPriceBasedOnQuintity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);

            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           
           ApplicationUser  applicationUser= _unitOfWork.ApplicationUser.Get(us => us.Id == UserId);
            ShoppingCartVM.ShoppingCardList = _unitOfWork.shoppingCard.GetAll(car => car.ApplicationUserId == UserId,includeProperties:"product");
            ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationUserId = UserId;

            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                cart.Price = GetPriceBasedOnQuintity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);

            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }
            if (ModelState.IsValid && ShoppingCartVM.ShoppingCardList.Count()>0)
            {
                _unitOfWork.OrderHeader.Add(ShoppingCartVM.orderHeader);
                _unitOfWork.Save();
            }
            else
            {
              
                return RedirectToAction(nameof(Summary));
            }
            

            foreach(var cart in ShoppingCartVM.ShoppingCardList)
            {
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = ShoppingCartVM.orderHeader.Id,
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var admin = "https://localhost:7057/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = admin+$"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.orderHeader.Id}",
                    CancelUrl= admin + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
   
                    Mode = "payment",
                };

                foreach(var item in ShoppingCartVM.ShoppingCardList)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Title
                            }
                        },
                        Quantity=item.Count

                    };
                    options.LineItems.Add(SessionLineItem);

                }
                var service = new SessionService();
               Session session= service.Create(options);
                _unitOfWork.OrderHeader.updateStripePaymentID(ShoppingCartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }


            return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.orderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(or => or.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var Server = new SessionService();
                Session session = Server.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.updateStripePaymentID(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.updateStatus(orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            HttpContext.Session.Clear();
            List<ShoppingCard> shoppingCards = _unitOfWork.shoppingCard.GetAll(us => us.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.shoppingCard.RemoveRange(shoppingCards);
            _unitOfWork.Save(); 
            return View(id);
        }
        public IActionResult Plus(int CartId)
        {
            var CartFromDb = _unitOfWork.shoppingCard.Get(car => car.Id == CartId);
            CartFromDb.Count += 1;
            _unitOfWork.shoppingCard.update(CartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int CartId)
        {
            var CartFromDb = _unitOfWork.shoppingCard.Get(car => car.Id == CartId,tracked:true);
            if (CartFromDb.Count == 1)
            {
             
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == CartFromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.shoppingCard.Remove(CartFromDb);
            }
            else
            {
                CartFromDb.Count -= 1;
                _unitOfWork.shoppingCard.update(CartFromDb);
            }
            
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int CartId)
        {
            var CartFromDb = _unitOfWork.shoppingCard.Get(car => car.Id == CartId,tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCard.GetAll(sh => sh.ApplicationUserId == CartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.shoppingCard.Remove(CartFromDb);
           
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
            private double GetPriceBasedOnQuintity(ShoppingCard shoppingCard)
        {
            if (shoppingCard.Count <= 50)
            {
                return shoppingCard.product.Price;
            }
            else if(shoppingCard.Count <= 100)
            {
                return shoppingCard.product.Price50;
            }
            else
            {
                return shoppingCard.product.Price100;
            }
        }
    }
}
