using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using YemekSiparisWeb.Utility;

namespace YemekSiparisWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _db.ApplicationUser.Where(x=>x.Id!=claim.Value).ToListAsync());
        }
        public async Task<IActionResult> Lock(string id)//bir kullanıcı login olursan 4-5 kere yanlıs girerse locked oluyor.
        {
            if (id==null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);

            if (applicationUser==null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);//lock ikonuna basarsak kullanıcı 100yıl locked olucak.

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UnLock(string id)//bir kullanıcının kilidini,banını,engelini acmak icin;
        {
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now;//unlock ikonuyla kulalnıcıyı tekrar aktif ettik.

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
