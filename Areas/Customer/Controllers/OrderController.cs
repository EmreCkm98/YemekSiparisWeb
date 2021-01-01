using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekSiparisWeb.Data;
using YemekSiparisWeb.Models;
using YemekSiparisWeb.Models.ViewModels;
using YemekSiparisWeb.Utility;

namespace YemekSiparisWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private int pageSize = 2;

        public OrderController(ApplicationDbContext db,IEmailSender emailSender)
        {
            _db=db;
            _emailSender = emailSender;
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);//login olmus kullanıcının idsini aldık.

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                //tüm kullanıcılardan orderheader idsi parametrede gelen order idye esit ve user idsi login olan kullanıcı idsine esit olan kayıtı bulduk.
                OrderHeader = await _db.OrderHeader.Include(o => o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claims.Value),
                OrderDetails=await _db.OrderDetails.Where(o=>o.OrderId==id).ToListAsync()//order header idsi parametreden gelen order header idsi esit olan
            };
            return View(orderDetailsViewModel);

        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderlistVM = new OrderListViewModel
            {
                Orders=new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.UserId == claims.Value).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderlistVM.Orders.Add(individual);
            }

            var count = orderlistVM.Orders.Count;
            orderlistVM.Orders = orderlistVM.Orders.OrderByDescending(p => p.OrderHeader.Id).Skip((productPage - 1) * pageSize).Take(pageSize).ToList();
            orderlistVM.PagingInfo = new PagingInfo
            {
                CurrentPage=productPage,
                ItemsPerPage=pageSize,
                TotalItem=count,
                UrlParam="/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderlistVM);
        }

        [Authorize(Roles =SD.KitchenUser+","+SD.ManagerUser)]
        public async Task<IActionResult> ManageOrder(int productPage=1)
        {
            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();


            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Where(o=>o.Status==SD.StatusSubmitted||o.Status==SD.StatusInProcess).
                OrderByDescending(o=>o.PickUpTime).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }
            

            return View(orderDetailsVM.OrderBy(o=>o.OrderHeader.PickUpTime).ToList());
        }


        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderHeader = await _db.OrderHeader.Where(m => m.Id == Id).FirstOrDefaultAsync(),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync
                (u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderDetails",orderDetailsViewModel);
        }

        public IActionResult GetOrderStatus(int Id)
        {
            return PartialView("_OrderStatus",_db.OrderHeader.Where(m=>m.Id==Id).FirstOrDefault().Status);
        }

        [Authorize(Roles =SD.KitchenUser+","+SD.ManagerUser)]
        public async Task<IActionResult>OrderPrepare(int OrderId)//maangeorder viewindeki asp-route-OrderId taghelperından geliyor son ksıım aynı OLMALI!
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder","Order");
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)//maangeorder viewindeki asp-route-OrderId taghelperından geliyor son ksıım aynı OLMALI!
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusReady;

            await _db.SaveChangesAsync();

            //email logic to notify user that order is ready for pickup
            await _emailSender.SendEmailAsync(
            _db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email,
            "Spice - Order Ready For Pickup" + orderHeader.Id.ToString(),
            "Order is ready for pickup."
            );

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)//maangeorder viewindeki asp-route-OrderId taghelperından geliyor son ksıım aynı OLMALI!
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(OrderId);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(
                _db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email,
                "Spice - Order Canceled" + orderHeader.Id.ToString(),
                "Order has been canceled succesfully"
                );
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickup(int productPage = 1,string searchName=null, string searchPhone = null, string searchEmail = null)
        {//parametrelerdeki searchler orderpickup viewindeki arama inputları icin.

            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderlistVM = new OrderListViewModel
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName!=null)
            {
                param.Append(searchName);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();
            if (searchName!=null||searchPhone!=null|| searchEmail != null)//eger inputlara hethangi birsey girildiyse orderheaderlsite gore filtreliyoz;
            {
                var user = new ApplicationUser();

                if (searchName!=null)
                {
                    OrderHeaderList = await _db.OrderHeader.Include(u => u.ApplicationUser).
                        Where(u => u.PickUpName.ToLower().Contains(searchName.ToLower())).
                        OrderByDescending(o => o.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        OrderHeaderList = await _db.OrderHeader.Include(u => u.ApplicationUser).
                            Where(u=>u.UserId==user.Id).
                            OrderByDescending(o => o.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = await _db.OrderHeader.Include(u => u.ApplicationUser).
                                Where(u => u.PhoneNumber.Contains(searchPhone)).
                                OrderByDescending(o => o.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {         
            //hazır olan ürünleri buluyoruz.statusu ready oalnları
            OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderlistVM.Orders.Add(individual);
            }


            var count = orderlistVM.Orders.Count;
            orderlistVM.Orders = orderlistVM.Orders.OrderByDescending(p => p.OrderHeader.Id).Skip((productPage - 1) * pageSize).Take(pageSize).ToList();
            orderlistVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = pageSize,
                TotalItem = count,
                UrlParam = param.ToString()
            };

            return View(orderlistVM);
        }

        [Authorize(Roles =SD.FrontDeskUser+","+SD.ManagerUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult>OrderPickupPost(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(
            _db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email,
            "Yemek - Order Completed" + orderHeader.Id.ToString(),
            "Order has been completed succesfully"
            );
            return RedirectToAction("OrderPickup", "Order");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
