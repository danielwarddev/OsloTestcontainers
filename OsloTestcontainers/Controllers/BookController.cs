using Microsoft.AspNetCore.Mvc;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.Api.Controllers;

public class LikeBookRequest
{
    public int BookId { get; set; }
    public int UserId { get; set; }
}
public record ReviewBookRequest(int BookId, int UserId, string ReviewContent);

[ApiController]
[Route("[controller]")]
public class BookController
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("{name}")]
    public async Task<Book?> GetBook([FromRoute] string name)
    {
        return await _bookService.GetBook(name);
    }

    [HttpPut("like")]
    public async Task LikeBook([FromBody] LikeBookRequest request)
    {
        await _bookService.LikeBook(request.BookId, request.UserId);
    }

    [HttpPut("review")]
    public async Task ReviewBook([FromBody] ReviewBookRequest request)
    {
        await _bookService.ReviewBook(request.BookId, request.UserId, request.ReviewContent);
    }
}