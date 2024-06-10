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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }


        

        public void update(OrderDetail obj)
        {
            _db.orderDetails.Update(obj);
        }
    }
}
