using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using YemekSiparisWeb.Models;
using YemekSiparisWeb.Models.ViewModels;
using YemekSiparisWeb.Utility;

namespace YemekSiparisWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategory.Include(s=>s.Category).ToListAsync();
            return View(subCategory);
        }
        //get-create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.SubCategory.Name && x.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count()>0)
                {
                    StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category.Please use another name.";
                }
                else
                {
                    await _db.SubCategory.AddAsync(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).ToListAsync(),
                StatusMessage=StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)//parametreden gelen kategoriye gore subcategorileri listeliyoz.
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await(from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            return Json(new SelectList(subCategories,"Id","Name"));
        }

        //get-edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m=>m.Id==id);
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.SubCategory.Name && x.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category.Please use another name.";
                }
                else
                {
                    var subCategoryFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCategoryFromDb.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            //modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                var willDelete = await _db.SubCategory.FindAsync(id);
                if (willDelete == null)
                {
                    return View();
                }
                _db.SubCategory.Remove(willDelete);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(a => a.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
    }
}
