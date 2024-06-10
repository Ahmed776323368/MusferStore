using StoreMusfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void update(OrderHeader obj);
        void updateStatus(int id,string OrderStatus,string? PaymentStatus=null);
        void updateStripePaymentID(int id,string SessionId, string PaymentIntentId );
    }
}
