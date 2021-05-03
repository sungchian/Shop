using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopee1.Models
{
    public class MShoppingCart
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string ImgURL { get; set; }
        public string Status { get; set; }
        public string onUse { get; set; }
    }
}
