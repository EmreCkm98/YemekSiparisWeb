using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using YemekSiparisWeb.Services;
using Microsoft.AspNetCore.Mvc;
using YemekSiparisWeb.Utility;
using Stripe;

namespace YemekSiparisWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //database icin.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            ////projeyi indivitual authentication olarak actýk.otamatik kendi olusturdu.
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IDbInitializer,DbInitializer>();//baslangýc icin admin olusturmustuk o clasý cagýrdýk.configure metoduna gecicez.
            //asagýdaki configure metodunda stripe kutuphanesini kullanýcaz.
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));//stripesettings clasýný belirttik.section ismi appjson dosyasýna ayný isimle yazmýstýk.
            services.AddSingleton<IEmailSender, EmailSender>();//emailsender clasý icin.asagýda configure servicesinde kullanabilmmek icin!!!
            services.Configure<EmailOptions>(Configuration);//yazdýgýmýz clasý kullanmak icin.                   
            services.AddRazorPages();

            services.AddControllersWithViews();//controller icin
            services.ConfigureApplicationCookie(options =>

            {

                options.LoginPath = $"/Identity/Account/Login";

                options.LogoutPath = $"/Identity/Account/Logout";

                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

            });

            //services.AddAuthentication().AddFacebook(facebookOptions=> { //facebookla giriþ icin developer facebook hesabýmýzdan gelen api idyi girdik.
            //    facebookOptions.AppId = "358686958698218";
            //    facebookOptions.AppSecret = "e85fe11c8336fa69a11af014a4fff0ed";//burasý developer facebook hesabýmýzdaki settings basic kýsmýndan api key ve api secret olarak gorunuyor.
            //    //daah sonra url icin calýsan sitemizde urli kopyalayýp (https://localhost:44363/)facebook developer sayfamýzdan dashboarda geliyoz
            //    //facebook login sonra webi seciyoruz.site url kýsmýna yapýstýrýyoz.save diyoruz.sol menuden products kýsmýndan settingse geliyoruz.
            //    //valid oauth redirrecrt url kýsmýnada urlimizi yapýstýrýyoz/signin-facebook ekliyoruz sonuna.save changes diyoruz.artýk projemizde
            //    //login sayfasýnda facebook butonu var.týklayýnca facebook hesabýyla giris yapýyoruz.identity-acoount-externallogin.cshtml viewwini
            //    //donuyor url.bu viewi ve register viewini biraz güzelleþtiricez.
            //});

            //homecontroller details post metodunda sepet icin session kullandýk.session kullanabilmek icin bunu eklicez.
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly=true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IDbInitializer dbInitializer)//dbintializer biz ekledik admin rolü icin.
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);//secrectkey appjsondan geliyor.summary viewwinde kullanýcaz.
            dbInitializer.Initialize();//clasýmýzý cagýrmýs olduk.
            
            app.UseSession();//session kullanmak icin..
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");//routea area yýda ekledik{}ile
                endpoints.MapRazorPages();
            });
        }
    }
}
