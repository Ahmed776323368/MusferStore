using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StoreMusfer.DateAccsse.Repository;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using StoreMusfer.Models.ViewModels;
using StoreMusfer.Utility;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;

namespace Store_Musfer_Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Details(int orderId)
        {
             orderVM = new()
            {
                OrderHeader = _UnitOfWork.OrderHeader.Get(or => or.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _UnitOfWork.OrderDetail.GetAll(or => or.OrderHeaderId == orderId, includeProperties: "Product").ToList()
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin +" ,"+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(or => or.Id == orderVM.OrderHeader.Id);
            OrderHeaderFromDB.Name = orderVM.OrderHeader.Name;
            OrderHeaderFromDB.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDB.PostalCode = orderVM.OrderHeader.PostalCode;
            OrderHeaderFromDB.StreetAdress = orderVM.OrderHeader.StreetAdress;
            OrderHeaderFromDB.City = orderVM.OrderHeader.City;
            OrderHeaderFromDB.State= orderVM.OrderHeader.State;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            }
            _UnitOfWork.OrderHeader.update(OrderHeaderFromDB);
            _UnitOfWork.Save();
            TempData["success"] = "Order Details  Updated Successfuly";
            return RedirectToAction(nameof(Details), new {orderId=OrderHeaderFromDB.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + " ," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _UnitOfWork.OrderHeader.updateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _UnitOfWork.Save();
            TempData["success"] = "Order Details  Updated Successfuly";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + " ," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(or => or.Id == orderVM.OrderHeader.Id);
            OrderHeaderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            OrderHeaderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            OrderHeaderFromDB.OrderStatus = SD.StatusShipped;
            OrderHeaderFromDB.ShippingDate = DateTime.Now;
            if (orderVM.OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                OrderHeaderFromDB.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _UnitOfWork.OrderHeader.update(OrderHeaderFromDB);
            _UnitOfWork.Save();
            TempData["success"] = "Order Details  Updated Successfuly";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + " ," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(or => or.Id == orderVM.OrderHeader.Id);
            if (OrderHeaderFromDB.PaymentStatus == SD.PaymentStatusApproved)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderHeaderFromDB.PaymentIntentId
                };
                var server = new RefundService();
                Refund refund = server.Create(option);
                _UnitOfWork.OrderHeader.updateStatus(OrderHeaderFromDB.Id, SD.StatusCancelled, SD.StatusRefunded);
                
            }
            else
            {
                _UnitOfWork.OrderHeader.updateStatus(OrderHeaderFromDB.Id, SD.StatusCancelled, SD.StatusCancelled);
               
            }
            _UnitOfWork.Save();
            TempData["success"] = "Order Details  Cancelled Successfuly";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + " ," + SD.Role_Employee)]
        public IActionResult Details_PAY_NOW()
        {
            orderVM.OrderHeader = _UnitOfWork.OrderHeader.Get(or => or.Id == orderVM.OrderHeader.Id);
            orderVM.OrderDetail = _UnitOfWork.OrderDetail.GetAll(or => or.OrderHeaderId == orderVM.OrderHeader.Id,includeProperties:"Product").ToList();

            var admin = "https://localhost:7057/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = admin + $"Admin/Order/PaymentConfirmation?OrderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = admin + $"/Admin/Order/Details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
            };

            foreach (var item in orderVM.OrderDetail)
            {
                var SessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count

                };
                options.LineItems.Add(SessionLineItem);

            }
            var service = new SessionService();
            Session session = service.Create(options);
            _UnitOfWork.OrderHeader.updateStripePaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _UnitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

           
        }

        public IActionResult PaymentConfirmation(int OrderHeaderId)
        {
            OrderHeader orderHeader = _UnitOfWork.OrderHeader.Get(or => or.Id == OrderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var Server = new SessionService();
                Session session = Server.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _UnitOfWork.OrderHeader.updateStripePaymentID(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _UnitOfWork.OrderHeader.updateStatus(orderHeader.Id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _UnitOfWork.Save();
                }
            }
            return View(OrderHeaderId);
        }
        #region Api Calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
         
            IEnumerable<OrderHeader> orderHeaders; 
            if(User.IsInRole(SD.Role_Employee)|| User.IsInRole(SD.Role_Admin))
            {
                orderHeaders = _UnitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var ClaimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = _UnitOfWork.OrderHeader.GetAll(or => or.ApplicationUserId == UserId, includeProperties: "ApplicationUser").ToList();
               
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(or => or.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "Inprocess":
                    orderHeaders = orderHeaders.Where(or => or.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(or => or.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(or => or.OrderStatus == SD.StatusApproved);
                    break;
                default:
                   
                    break;


            }
            return Json(new { data = orderHeaders });


        }

       

        #endregion
    }
}
