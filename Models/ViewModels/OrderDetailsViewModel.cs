using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models.ViewModels
{
    public class OrderDetailsViewModel//order confirm ekranı icin ekranın solunda orderheader sagında orderrdetail bilgielrini gostericez.
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
