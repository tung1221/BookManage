using BookManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookManage.Controllers
{
    public class OrderController : Controller
    {
        
        public IActionResult Dashboard(string mess) 
        {
            using (var conn = new BookManagementContext()) {
                if (HttpContext.Session.GetString("account") == null)
                {
                    return RedirectToAction("Notfound");
                }
                var listOrder = conn.Orders.Where(o => o.Status.Equals("Pending")).ToList();

                ViewBag.mess = mess;
                return View(listOrder);
            }
            
        }

        public IActionResult ListOrder(string mess)
        {
            using (var conn = new BookManagementContext())
            {
                var listOrder = conn.Orders.ToList();
                ViewBag.mess = mess;
                return View(listOrder);
            }
                
        }

        public IActionResult Index()
        {

            using (var conn = new BookManagementContext()) {
                if (HttpContext.Session.GetString("account") == null)
                {
                    return RedirectToAction("Notfound");
                }
                Account user = JsonConvert.DeserializeObject<Account>(HttpContext.Session.GetString("account"));


                var orders = conn.Orders.Where(x => x.UserId == user.Id).ToList();
                return View(orders);
            }
        }


        public IActionResult Details(int id)
        {
            BookManagementContext conn = new BookManagementContext();
            if (HttpContext.Session.GetString("account") == null)
            {
                return RedirectToAction("Notfound");
            }else
            {
                var orderDetails = conn.OrderDetails.Include(x => x.Book).Where(x => x.OrderId == id).ToList();
                return View(orderDetails);
            }
            
           
        }

        public IActionResult Edit(int id,string status)
        {
            if (HttpContext.Session.GetString("account") == null)
            {
                return RedirectToAction("Notfound");
            }
            using (var conn = new BookManagementContext())
            {
                Order order = conn.Orders.Where(o => o.Id == id).FirstOrDefault();
                if(order != null)
                {
                    order.Status = status;
                    conn.Orders.Update(order);
                    conn.SaveChanges();
                    ViewBag.mess = $"order {order.Id} has been {status}";
                }
                return RedirectToAction("Dashboard", new {mess = $"order {order.Id} has been {status}" });
            }
        }
        public IActionResult Update(int id)
        {
            if (HttpContext.Session.GetString("account") == null)
            {
                return RedirectToAction("Notfound");
            }
            using (var conn = new BookManagementContext())
            {
                Order order = conn.Orders.Where(o => o.Id == id).FirstOrDefault();
                if (order != null)
                {
                    switch (order.Status)
                    {
                        case "confirmed":
                            order.Status = "Waiting";
                            break;
                        case "Waiting":
                            order.Status = "Completed";
                            break;
                    }
                    conn.Orders.Update(order);
                    conn.SaveChanges() ;
                }
                return RedirectToAction("ListOrder", new { mess = $"order {order.Id} has been updated" });
            }
        }
        public IActionResult Delete(int id) 
        {
            using (var conn = new BookManagementContext())
            {
                Order order = conn.Orders.Where(o => o.Id == id).FirstOrDefault();
                if (order != null)
                {
                    if (!order.Status.Equals("rejected"))
                    {
                        return RedirectToAction("ListOrder", new { mess = $"order {order.Id} can not deleted" });
                    }
                    var orderDetails = conn.OrderDetails.Include(x => x.Book).Where(x => x.OrderId == id).ToList();
                    foreach(var item in orderDetails)
                    {
                        conn.OrderDetails.Remove(item);
                    }
                    conn.Orders.Remove(order);
                    conn.SaveChanges();
                }
                return RedirectToAction("ListOrder", new { mess = $"order {order.Id} has been deleted" });
            }
        }

        public IActionResult Notfound()
        {
            return View();
        }

        
    }
}
