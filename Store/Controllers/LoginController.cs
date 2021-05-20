using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Shopee1.Models;

namespace Store.Controllers
{
    //[AutoValidateAntiforgeryToken]//此項跟資安有關，只要是http method post，都要驗證token。Startup.cs有設定過全域驗證的話，這行可省略
    public class LoginController : Controller
    {
        private readonly IConfiguration config;
        public LoginController(IConfiguration config)
        {
            this.config = config;
        }
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 表單post提交，準備登入
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string Account, string pd, string ReturnUrl)
        {//未登入者想進入必須登入的頁面，他會被自動導頁至/Home/Login，網址後面也會自動帶上名為ReturnUrl(原始要求網址)的QueryString

            //pd是密碼，記得加密pd變數或雜湊過後再和DB資料比對
            //從自己DB檢查帳&密，輸入是否正確
            if ((Account == "shadow" && pd == "shadow") == false)
            {
                //帳&密不正確
                ViewBag.errMsg = "帳號或密碼輸入錯誤";
                return View();//流程不往下執行
            }

                //string connectionString = "Server=.;Database=Shopee;Trusted_Connection=true;";

                //var sqlString = "SELECT UserId, UserName, UserPhone, UserEmail, UserPassword, UserAddress FROM UserInfo WHERE UserEmail = @useremail AND UserPassword = @userpassword";

                //var UserList = new List<MUserInfo>();

                //using (SqlConnection connect = new SqlConnection(connectionString))
                //{
                //    using (SqlCommand cmd = new SqlCommand(sqlString, connect))
                //    {
                //        connect.Open();

                //        cmd.Parameters.Add("@useremail", SqlDbType.NVarChar).Value = Account;

                //        cmd.Parameters.Add("@userpassword", SqlDbType.NVarChar).Value = pd;

                //        SqlDataReader sqlDataReader = cmd.ExecuteReader();

                //        while (sqlDataReader.Read())
                //        {
                //            var user = new MUserInfo();

                //            //dataReader ["欄位名稱"].ToString()    資料庫的資料
                //            user.UserId = Convert.ToInt32(sqlDataReader["UserId"]);
                //            user.UserName = Convert.ToString(sqlDataReader["UserName"]);
                //            user.UserPhone = Convert.ToString(sqlDataReader["UserPhone"]);
                //            user.UserEmail = Convert.ToString(sqlDataReader["UserEmail"]);
                //            user.UserPassword = Convert.ToString(sqlDataReader["UserPassword"]);
                //            user.UserAddress = Convert.ToString(sqlDataReader["UserAddress"]);
                //            UserList.Add(user);
                //        }
                //        HttpContext.Session.SetString("UserName", UserList[0].UserName);
                //        string Uname = HttpContext.Session.GetString("UserName");
                //        return View();
                //    }
                //}

            //帳密都輸入正確，ASP.net Core要多寫三行程式碼 
            Claim[] claims = new[] { new Claim("Account", Account) }; //Key取名"Account"，在登入後的頁面，讀取登入者的帳號會用得到，自己先記在大腦
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//Scheme必填
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);


            //從組態讀取登入逾時設定
            double loginExpireMinute = this.config.GetValue<double>("LoginExpireMinute");
            //執行登入，相當於以前的FormsAuthentication.SetAuthCookie()
            await HttpContext.SignInAsync(principal,
                new AuthenticationProperties()
                {
                    IsPersistent = false, //IsPersistent = false：瀏覽器關閉立馬登出；IsPersistent = true 就變成常見的Remember Me功能
                                          //用戶頁面停留太久，逾期時間，在此設定的話會覆蓋Startup.cs裡的逾期設定
                                          /* ExpiresUtc = DateTime.UtcNow.AddMinutes(loginExpireMinute) */
                });
            //加上 Url.IsLocalUrl 防止Open Redirect漏洞
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);//導到原始要求網址
            }
            else
            {
                return RedirectToAction("Index", "Products");//到登入後的第一頁，自行決定
            }

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        //登出 Action 記得別加上[Authorize]，不管用戶是否登入，都可以執行Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Login");//導至登入頁
        }
    }
}