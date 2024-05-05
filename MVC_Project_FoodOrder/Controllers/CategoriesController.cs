using MVC_Project_FoodOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace MVC_Project_FoodOrder.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly FoodDbContext db = new FoodDbContext();
        // GET: Categories
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(model);
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }
        public ActionResult Edit(int id)
        {
            var data = db.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (data == null) return new HttpNotFoundResult();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }
        [HttpPost]      
        public ActionResult Delete(int id)
        {
            var category = db.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null) return new HttpNotFoundResult();
            db.Categories.Remove(category);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public PartialViewResult ModelsTable(int pg = 1)
        {
            var data = db.Categories.OrderBy(x => x.CategoryId).ToPagedList(pg, 5);
            return PartialView("_Categories", data);
        }
    }
}