using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace apiEcom
{
	public class Order
	{
		
		public int OrderID { get; set; }

		
		public int VinylID { get; set; }

		
		public int Quantity { get; set; }

		
		public string CustomerEmail { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public double? PurchasePrice { get; set; }

		[Required]
		[AllowedValues("Processing", "Shipped", "Delivered", ErrorMessage = "Must enter valid order status")]
		public string? OrderStatus { get; set; }
	}

    public class CreateOrder
    {
        [Required]
        public int VinylID { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        public string CustomerEmail { get; set; }
    }

}

