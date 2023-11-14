using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManage.Models;
using Microsoft.Extensions.Hosting;

namespace BookManage.Controllers
{
    public class BooksController : Controller
    {
       

        // GET: Books
        public async Task<IActionResult> Index()
        {
            BookManagementContext context = new BookManagementContext();
            var bookManagementContext = context.Books.Include(b => b.Category);
            return View(await bookManagementContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            BookManagementContext context = new BookManagementContext();
            if (id == null || context.Books == null)
            {
                return NotFound();
            }

            var book = await context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            BookManagementContext context = new BookManagementContext();
            ViewData["CategoryName"] = new SelectList(context.Categories, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,Author,CategoryId,Status")] Book book)
        {
            BookManagementContext context = new BookManagementContext();
            if (!ModelState.IsValid)
            {
                var imageUrl = Request.Form.Files.GetFile("imageURL");
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    // Save the uploaded file to the "images" folder inside the wwwroot directory
                    var uploadsFolder = Path.Combine("wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageUrl.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageUrl.CopyToAsync(stream);
                    }

                    // Update the Book.Image property with the file name
                    book.Image = "Images/" + uniqueFileName;
                }
                context.Add(book);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryName"] = new SelectList(context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            BookManagementContext context = new BookManagementContext();
            if (id == null || context.Books == null)
            {
                return NotFound();
            }

            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryName"] = new SelectList(context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,Author,CategoryId,Status,Image")] Book book,string defaultImage)
        {
            BookManagementContext context = new BookManagementContext();
            
            if (id != book.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try

                {
                    var imageUrl = Request.Form.Files.GetFile("imageUrl");
                    if (imageUrl != null && imageUrl.Length > 0)
                    {
                        // Save the uploaded file to the "images" folder inside the wwwroot directory
                        var uploadsFolder = Path.Combine("wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageUrl.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageUrl.CopyToAsync(stream);
                        }

                        // Update the Book.Image property with the file name
                        book.Image = "Images/"+uniqueFileName;
                    }
                    else {
                       book.Image=defaultImage;
                        
                    }
                    context.Update(book);
                    await context.SaveChangesAsync();
                } 
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryName"] = new SelectList(context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            BookManagementContext context = new BookManagementContext();
            if (context.Books == null)
            {
                return Problem("Entity set 'BookManagementContext.Books'  is null.");
            }
            var book = await context.Books.FindAsync(id);
            if (book != null)
            {
                context.Books.Remove(book);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Books/Delete/5
        

        private bool BookExists(int id)
        {
            using (var context = new BookManagementContext())
            {
                return (context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
            }
                
            
        }
    }
}
