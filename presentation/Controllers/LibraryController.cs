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

    public LibraryController(LibraryService libraryService, AccountService accountSerivce)
    {
        _libraryService = libraryService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> AllBooks(string searchTerm, bool availableOnly, List<string> ISBNs)
    {
        if (ISBNs.Count() > 0)
        {
            int UserId = int.Parse(User.FindFirst("UserId")!.Value);
            var borrowResult = await _libraryService.BorrowBooks(ISBNs, UserId);
            if (!borrowResult.Result)
                return Content($"Could not borrow books because: {borrowResult.Message}");
        }

        var books = await _libraryService.SearchBooksByISBNOrTitleOrAuthor(searchTerm, availableOnly);
        return View(books);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> AllBorrowings()
    {
        var details = await _libraryService.GetAllBorrowingsWithDetails();
        return View(details);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyBorrowings()
    {
        int UserId = int.Parse(User.FindFirst("UserId")!.Value);
        var borrowings = await _libraryService.GetAllBorrowingsWithDetailsByUser(UserId);

        return View(borrowings);
    }

    [Authorize(Policy = "AdminOnly")]
    public IActionResult AddBook()
    {
        BookDto bookModel = new();
        return View(bookModel);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> AddBook(BookDto bookModel)
    {
        if (ModelState.IsValid)
        {
            var addResult = await _libraryService.AddNewBook(bookModel);
            if (addResult.Result)
            {
                return RedirectToAction("Index", "Library");
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

    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateBook(string ISBN)
    {
        if (string.IsNullOrWhiteSpace(ISBN))
            return NotFound("Book with this ISBN is not found!");

        UpdatedBookDto updatedBook = new() { ISBN = ISBN };
        return View(updatedBook);
    }

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
                return RedirectToAction("Index", "Library");
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
    public async Task<IActionResult> ReturnBooks(List<string> ISBNs)
    {
        var returnResult = await _libraryService.ReturnBooks(ISBNs);
        if (!returnResult.Result)
            return Content($"Could not return books because: {returnResult.Message}");

        return RedirectToAction("MyBorrowings");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
