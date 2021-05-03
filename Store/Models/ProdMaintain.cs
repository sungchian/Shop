using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopee1.Models
{
    public class ProdMaintain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string Price { get; set; }
        public string PublishDate { get; set; }
        public string Status { get; set; }
        public string ImgURL { get; set; }
        public string Quantity { get; set; }

    }
}
