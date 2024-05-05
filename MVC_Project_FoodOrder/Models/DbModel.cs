using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_Project_FoodOrder.Models
{
    public enum OrderType { DineIn = 1, TakeAway }
    public class Category
    {
        public int CategoryId { get; set; }
        [Required, StringLength(50), Display(Name = "Category name")]

        public string CategoryName { get; set; }     
        public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();   
    }
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        [Required, StringLength(50), Display(Name = "FoodItem name")]

        public string FoodName { get; set; }
        [Required, Column(TypeName = "money"), DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]

        public decimal UnitPrice { get; set; }
        public bool Available { get; set; }
        [Required, StringLength(50)]

        public string Picture { get; set; }
        [Required, ForeignKey("Category")]

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
    public class Order
    {
        public int OrderId { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "Order date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime OrderDate { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]

        public OrderType OrderType { get; set; }
        [Required, ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }    
        public virtual FoodItem FoodItem { get; set; }
    }
    public class FoodDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }

    }

}