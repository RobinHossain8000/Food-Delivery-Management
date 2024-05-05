using MVC_Project_FoodOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Project_FoodOrder.ViewModels
{
    public class GroupedData
    {
        public string Key { get; set; }
        public IEnumerable<Order> Data { get; set; } = new List<Order>();
    }
}