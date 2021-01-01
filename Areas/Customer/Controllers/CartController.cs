using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using YemekSiparisWeb.Models;
using YemekSiparisWeb.Models.ViewModels;
using YemekSiparisWeb.Utility;
using Stripe;

namespace YemekSiparisWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;//email gondermk icin emailsender clasını kullanıcaz.asagıda summary post metodunda.

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db,IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            detailCart = new OrderDetailsCart()
            {
               OrderHeader=new Models.OrderHeader()
            };
            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claims.Value);//kullanıcının sepete ekledigi urunleri bulduk.
            if (cart!=null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(x => x.Id == list.MenuItemId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);

                if (list.MenuItem.Description.Length>100)//acıklama kısmının ilk 100 karakterini gormek istedik.
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;//total original indirimler falan olursa en son hali tutuyor.

            if (HttpContext.Session.GetString(SD.ssCouponCode)!=null)//adcoupon metodunda session olusturduk.onu kontrol ediyoruz.kupon varsa:
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(x => x.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }


            return View(detailCart);
        }

        public async Task<IActionResult> Summary()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser=await _db.ApplicationUser.Where(x => x.Id == claims.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCart.Where(x => x.ApplicationUserId == claims.Value);//kullanıcının sepete ekledigi urunleri bulduk.
            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(x => x.Id == list.MenuItemId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

            }

            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;//total original indirimler falan olursa en son hali tutuyor.
            detailCart.OrderHeader.PickUpName = applicationUser.Name;
            detailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailCart.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)//adcoupon metodunda session olusturduk.onu kontrol ediyoruz.kupon varsa:
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(x => x.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }


            return View(detailCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {//sumary viewinde odeme icin kart bilgielrini girip butona basarsak tüm bilgiler post edilip stripe serverine gidiyor.
            //oradan token olarak parametremize geliyor.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailCart.listCart = await _db.ShoppingCart.Where(x => x.ApplicationUserId == claims.Value).ToListAsync();
            //orderheaderı olusturduk yani siparisi
            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claims.Value;
            detailCart.OrderHeader.Status = SD.PaymentStatusPending;
            detailCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailCart.OrderHeader.PickUpDate.ToShortDateString()+" "+detailCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(detailCart.OrderHeader);//olusan order headerı databaseye ekledik.
            await _db.SaveChangesAsync();

            detailCart.OrderHeader.OrderTotalOriginal = 0;


            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(x => x.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails//order headerdan sonra order detailsi olusturuyoruz.
                {
                    MenuItemId=item.MenuItemId,
                    OrderId=detailCart.OrderHeader.Id,//2tablo iliskili
                    Description=item.MenuItem.Description,
                    Name=item.MenuItem.Name,
                    Price=item.MenuItem.Price,
                    Count=item.Count
                };
                detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);
            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)//adcoupon metodunda session olusturduk.onu kontrol ediyoruz.kupon varsa:
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(x => x.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotalOriginal;//kupon yoksa indirim yok yani olan neyse o fiyat
            }
            detailCart.OrderHeader.CouponCodeDiscount = detailCart.OrderHeader.OrderTotalOriginal - detailCart.OrderHeader.OrderTotal;
            await _db.SaveChangesAsync();
            _db.ShoppingCart.RemoveRange(detailCart.listCart);//siparıs olustu.sepeti silebiliriz.
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions//odeme işlemi icin stripe kütüphanesini kullanıyoruz.
            {
                Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),//100le carpmamız gerekiyor.
                Currency="usd",
                Description="Order ID:"+detailCart.OrderHeader.Id,
                Source=stripeToken//parametreden gelen token
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);//olusturdugumuz optionu service veriyoruz.
            if (charge.BalanceTransactionId==null)//eger bir hata olduysa;
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower()=="succeeded")
            {
                //email for succesful order
                await _emailSender.SendEmailAsync(
                    _db.Users.Where(u => u.Id == claims.Value).FirstOrDefault().Email,
                    "Spice - Order Created"+detailCart.OrderHeader.Id.ToString(),
                    "Order has been submitted succesfully"
                    );

                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                detailCart.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            await _db.SaveChangesAsync();//kredi kartı icin sag üstte testmode yazıyor.ordan test numara alıp deneyince api hesabımıza siparis geliyor.
            //dashboarddan siparis tutarı gorunuyor.databaseye eklendi.

            return RedirectToAction("Confirm","Order",new { id=detailCart.OrderHeader.Id});
        }
        public async Task<IActionResult> AddCoupon()
        {
            if (detailCart.OrderHeader.CouponCode==null)
            {
                detailCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailCart.OrderHeader.CouponCode);

            return RedirectToAction("Index");

        }
        public async Task<IActionResult> RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode,string.Empty);//kupon girdik o yüzden bos.

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            if (cart.Count==1)//eger 1 urun kaldıysa;
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                //sessionu urunu kaldırdıgımız icin tekrar guncelledik.
                var cnt = _db.ShoppingCart.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            //sessionu urunu kaldırdıgımız icin tekrar guncelledik.
            var cnt = _db.ShoppingCart.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
 
            return RedirectToAction("Index");
        }

    }
}
