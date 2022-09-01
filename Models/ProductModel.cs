using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductMaster.Models
{
    public class ProductModel
    {
        [Required]
        public int Product_Id { get; set; }
        [Required]
        public string Product_Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        public string Category { get; set; }

        [Required]
        public IFormFile ProfilePic { get; set; }

        [Required]
        public string Status { get; set; }

        public List<string> CatList = new List<string> { "Category A", "Category B", "Category C", "Category D", "Category E" };

        //[Required]
        //public IFormFile ProfilePic { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime CreationDate { get; set; }
        public string base64Image { get; set; }
    }

    //public class UserModelExtend : ProductModel
    //{

    //}
}
