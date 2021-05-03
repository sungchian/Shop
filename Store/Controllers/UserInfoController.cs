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
using Microsoft.AspNetCore.Session;

namespace Shopee1.Controllers
{
    public class UserInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
        public async Task<string> Login(string userEmail, string userPassword)
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = "SELECT UserId, UserName, UserPhone, UserEmail, UserPassword, UserAddress FROM UserInfo WHERE UserEmail = @useremail AND UserPassword = @userpassword";

            var UserList = new List<MUserInfo>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    cmd.Parameters.Add("@useremail", SqlDbType.NVarChar).Value = userEmail;

                    cmd.Parameters.Add("@userpassword", SqlDbType.NVarChar).Value = userPassword;

                    SqlDataReader sqlDataReader = cmd.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        var user = new MUserInfo();

                        //dataReader ["欄位名稱"].ToString()    資料庫的資料
                        user.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
                        user.UserName = Convert.ToString(sqlDataReader["UserName"]);
                        user.UserPhone = Convert.ToString(sqlDataReader["UserPhone"]);
                        user.UserEmail = Convert.ToString(sqlDataReader["UserEmail"]);
                        user.UserPassword = Convert.ToString(sqlDataReader["UserPassword"]);
                        user.UserAddress = Convert.ToString(sqlDataReader["UserAddress"]);
                        UserList.Add(user);
                    }
                    HttpContext.Session.SetString("UserName", UserList[0].UserName);
                    string Uname = HttpContext.Session.GetString("UserName");
                    return Uname;
                }
            }
        }

        public async Task<bool> SignUp(string userName, string userPhone, string userEmail, string userPassword, string userAddress)
        {
            string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

            var sqlString = "INSERT INTO dbo.UserInfo (UserName, UserPhone, UserEmail, UserPassword, UserAddress) VALUES (@username, @userphone, @useremail, @userpassword, @useraddress);";

            var UserList = new List<MUserInfo>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                {
                    connect.Open();

                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = userName;

                    cmd.Parameters.Add("@userphone", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(userPhone) ? DBNull.Value : (object)userPhone;

                    cmd.Parameters.Add("@useremail", SqlDbType.NVarChar).Value = userEmail;

                    cmd.Parameters.Add("@userpassword", SqlDbType.NVarChar).Value = userPassword;

                    cmd.Parameters.Add("@useraddress", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(userAddress) ? DBNull.Value : (object)userAddress;

                    int row = cmd.ExecuteNonQuery();

                    if(row > 0)
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


    }
}