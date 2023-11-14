using AutoMapper;
using BookManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace BookManage.Controllers
{
    public class CartController : Controller
    {
        

        
        public IActionResult ViewCart() {
             String ? a = HttpContext.Session.GetString("cart");
            Dictionary<int, Book> cart = new Dictionary<int, Book>();
            if (a != null)
            {
                cart = JsonConvert.DeserializeObject<Dictionary<int, Book>>(a);
            }
            

            return View(cart.Values);
        }

        public IActionResult Add(int id)
        {
            using (var conn = new BookManagementContext())
            {
                Dictionary<int, Book> cart = new Dictionary<int, Book>();
                String? a = HttpContext.Session.GetString("cart");
                if (a != null)
                {
                    cart = JsonConvert.DeserializeObject<Dictionary<int, Book>>(a);

                }
                if (cart.ContainsKey(id))
                {
                    cart[id].Quantity += 1;
                }
                else
                {
                    var book = conn.Books.FirstOrDefault(x => x.Id == id);
                    book.Quantity = 1;
                    cart.Add(id, book);
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));

                return RedirectToAction("Index", "Home", new { name = "", mess = "add cart success" });
            }
                
            

        }

        public IActionResult update(int id,int type)
        {
            Dictionary<int, Book> cart = new Dictionary<int, Book>();
            String? a = HttpContext.Session.GetString("cart");
            if (a != null) cart = JsonConvert.DeserializeObject<Dictionary<int, Book>>(a);

            if(type == 1)
            {
                cart[id].Quantity += 1;

            }
            else
            {
                cart[id].Quantity -= 1;
                if (cart[id].Quantity == 0) cart.Remove(id);
            }
            
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart));
            return RedirectToAction("ViewCart");
        }


        public IActionResult payment(string email, string address)
        {
            BookManagementContext conn = new BookManagementContext();
            decimal total = 0;
            String? a = HttpContext.Session.GetString("cart");
            var cart = JsonConvert.DeserializeObject<Dictionary<int, Book>>(a);

            string mes = "Not enough quantity: ";
            bool check = false;
            foreach(var item in cart)
            {
                var book = conn.Books.FirstOrDefault(x => x.Id == item.Value.Id);
                total += book.Price * item.Value.Quantity;
                if (item.Value.Quantity > book.Quantity)
                {
                    mes += book.Name + ", ";
                    check = true;
                }
            }

            if (check)
            {
                ViewBag.mess =mes;
                return View("ViewCart",cart.Values);
            }
            else
            {
                Account user = new Account();
                if (HttpContext.Session.GetString("account") != null)
                {
                    user = JsonConvert.DeserializeObject<Account>(HttpContext.Session.GetString("account"));
                    Order order = new Order()
                    {
                        CustomerId = user.Id,
                        CustomerName = user.Username,
                        BuyDate = DateTime.Now,
                        Address = address,
                        Email = email,
                        Phone = user.Phone,
                        Status = "Pending",
                        Total = total,
                        UserId = user.Id,
                    };
                    conn.Add(order);
                    conn.SaveChanges();

                    int id = order.Id;
                    foreach (var item in cart)
                    {
                        var book = conn.Books.FirstOrDefault(x => x.Id == item.Value.Id);
                        book.Quantity -= item.Value.Quantity;

                        OrderDetail detail = new OrderDetail()
                        {
                            BookId = book.Id,
                            OrderId = id,
                            Price = item.Value.Price,
                            Quantity = item.Value.Quantity,
                        };
                        conn.Add(detail);
                    }
                    conn.SaveChanges();
                    HttpContext.Session.Remove("cart");
                    ViewBag.mess = "Order Confirm Success!";
                    cart.Clear();
                    return View("ViewCart", cart.Values);
                }
                else
                {
                    ViewBag.mess = "You haven't logged in. Please login! ";
                    return View("ViewCart", cart.Values);
                }   

            }


            return View();
        }

        

    }
}
