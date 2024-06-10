using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreMusfer.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Range(1, 100)]
        [DisplayName("Disply Order")]
        public int DisplayOrder { get; set; }
        
    }
}
