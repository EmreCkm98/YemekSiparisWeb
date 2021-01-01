using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models.ViewModels
{
    public class OrderListViewModel//order history actionunda bu viewmodeli kullanıcaz.normalde orderdetailsviewmodeli kullanıyoduk.
    {//ama paging islemleri icin olusturdugumuz clasıda kullanacagımız icin bu viewmodel ile 2sini birden tanımladık bunu kullanıcaz.
        public IList<OrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
