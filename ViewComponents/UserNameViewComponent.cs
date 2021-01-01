using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YemekSiparisWeb.ViewComponents//view component kullanımını gorucez burda.class olusturduk.clasın sonu viewcomponent ile bitmesi yeterli veya
{//viewcomponent clasından kalıtım alırız.login olunca sag ust kosede email yerine login olunan ismi gostermesini istiyoruz.bunu direk layout viewinden
    //yaapbilirdik ama viewcomponent kullanıp yapmak istedik.viewcomponent controller gibi.asagıda user bilgisini aldık controller gibi simdi view olusturcaz
    //asagıdaki metodun viewini olusturmak icin views-shared altında Components klasoru olusturduk.onunda altında bu clasla atnı adda olan klasor yaptık.
    //aynı isim olması zorunlu.bu klasor yapısını boyle yapmak best practise.daha sonra username klasorune add view yapıyoruz.view partial olmalı.
    //adı Default olmalı.default viewinde istedigimiz name yi yazdık.simdi bu viewcomponenti kullanabilmek icin loginpartial viewine gecicez.
    //cunku login olunca sag ust kosede email yerine login olunan ismi bu viewde.@await Component.InvokeAsync("UserName") kodu ile viewcomponenti kullan
    //dık.ordaki ınvokeasyns metodu bizim metodla aynı isimde ve username kısmıda clasımızla aynı olmalı.işimiz bitti
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult>InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);//login olan kullanıcı bilgilerini aldık.

            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == claims.Value);

            return View(userFromDb);
        }
    }
}
