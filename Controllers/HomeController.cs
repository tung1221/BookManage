using BookManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookManage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string name, string mess)
        {          
            using (var conn = new BookManagementContext())
            {
                if (name == null) name = "";
                var st = conn.Books.Include(x => x.Category).Where(x => x.Name.Contains(name) && x.Status == true).ToList();
                if (mess != null) ViewBag.mess = mess;
                return View(st);
            }

        }


        public IActionResult Login(String mess)
        {
            if (mess != null) ViewBag.Message = mess;
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            Account acc = checkLogin(username, password);
            string message;
            if(acc!= null)
            {
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(acc));
                message = "Login successfull";
                if(acc.RoleId == 1)
                {
                    
                    return RedirectToAction("Dashboard", "Order", new {mess = message});
                }
                return RedirectToAction("Index", new {name ="", mess = message});
            }
            else
            {
                message = "User name or password is wrong. Try again";
                return RedirectToAction("Login", new { mess = message });
            }

            
        }

        //check login
        private Account checkLogin(string user, string pass)
        {
            using (var conn = new BookManagementContext() )
            {
                Account acc = conn.Accounts.Where(a => a.Username == user && a.Password == pass).FirstOrDefault();

                if (acc != null) { return acc; }
                return null;
            }
                 
            
        }


        public IActionResult CreateAccount(string mess) 
        {
            if (mess != null) ViewBag.Message = mess;
            return View(); 
        }
        [HttpPost]
        public IActionResult CreateAccount(Account account)
        {
            using (var conn = new BookManagementContext())
            {
                Account acc = conn.Accounts.Where(x => x.Username.Equals(account.Username)).FirstOrDefault();
                if (acc == null)
                {
                    account.RoleId = 2;

                    conn.Accounts.Add(account);
                    conn.SaveChanges();
                    return RedirectToAction("Login", new { mess = "create account successfully" });
                }
                return RedirectToAction("CreateAccount", new { mess = "user name is existed" });
            }
                
            
        }
        //logout
        public IActionResult Logout(string mess)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
