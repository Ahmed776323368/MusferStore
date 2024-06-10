using StoreMusfer.DateAccsse.Date;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }


        

        public void update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void updateStatus(int id, string OrderStatus, string? PaymentStatus = null)
        {
            var orderFromDb = _db.orderHeaders.FirstOrDefault(or => or.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = OrderStatus;

            }
            if (!string.IsNullOrEmpty(PaymentStatus)){

                orderFromDb.PaymentStatus = PaymentStatus;
            }
        }

        public void updateStripePaymentID(int id, string SessionId, string PaymentIntentId)
        {
            var orderFromDb = _db.orderHeaders.FirstOrDefault(or => or.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(SessionId))
                {

                    orderFromDb.SessionId = SessionId;
                }
                if (!string.IsNullOrEmpty(PaymentIntentId))
                {

                    orderFromDb.PaymentIntentId = PaymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }

            }
        }
    }
}
