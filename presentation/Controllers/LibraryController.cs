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
        var details = await _libraryService.GetAllBorrowingsWithDetails();
        return View(details);
    }

    //  For showing current logged in user borrowings with details
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyBorrowings()
    {
        // Get the logged in user Id
        int UserId = int.Parse(User.FindFirst("UserId")!.Value);

        var borrowings = await _libraryService.GetAllBorrowingsWithDetailsByUser(UserId);

        return View(borrowings);
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
        else
        {
            ModelState.AddModelError("", "Please check ISBN, Title and Author fields!");
            return View(bookModel);
        }
    }

    // For admin only
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateBook(string ISBN)
    {
        if (string.IsNullOrWhiteSpace(ISBN))
            return NotFound("Book with this ISBN is not found!");

        UpdatedBookDto updatedBook = new() { ISBN = ISBN };
        return View(updatedBook);
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
        else
        {
            ModelState.AddModelError("", "Please check the Title and Author fields!");
            return View(updatedBook);
        }
    }

    [Authorize]
    public async Task<IActionResult> BorrowBooks(List<string> ISBNs)
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

    [Authorize]
    public async Task<IActionResult> ReturnBooks(List<string> ISBNs)
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
