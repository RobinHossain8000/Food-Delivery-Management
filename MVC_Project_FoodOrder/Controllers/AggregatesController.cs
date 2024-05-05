using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_Project_FoodOrder.Models;
namespace MVC_Project_FoodOrder.Controllers
{
    public class AggregatesController : Controller
    {
        private readonly FoodDbContext db = new FoodDbContext();
        // GET: Aggregates
        public ActionResult Index()
        {
            var data =db.FoodItems.Include(x=>x.Orders).ToList();   
            return View(data);
        }
    }
}