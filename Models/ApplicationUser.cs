using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models
{//normalde user işlemlerini asp core yapmıştı.identy klasorunde account maange register.csde inputmodel clasına bizde bazı proplar ekledik.
    //databasede aspnetuser tablosuna bakarsak orda hazır kolonlar var.ama biz oraya adres postakodu gibi seyler eklemek istiyoruz.onu yapıcaz burda.
    public class ApplicationUser:IdentityUser
    {//bunlar databasede yoktu biz ekliyoruzz o yüzden.dbset olarak clası eklicez.daah sonra migration ile eklicez.
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
