using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace apiEcom
{
	public class Vinyl
	{
        public int VinylID { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter an artist")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "Please enter a genre")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Please enter the condition")]
        public string Condition { get; set; }

        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

