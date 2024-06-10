using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.Models
{
    public class ShoppingCard
    {
        public int Id { get; set; }

        [ValidateNever]
        public int ProductId { get; set; }
        [Range(1,1000,ErrorMessage ="Plase enter a value between 1 to 1000")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product product { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}