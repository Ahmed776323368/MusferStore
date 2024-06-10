using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.Models
{
    public class Product
    {
        [Key]
       public  int Id { get; set; }

       public string Title { get; set; }
       public string  Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        [DisplayName("List price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }
        [DisplayName(" price For 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [DisplayName(" price For 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [DisplayName(" price For 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey("CategoryId")]
        public Category? category { get; set; }





    }
}
