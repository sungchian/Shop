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
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
namespace Shopee1.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index(int page = 1, int sizePage = 8)
        {

            List<ProdMaintain> parts = new List<ProdMaintain>();

            parts = new ProductsController().getAllProduct();

            return View(parts.ToPagedList(page, sizePage));
        }

        public List<ProdMaintain> getAllProduct()
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"SELECT * FROM dbo.Products";

            var ProductList = new List<ProdMaintain>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    SqlDataReader test = cmd.ExecuteReader();

                    while (test.Read())
                    {
                        var prod = new ProdMaintain();
                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        prod.Id = Convert.ToInt32(test["Id"]);
                        prod.Name = Convert.ToString(test["Name"]);
                        prod.Description = Convert.ToString(test["Description"]);
                        prod.CategoryId = Convert.ToString(test["CategoryId"]);
                        prod.Price = Convert.ToString(test["Price"]);
                        prod.Quantity = Convert.ToString(test["Quantity"]);
                        prod.Status = Convert.ToString(test["Status"]);
                        prod.ImgURL = Convert.ToString(test["ImgURL"]);
                        ProductList.Add(prod);

                    }

                    return ProductList;
                }
            }
        }

        public List<ProdMaintain> getActiveProduct()
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"SELECT * FROM dbo.Products WHERE Status = 1";

            var ProductList = new List<ProdMaintain>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    SqlDataReader test = cmd.ExecuteReader();

                    while (test.Read())
                    {
                        var prod = new ProdMaintain();
                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        prod.Id = Convert.ToInt32(test["Id"]);
                        prod.Name = Convert.ToString(test["Name"]);
                        prod.Description = Convert.ToString(test["Description"]);
                        prod.CategoryId = Convert.ToString(test["CategoryId"]);
                        prod.Price = Convert.ToString(test["Price"]);
                        prod.Quantity = Convert.ToString(test["Quantity"]);
                        prod.Status = Convert.ToString(test["Status"]);
                        prod.ImgURL = Convert.ToString(test["ImgURL"]);
                        ProductList.Add(prod);

                    }

                    return ProductList;
                }
            }
        }

        //找尋by id
        //[AllowAnonymous]        
        public async Task<IActionResult> QueryProdById(int id)
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = @"
                SELECT * FROM dbo.Products WHERE Id = @id";

            using(SqlConnection connect = new SqlConnection(connectionString))
            {
                using(SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader test = cmd.ExecuteReader();

                    var maintainProd = new ProdMaintain();

                    while (test.Read())
                    {
                        maintainProd.Id = Convert.ToInt32(test["Id"]);
                        maintainProd.Name = Convert.ToString(test["Name"]);
                        maintainProd.Description = Convert.ToString(test["Description"]);
                        maintainProd.CategoryId = Convert.ToString(test["CategoryId"]);
                        maintainProd.Price = Convert.ToString(test["Price"]);
                        maintainProd.PublishDate = Convert.ToString(test["PublishDate"]);
                        maintainProd.Status = Convert.ToString(test["Status"]);
                        maintainProd.ImgURL = Convert.ToString(test["ImgURL"]);
                        maintainProd.Quantity = Convert.ToString(test["Quantity"]);
                        
                    }
                    return Json(maintainProd);
                }
            }
        }

        //刪除功能
        public async Task<string> DeleteProd(int Id)
        {
            try
            {
                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                var sqlString = @"DELETE FROM dbo.Products WHERE Id = @id";

                using(SqlConnection connect = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(sqlString, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = Id;

                        int row = cmd.ExecuteNonQuery();

                        if(row > 0)
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

        //新增功能
        //[Authorize]
        [HttpPost]
        public async Task<string> InsertProd(ProdMaintain prod)
        {
            try
            {
                string imgUrl = "no.jpg";

                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                var sqlString = @"INSERT INTO dbo.Products(Name, Description, CategoryId, Price, PublishDate, Status, ImgURL, Quantity) values(@name, @description, @categoryId, @price, @publishDate, @status, @imgURL, @quantity);SELECT SCOPE_IDENTITY();";

                using(SqlConnection connect = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(sqlString, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = prod.Name;

                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(prod.Description) ? DBNull.Value : (object)prod.Description;

                        cmd.Parameters.Add("@categoryId", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(prod.CategoryId) ? DBNull.Value : (object)prod.CategoryId;

                        cmd.Parameters.Add("@price", SqlDbType.Int).Value = prod.Price;

                        cmd.Parameters.Add("@publishDate", SqlDbType.NVarChar).Value = prod.PublishDate;

                        cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(prod.Status) ? DBNull.Value : (object)prod.Status;

                        cmd.Parameters.Add("@imgURL", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(prod.ImgURL) ? imgUrl : (object)prod.ImgURL;

                        cmd.Parameters.Add("@quantity", SqlDbType.NVarChar).Value = prod.Quantity;

                        //會傳遞一個字串，代表要插入資料表中的新值
                        int test = Decimal.ToInt32((System.Decimal)cmd.ExecuteScalar());

                        if (connect.State == System.Data.ConnectionState.Open)
                        {
                            connect.Close();
                        }

                        if (test != 0)
                        {
                            return "success";
                        }
                        else
                        {
                            return "failed";
                        }
                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                    }
                }
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }

        //編輯功能
        public async Task<bool> UpdateProd(int Id, string name, string description, string categoryId, string price, string publishDate, string status, string imgUrl, string quantity)
        {
            try
            {
                string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";
                //if (String.IsNullOrEmpty(imgUrl))
                //{
                //    imgUrl = "no.jpg";
                //}
                //string connectionString = "Server=skmh.database.windows.net,1433;Initial Catalog=SKMH-LINE-BOT-DB;Persist Security Info=False;User ID=skmh_admin;Password=1qaz@wsx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var sqlStr = "UPDATE Products SET Name = @name, Description = @description, CategoryId = @categoryId, Price = @price, PublishDate = @publishDate, Status = @status, ImgURL = @imgURL, Quantity = @quantity WHERE Id = @Id ";
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlStr, connect))
                    {
                        connect.Open();

                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(name) ? DBNull.Value : (object)name;
                        cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description;
                        cmd.Parameters.Add("@categoryId", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(categoryId) ? DBNull.Value : (object)categoryId;
                        cmd.Parameters.Add("@price", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(price) ? DBNull.Value : (object)price;
                        cmd.Parameters.Add("@publishDate", SqlDbType.DateTime).Value = string.IsNullOrEmpty(publishDate) ? DBNull.Value : (object)publishDate;
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(status) ? DBNull.Value : (object)status;
                        cmd.Parameters.Add("@imgURL", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(imgUrl) ? DBNull.Value : (object)imgUrl;
                        cmd.Parameters.Add("@quantity", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(quantity) ? DBNull.Value : (object)quantity;
                        int row = cmd.ExecuteNonQuery();
                        if (row > 0)

                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //上傳檔案
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> upload)
        {
            if (upload == null || upload.Count == 0)
            {
                return Ok(new { IsOk = false, ErrorText = "請選擇圖片!" });
            }
            try
            {
                var fileNameList = new List<string>();
                foreach (var formFile in upload)
                {
                    if (formFile.Length > 0)
                    {
                        Random crandom = new Random();

                        int ram = crandom.Next(10000);

                        //var imagename = DateTime.Now.ToString("yyyyMMddhhmmss") + ram.ToString() + "." + formFile.FileName.Split(".")[formFile.FileName.Split(".").Length - 1];
                        var imagename = "rpt_Img" + ((DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Substring(0, 19).Replace(" ", "_")).Replace("/", "")).Replace(":", "") + "_" + ram.ToString() + ".jpg";

                        var filePath = @"C:\Users\A015953.SKHCORP\source\repos\Store\Store\wwwroot\images\" + imagename;

                        var filePathCopy = @"C:\Users\A015953.SKHCORP\source\repos\Store\Store\wwwroot\orignImages\" + imagename;

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        System.IO.File.Copy(filePath, filePathCopy, false);
                        fileNameList.Add(filePath);
                        //fileNameList.Add(filePathCopy);
                    }
                }
                return Ok(new { IsOk = true, fileNames = fileNameList });
            }
            catch (Exception ex)
            {
                return Ok(new { IsOk = false, ErrorText = ex.ToString() });
            }
        }
        //Update Image 上傳
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> upload)
        {
            if (upload == null || upload.Count == 0)
            {
                return Ok(new { IsOk = false, ErrorText = "請選擇圖片!" });
            }
            try
            {
                var fileNameList = new List<string>();
                foreach (var formFile in upload)
                {
                    if (formFile.Length > 0)
                    {
                        Random crandom = new Random();

                        int ram = crandom.Next(10000);

                        //var imagename = DateTime.Now.ToString("yyyyMMddhhmmss") + ram.ToString() + "." + formFile.FileName.Split(".")[formFile.FileName.Split(".").Length - 1];
                        var imagename = "UPD_" + ((DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Substring(0, 19).Replace(" ", "_")).Replace("/", "")).Replace(":", "") + "_" + ram.ToString() + ".jpg";

                        var filePath = @"C:\Users\A015953.SKHCORP\source\repos\Store\Store\wwwroot\images\" + imagename;

                        var filePathCopy = @"C:\Users\A015953.SKHCORP\source\repos\Store\Store\wwwroot\updateImages\" + imagename;

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        System.IO.File.Copy(filePath, filePathCopy, false);
                        fileNameList.Add(filePath);
                        //fileNameList.Add(filePathCopy);
                    }
                }
                return Ok(new { IsOk = true, fileNames = fileNameList });
            }
            catch (Exception ex)
            {
                return Ok(new { IsOk = false, ErrorText = ex.ToString() });
            }
        }
    }
}