using Microsoft.AspNetCore.Mvc;
using ProductMaster.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using NetTopologySuite.Mathematics;

namespace ProductMaster.Controllers
{
    public class ProductDBContext : Controller
    {

        string cs = @"Data Source=SURYA\SQLEXPRESS;Initial Catalog=Suresh;Integrated Security=True";

        public List<ProductModel> GetProducts()
        {

            List<ProductModel> pmlist = new List<ProductModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SelectAllProduct", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    try
                    {
                        ProductModel pm = new ProductModel();

                        pm.Product_Id = dr.GetInt32(0);
                        pm.Product_Code = dr.GetString(1);
                        pm.Name = dr.GetString(2);
                        pm.Description = dr.GetString(3);
                        pm.ExpiryDate = dr.GetDateTime(4).ToString();
                        pm.Category = dr.GetString(5);

                        if (dr["Image"] is byte[])
                        {
                            byte[] bytes = (byte[])dr["Image"];
                            pm.base64Image = Convert.ToBase64String(bytes);
                        }
                        var status = dr.GetInt32(7);
                        pm.Status = status == 1 ? "Active" : "Inactive";
                        pm.CreationDate = dr.GetDateTime(8);

                        pmlist.Add(pm);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return pmlist;
        }

        internal bool IsCodeAvailable(string product_Code)
        {
            var res = GetProducts().FirstOrDefault(p => p.Product_Code.Equals(product_Code, StringComparison.OrdinalIgnoreCase));
            return res != null;
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }


        public void AddProduct(ProductModel pm)
        {
            SqlConnection con = new SqlConnection(cs);


            var status = pm.Status.Equals("Active", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            SqlCommand cmd = new SqlCommand("AddProduct", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //SqlCommand cmd = new SqlCommand(@$"insert into Product values('{pm.Product_Code}', '{pm.Name}', '{pm.Description}', '{pm.ExpiryDate:yyyy-MM-dd}','{pm.Category}',@image_byte_array,{status},'{pm.CreationDate:yyyy-MM-dd}')", con);
            cmd.Parameters.Add(new SqlParameter("@Product_Code", SqlDbType.VarChar)
            {
                Value = pm.Product_Code
            });

            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar)
            {
                Value = pm.Name
            });
            cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar)
            {
                Value = pm.Description
            });

            cmd.Parameters.Add(new SqlParameter("@ExpiryDate", SqlDbType.Date)
            {
                Value = $"{pm.ExpiryDate:yyyy-MM-dd}"
            });

            cmd.Parameters.Add(new SqlParameter("@Category", SqlDbType.VarChar)
            {
                Value = pm.Category
            });

            cmd.Parameters.Add(new SqlParameter("@Image", SqlDbType.VarBinary)
            {
                Direction = ParameterDirection.Input,
                Size = (int)pm.ProfilePic.Length,
                Value = GetByteArrayFromImage(pm.ProfilePic)
            });

            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar)
            {
                Value = status
            });

            cmd.Parameters.Add(new SqlParameter("@CreationDate", SqlDbType.Date)
            {
                Value = $"{pm.CreationDate:yyyy-MM-dd}"
            });

            con.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            con.Close();


        }

        public void UpdateProduct(ProductModel pm)
        {
            SqlConnection con = new SqlConnection(cs);
            //string strquery = @$"CREATE PROCEDURE UpdateProduct
            //                  AS
            //                  BEGIN
            //                  UPDATE Product
            //                  SET Product_Code = {pm.Product_Code}, Name = '{pm.Name}', Description = '{pm.Description}', Expiry_Date = '{pm.Expiry_Date}', Category = '{pm.Category}', Image = '{pm.Image}', Status = {pm.Status}, Creation = '{pm.Creation}'

            //                  WHERE {pm.Product_Id}
            //                  END";

            SqlCommand cmd = new SqlCommand("UpdateProduct", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }




















    }
}
