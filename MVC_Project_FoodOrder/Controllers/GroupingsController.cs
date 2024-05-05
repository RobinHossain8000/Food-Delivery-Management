using MVC_Project_FoodOrder.Models;
using MVC_Project_FoodOrder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace MVC_Project_FoodOrder.Controllers
{
    public class GroupingsController : Controller
    {
        private readonly FoodDbContext db = new FoodDbContext();
        // GET: Groupings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GroupingByFoodName()
        {
            var data = db.Orders.Include(x => x.FoodItem).ToList()
               .GroupBy(s => s.FoodItem.FoodName)
               .Select(g => new GroupedData { Key = g.Key, Data = g.Select(x => x) })
               .ToList();
            return View(data);
        }
        public ActionResult GroupingByOrderType()
        {
            var data = db.Orders
                .ToList()
               .GroupBy(s => s.OrderType)
               .Select(g => new GroupedData { Key = g.Key.ToString(), Data = g.Select(x => x) })
               .ToList();
            return View(data);
        }
    }
}