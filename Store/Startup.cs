using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Store
{
    public class Startup
    {
        private readonly IConfiguration config;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //從組態讀取登入逾時設定
            //double loginExpireMinute = Configuration.GetValue<double>("LoginExpireMinute");
            ////註冊 CookieAuthentication，Scheme必填
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            //{
            //    //或許要從組態檔讀取，自己斟酌決定
            //    option.LoginPath = new PathString("/Login/Login");//登入頁
            //    option.LogoutPath = new PathString("/Login/Logout");//登出Action
            //                                                       //用戶頁面停留太久，登入逾期，或Controller中用戶登入時也可設定
            //    option.ExpireTimeSpan = TimeSpan.FromMinutes(loginExpireMinute);//沒給預設14天
            //                                                                    //↓資安建議false，白箱弱掃軟體會要求cookie不能延展效期，這時設false變成絕對逾期時間
            //                                                                    //↓如果你的客戶反應明明一直在使用系統卻容易被自動登出的話，你再設為true(然後弱掃policy請客戶略過此項檢查) 
            //    option.SlidingExpiration = false;
            //});

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddMvc(options => {
            //    //↓和CSRF資安有關，這裡就加入全域驗證範圍Filter的話，待會Controller不必再加上[AutoValidateAntiforgeryToken]屬性
            //    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //留意先執行驗證...
            app.UseAuthentication();
            //再執行Route，如此順序程式邏輯才正確
            app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
