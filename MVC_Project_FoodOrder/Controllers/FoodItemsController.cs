using MVC_Project_FoodOrder.Models;
using MVC_Project_FoodOrder.ViewModels;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using System.Threading;

namespace MVC_Project_FoodOrder.Controllers
{
    [Authorize(Roles = "Admin, Consumers")]
    public class FoodItemsController : Controller
    {
        private readonly FoodDbContext db = new FoodDbContext();
        // GET: FoodItems
        public ActionResult Index()
        {
           
            return View();
        }
        public PartialViewResult FoodItemTable(int pg = 1)
        {
            //var data = db.FoodItems.OrderBy(x => x.FoodItemId).ToPagedList(pg, 5);//lazy loading
            var data = db.FoodItems
                .Include(x => x.Orders)
                .Include(x => x.Category)               
                .OrderBy(x => x.FoodItemId)
                .ToPagedList(pg, 2);
            Thread.Sleep(1000); 
            return PartialView("_FoodItemTable", data);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
           
            return View();
        }
        public ActionResult CreateForm()
        {
            FoodInputModel model = new FoodInputModel();
            model.Orders.Add(new Order());
            ViewBag.Categories = db.Categories.ToList();
            return PartialView("_CreateForm", model);
        }
        [HttpPost]
        public ActionResult Create(FoodInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Orders.Add(new Order());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Orders.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var fooditem = new FoodItem
                    {
                        CategoryId = model.CategoryId,
                        FoodName = model.FoodName,
                        UnitPrice = model.UnitPrice,                        
                        Available = model.Available
                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                    model.Picture.SaveAs(savePath);
                    fooditem.Picture = f;

                    db.FoodItems.Add(fooditem);
                    db.SaveChanges();
                    foreach (var s in model.Orders)
                    {
                        db.Database.ExecuteSqlCommand($@"EXEC InsertOrder '{s.OrderDate}', {s.Quantity}, {(int)s.OrderType},  {fooditem.FoodItemId}");
                    }
                    FoodInputModel newmodel = new FoodInputModel() { FoodName = "" };
                    newmodel.Orders.Add(new Order());
                    ViewBag.Categories = db.Categories.ToList();
                    foreach (var e in ModelState.Values)
                    {

                        e.Value = null;
                    }
                    return View("_CreateForm", newmodel);
                }
            }
            ViewBag.Categories = db.Categories.ToList();
            return View("_CreateForm", model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            ViewBag.Id = id;
            return View();
        }
        public ActionResult EditForm(int id)
        {
            var data = db.FoodItems.FirstOrDefault(x => x.FoodItemId == id);
            if (data == null) return new HttpNotFoundResult();
            db.Entry(data).Collection(x => x.Orders).Load();
            FoodEditModel model = new FoodEditModel
            {
                FoodItemId = id,
                CategoryId = data.CategoryId,               
                FoodName = data.FoodName,
                UnitPrice = data.UnitPrice,
                Available = data.Available,
                Orders = data.Orders.ToList()
            };
            ViewBag.Categories = db.Categories.ToList();          
            ViewBag.CurrentPic = data.Picture;
            return PartialView("_EditForm", model);
        }

        [HttpPost]
        public ActionResult Edit(FoodEditModel model, string act = "")
        {
            if (act == "add")
            {
                model.Orders.Add(new Order());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Orders.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    var food = db.FoodItems.FirstOrDefault(x => x.FoodItemId == model.FoodItemId);
                    if (food == null) { return new HttpNotFoundResult(); }
                    food.FoodName = model.FoodName;
                    food.UnitPrice = model.UnitPrice;
                    food.Available = model.Available;
                    food.CategoryId = model.CategoryId;
                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                        model.Picture.SaveAs(savePath);
                        food.Picture = f;
                    }

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand($"EXEC DeleteOrderByFoodItemId {food.FoodItemId}");
                    foreach (var s in model.Orders)
                    {
                        db.Database.ExecuteSqlCommand($"EXEC InsertOrder '{s.OrderDate}', {s.Quantity}, {(int)s.OrderType}, {food.FoodItemId}");
                    }


                }
            }
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.CurrentPic = db.FoodItems.FirstOrDefault(x => x.FoodItemId == model.FoodItemId)?.Picture;
            return View("_EditForm", model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var fooditem = db.FoodItems.FirstOrDefault(x => x.FoodItemId == id);
            if (fooditem == null) return new HttpNotFoundResult();
            db.Orders.RemoveRange(fooditem.Orders.ToList());
            db.FoodItems.Remove(fooditem);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public ActionResult CreateOrder()
        {
            var model = new Order();
            ViewBag.FoodItems = db.FoodItems.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateOrder(Order model)
        {

            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    FoodItemId = model.FoodItemId,
                    OrderDate = model.OrderDate,
                    Quantity = model.Quantity,
                    OrderType = model.OrderType                
                };
                db.Orders.Add(order);
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            ViewBag.FoodItems = db.FoodItems.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult OrderDelete(int id)
        {
            var order = db.Orders.FirstOrDefault(x => x.OrderId == id);
            if (order == null) return new HttpNotFoundResult();
            db.Orders.Remove(order);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}