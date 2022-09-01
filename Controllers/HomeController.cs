using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProductMaster.Models;

namespace ProductMaster.Controllers
{
    public class HomeController : Controller
    {

        [Route("~/")]
        public IActionResult Index()
        {
            ProductDBContext db = new ProductDBContext();
            List<ProductModel> obj = db.GetProducts();
            return View(obj);
        }



        [Route("Create")]
        public IActionResult Create()
        {
            ProductModel pm = new ProductModel();
            return View(pm);
        }


        [HttpPost]
        [Route("Create")]
        public ActionResult Create(ProductModel pm)
        {
            try
            {
                pm.base64Image = "";
                pm.CreationDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    ProductDBContext context = new ProductDBContext();
                    if (context.IsCodeAvailable(pm.Product_Code))
                    {
                        ModelState.AddModelError(nameof(ProductModel.Product_Code), $"Product code {pm.Product_Code} already available, Please enter new code");
                        return View();
                    }

                    if (pm.Name.Length > 250)
                    {
                        ModelState.AddModelError(nameof(ProductModel.Name), $"Only allowed 250 character for name");
                        return View();
                    }

                    context.AddProduct(pm);
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        [Route("UpdateRedirect")]
        public IActionResult UpdateRedirect(ProductModel pm)
        {
            return View("Update", pm);
            //return View();    
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult Update(ProductModel pm)
        {
            ProductDBContext context = new ProductDBContext();
            context.UpdateProduct(pm);
            return RedirectToAction("Index");
        }
    }
}
