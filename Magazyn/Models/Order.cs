using System;
using System.ComponentModel.DataAnnotations;

namespace Magazyn.Models
{
    public class Order
    {
        [Required]
        public int IdOrder {get;set;}
        [Required]
        public int IdProduct {get;set;}
        [Required]
        public int Amount {get;set;}
        public DateTime CreatedAt { get; set; }
        public DateTime FulfilledAt { get; set; }

    }
}
