using MVC_Project_FoodOrder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project_FoodOrder.ViewModels
{
    public class FoodInputModel
    {
        public int FoodItemId { get; set; }
        [Required, StringLength(50), Display(Name = "FoodItem name")]

        public string FoodName { get; set; }
        [Required, Column(TypeName = "money"), DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]

        public decimal UnitPrice { get; set; }
        public bool Available { get; set; }
        [Required]

        public HttpPostedFileBase Picture { get; set; }
        [Required]

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public List<Order> Orders { get; set; } =new List<Order>();
    }
}