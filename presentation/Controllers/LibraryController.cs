using System.Diagnostics;
using business_logic.DTOs;
using business_logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presentation.Models;

namespace presentation.Controllers;

public class LibraryController : Controller
{
    private readonly LibraryService _libraryService;

    public LibraryController(LibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> AllBooks(string searchTerm, bool availableOnly)
    {
        // For searching books
        var books = await _libraryService.SearchBooksByISBNOrTitleOrAuthor(searchTerm, availableOnly);
        return View(books);
    }

    // For admin only
    // Show all users borrowings with details
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> AllBorrowings()
    {
        try
        {
            var details = await _libraryService.GetAllBorrowingsWithDetails();
            return View(details);
        }
        catch
        {
            TempData["Notification"] = "Server error, Try again later";
            TempData["NotificationType"] = "error";
            return RedirectToAction("Logout", "Account");
        }
    }

    //  For showing current logged in user borrowings with details
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyBorrowings()
    {
        // Get the logged in user Id
        int UserId = int.Parse(User.FindFirst("UserId")!.Value);

        try
        {
            var borrowings = await _libraryService.GetAllBorrowingsWithDetailsByUser(UserId);

            return View(borrowings);
        }
        catch
        {
            TempData["Notification"] = "Server error, Try again later";
            TempData["NotificationType"] = "error";
            return RedirectToAction("Logout", "Account");
        }
    }

    // For admin only
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AddBook()
    {
        BookDto bookModel = new();
        return View(bookModel);
    }

    // For admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> AddBook(BookDto bookModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var addResult = await _libraryService.AddNewBook(bookModel);
                if (addResult.Result)
                {
                    TempData["Notification"] = "Book added successfully!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("AllBooks", "Library");
                }
                else
                {
                    ModelState.AddModelError("", addResult.Message!);
                    return View(bookModel);
                }
            }
            catch
            {
                TempData["Notification"] = "Server error, Try again later";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Logout", "Account");
            }
        }
        else
        {
            ModelState.AddModelError("", "Please check ISBN, Title and Author fields!");
            return View(bookModel);
        }
    }

    // For admin only
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateBookAsync(string ISBN)
    {
        if (string.IsNullOrWhiteSpace(ISBN))
        {
            TempData["Notification"] = "Book with this ISBN is not found";
            TempData["NotificationType"] = "error";
            return RedirectToAction("AllBooks", "Library");
        }
        else
        {
            try
            {
                var book = await _libraryService.GetBookByISBN(ISBN);
                if (book is null)
                {
                    TempData["Notification"] = "Book with this ISBN is not found";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("AllBooks", "Library");
                }

                UpdatedBookDto updatedBook = new() { ISBN = ISBN, Author = book.Author, Title = book.Title };
                return View(updatedBook);
            }
            catch
            {
                TempData["Notification"] = "Server error, Try again later";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Logout", "Account");
            }
        }
    }

    // For admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> UpdateBook(UpdatedBookDto updatedBook)
    {
        if (string.IsNullOrWhiteSpace(updatedBook.ISBN))
            return NotFound("Book with this ISBN is not found!");

        if (ModelState.IsValid)
        {
            try
            {
                var updateResult = await _libraryService.UpdateBookInfo(updatedBook);
                if (updateResult.Result)
                {
                    TempData["Notification"] = "Book updated successfully!";
                    TempData["NotificationType"] = "success";
                    return RedirectToAction("AllBooks", "Library");
                }
                else
                {
                    ModelState.AddModelError("", updateResult.Message!);
                    return View(updatedBook);
                }
            }
            catch
            {
                TempData["Notification"] = "Server error, Try again later";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Logout", "Account");
            }
        }
        else
        {
            ModelState.AddModelError("", "Please check the Title and Author fields!");
            return View(updatedBook);
        }
    }

    [Authorize]
    public async Task<IActionResult> BorrowBooks(List<string> ISBNs)
    {
        try
        {
            // If list of books were sent for borrowing
            if (ISBNs.Count() > 0)
            {
                // Get the current logged in user Id
                int UserId = int.Parse(User.FindFirst("UserId")!.Value);

                var borrowResult = await _libraryService.BorrowBooks(ISBNs, UserId);
                if (!borrowResult.Result)
                {
                    TempData["Notification"] = $"Could not borrow books: {borrowResult.Message}";
                    TempData["NotificationType"] = "error";
                }
                else
                {
                    TempData["Notification"] = "Books borrowed successfully!";
                    TempData["NotificationType"] = "success";
                }
            }
            return RedirectToAction("AllBooks", "Library");
        }
        catch
        {
            TempData["Notification"] = "Server error, Try again later";
            TempData["NotificationType"] = "error";
            return RedirectToAction("Logout", "Account");
        }
    }

    [Authorize]
    public async Task<IActionResult> ReturnBooks(List<string> ISBNs)
    {
        try
        {
            var returnResult = await _libraryService.ReturnBooks(ISBNs);
            if (!returnResult.Result)
            {
                TempData["Notification"] = $"Could not return books because: {returnResult.Message}";
                TempData["NotificationType"] = "error";
            }
            else
            {
                TempData["Notification"] = "Books returned successfully!";
                TempData["NotificationType"] = "success";
            }

            return RedirectToAction("MyBorrowings");
        }
        catch
        {
            TempData["Notification"] = "Server error, Try again later";
            TempData["NotificationType"] = "error";
            return RedirectToAction("Logout", "Account");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
