using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using YemekSiparisWeb.Models;
using YemekSiparisWeb.Utility;

namespace YemekSiparisWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _db.Coupon.ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)//eğer file uploaded olduysa demek;
                {
                    byte[] p1 = null;
                    using(var fs1=files[0].OpenReadStream())//burada image yi byte dizisine donusturuyoruz.databasede oyle saklıcaz.
                    {
                        using(var ms1=new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();//image i byte dizisine cevirip saklıyortuz.
                        }
                    }
                    coupons.Picture = p1;//image mizi databasede byte dizisi olarak tutuyoruz.
                }
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupons);
        }

        //get-edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var willEdit = await _db.Coupon.FindAsync(id);
                if (willEdit == null)
                {
                    return NotFound();
                }               
                return View(willEdit);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupons)
        {
            if (coupons.Id==0)
            {
                return NotFound();
            }
            var couponFromDb = await _db.Coupon.Where(c => c.Id == coupons.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)//eğer file uploaded olduysa demek;
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())//burada image yi byte dizisine donusturuyoruz.databasede oyle saklıcaz.
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();//image i byte dizisine cevirip saklıyortuz.
                        }
                    }
                    couponFromDb.Picture = p1;//image mizi databasede byte dizisi olarak tutuyoruz.
                }

                _db.Coupon.Update(couponFromDb);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupons);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var willDetails = await _db.Coupon.FindAsync(id);
                if (willDetails == null)
                {
                    return NotFound();
                }
                return View(willDetails);
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var willDeleted = await _db.Coupon.FindAsync(id);
                if (willDeleted == null)
                {
                    return NotFound();
                }
                return View(willDeleted);
            }
            return NotFound();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                var willDelete = await _db.Coupon.FindAsync(id);
                if (willDelete == null)
                {
                    return View();
                }
                _db.Coupon.Remove(willDelete);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
