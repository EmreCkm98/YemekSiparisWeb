using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Models;
using YemekSiparisWeb.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Data
{//projeyi deploy ederken bos bir database icin ya yeni bir database olusturup appsetingsten connectionu degisticez sonra update database diyince
    //o migrationlar otomatik tekrar olusucak.proje deploy edildiginde  bos bir dastabase olacagı icin baslangıc olarak asagıda rolleri olusturduk ve
    //1tane kullanıcı olusturduk.bu userı admin olarak ayaraldık yani deploy edince baslangıcta bir admin olacak onu ayarladık..
    //bu clas proje calısır calısmaz cagrılmalı.bunun icin startup.cs e geciyoruz.
    //appsetingten coonection stringten var olmayan bir database olan Spice2 yapsak consoledan update-database dersek database oluscak ve daha onceden
    //yaptıgımız tüm migrationlar tekrar oluscak keyler,foreing keyler gibi yani hicbisey silinmicek.sadece icindeki kayıtlar silinicek.
    //projeyi calıstırınca sadece olusturdugumuz 1tane user yani admin olucak.
    //onceden rol olusturma kısmını register.cs te yapıyorduk. ama artık dbinitializer clasında 1tane admin useri ve rolleri olusturduk.register.csde
    //sadece kayıt olunuyor ve admin rolündeki kisi kayıt yaaprken kullanıcının rolünü degistirebilir.
    public class DbInitializer : IDbInitializer//role olusturma kısmını burda yapıcaz.
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_db.Roles.Any(r=>r.Name==SD.ManagerUser))//eger bir role varsa return ediyor.yoksa asagıda rolleri ve 1tane admin user olusturuyor.
            {
                return;
            }

            _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.FrontDeskUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.KitchenUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();

            //ilk userimiz yani admin
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName="admin@gmail.com",
                Email= "admin@gmail.com",
                Name="Emre Çakmak",
                EmailConfirmed=true,
                PhoneNumber="1112223333"
            },"Emre1234.").GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");

            await _userManager.AddToRoleAsync(user, SD.ManagerUser);//ilk userımızı admin rolüne atadık.
        }
    }
}
