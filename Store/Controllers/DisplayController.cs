using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Shopee1.Models;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using X.PagedList;
using X.PagedList.Mvc.Common;
using X.PagedList.Mvc.Core;

namespace Shopee1.Controllers
{
    public class DisplayController : Controller
    {
        public IActionResult Index(int page = 1, int sizePage = 8)
        {

            List<ProdMaintain> parts = new List<ProdMaintain>();

            parts = new ProductsController().getActiveProduct();

            ViewBag.Category = getAllCategory().Result
                .Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() })
                .Prepend(new SelectListItem { Text = "請選擇", Value = "-9999" });
            return View((parts.ToPagedList(page, sizePage)));  //(parts.ToPagedList(page, sizePage))
        }

        public async Task<List<MCategory>> getAllCategory()
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"
            SELECT CategoryId, CategoryName FROM dbo.Category 
            WHERE IsEnable = 1
            ";

            var CategoryList = new List<MCategory>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    SqlDataReader sqlDataReader = cmd.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        var category = new MCategory();

                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        category.CategoryId = Convert.ToInt32(sqlDataReader["CategoryId"]);
                        category.CategoryName = Convert.ToString(sqlDataReader["CategoryName"]);
                        //category.IsEnable = Convert.ToString(sqlDataReader["IsEnable"]);
                        CategoryList.Add(category);
                    }

                    return CategoryList;
                }
            }
        }

        public async Task<List<ProdMaintain>> searchByName(string productName, string categoryId)
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"
            SELECT Name, ImgURL, Price FROM dbo.Products 
            WHERE Name LIKE N'%" + productName + "%'";


            if(categoryId != "-9999")
            {
                sqlString += " AND CategoryId = " + categoryId;
            }
            var SearchList = new List<ProdMaintain>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    SqlDataReader sqlDataReader = cmd.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        var products = new ProdMaintain();

                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        products.Name = Convert.ToString(sqlDataReader["Name"]);
                        products.ImgURL = Convert.ToString(sqlDataReader["ImgURL"]);
                        products.Price = Convert.ToString(sqlDataReader["Price"]);
                        SearchList.Add(products);
                    }

                    return SearchList;
                }
            }
        }

        public async Task<List<MShoppingCart>> searchCart()
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"
            SELECT * FROM dbo.ShoppingCart s FULL JOIN dbo.Products p ON s.ProductId = p.Id WHERE s.CartId IS NOT NULL ORDER BY s.ProductId ASC;
            ";

            var CartList = new List<MShoppingCart>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    SqlDataReader sqlDataReader = cmd.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        var cart = new MShoppingCart();

                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        cart.CartId = Convert.ToInt32(sqlDataReader["CartId"]);
                        cart.ProductId = Convert.ToInt32(sqlDataReader["ProductId"]);
                        cart.ProductName = Convert.ToString(sqlDataReader["ProductName"]);
                        cart.CategoryId = Convert.ToString(sqlDataReader["CategoryId"]);
                        cart.CategoryName = Convert.ToString(sqlDataReader["CategoryName"]);
                        cart.Quantity = Convert.ToString(sqlDataReader["Quantity"]);
                        cart.Price = Convert.ToString(sqlDataReader["Price"]);
                        cart.ImgURL = Convert.ToString(sqlDataReader["ImgURL"]);
                        //最後兩欄判斷物品是否供應
                        cart.Status = Convert.ToString(sqlDataReader["Status"]);
                        cart.onUse = Convert.ToString(sqlDataReader["Id"]);
                        CartList.Add(cart);
                    }

                    return CartList;
                }
            }
        }

        //新增功能
        public async Task<string> InsertCart(MShoppingCart cart)
        {
            try
            {
                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                var sqlString = @"INSERT INTO dbo.ShoppingCart(ProductId, ProductName, CategoryId, Quantity, Price, ImgURL) values(@productId, @productName, @categoryId, @quantity, @price, @imgURL);SELECT SCOPE_IDENTITY();";

                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@productId", SqlDbType.Int).Value = cart.ProductId;

                        cmd.Parameters.Add("@productName", SqlDbType.NVarChar).Value = cart.ProductName;

                        cmd.Parameters.Add("@categoryId", SqlDbType.Int).Value = cart.CategoryId;

                        //cmd.Parameters.Add("@categoryName", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(cart.CategoryName) ? DBNull.Value : (object)cart.CategoryName;

                        cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = cart.Quantity;

                        cmd.Parameters.Add("@price", SqlDbType.Int).Value = cart.Price;

                        cmd.Parameters.Add("@imgURL", SqlDbType.NVarChar).Value = cart.ImgURL;

                        //會傳遞一個字串，代表要插入資料表中的新值
                        //int test = Decimal.ToInt32((System.Decimal)cmd.ExecuteScalar());

                        int affectedCount = cmd.ExecuteNonQuery();

                        if (connect.State == System.Data.ConnectionState.Open)
                        {
                            connect.Close();
                        }

                        //if (test != 0)
                        //{
                            return affectedCount.ToString();
                        //}
                        //else
                        //{
                        //    return "failed";
                        //}
                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<string> DeleteCart(int Id)
        {
            try
            {
                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                string sqlString = "DELETE FROM dbo.ShoppingCart where CartId = @id";

                using(SqlConnection connect = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(sqlString, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = Id;

                        //要是沒這一段就不會把欲要改變的數值傳回資料庫
                        int row = cmd.ExecuteNonQuery();

                        if (row > 0)
                        {
                            return "success";
                        }
                        else
                        {
                            return "fail";
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<string> UpdateCart(int Id, int Quantity)
        {
            try
            {
                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                string sqlString = "UPDATE dbo.ShoppingCart SET Quantity = @quantity where CartId = @id";

                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = Id;

                        cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = Quantity;

                        //要是沒這一段就不會把欲要改變的數值傳回資料庫
                        int row = cmd.ExecuteNonQuery();

                        if (row > 0)
                        {
                            return "success";
                        }
                        else
                        {
                            return "fail";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }

}