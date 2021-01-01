using System;
using System.Collections.Generic;
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
    [Authorize(Roles =SD.ManagerUser)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }
        public async Task<IActionResult>Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _db.Category.AddAsync(category);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //get-edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id!=null)
            {
                var willEdit = await _db.Category.FindAsync(id);
                if (willEdit==null)
                {
                    return NotFound();
                }
                return View(willEdit);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                 _db.Category.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var willEdit = await _db.Category.FindAsync(id);
                if (willEdit == null)
                {
                    return NotFound();
                }
                return View(willEdit);
            }
            return NotFound();
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                var willDelete = await _db.Category.FindAsync(id);
                if (willDelete == null)
                {
                    return View();
                }
                _db.Category.Remove(willDelete);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                var willDetails = await _db.Category.FindAsync(id);
                if (willDetails == null)
                {
                    return NotFound();
                }
                return View(willDetails);
            }
            return NotFound();
        }
    }
}
