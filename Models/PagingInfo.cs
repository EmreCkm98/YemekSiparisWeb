using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models
{
    public class PagingInfo//page kısmını kendimiz yapıcaz o yüzden class olusturduk.kendi tag helperımızı yapıcaz.taghelper klasorunde class yaptık.
    {
        public int TotalItem { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage => (int)Math.Ceiling((decimal)TotalItem/ItemsPerPage);
        public string UrlParam { get; set; }//url bilgisini tutucak.
    }
}
