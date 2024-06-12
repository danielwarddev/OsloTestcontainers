using Microsoft.EntityFrameworkCore;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.Api;

public interface IBookService
{
    Task<Book?> GetBook(string name);
    Task LikeBook(int bookId, int userId);
    Task ReviewBook(int bookId, int userId, string reviewContent);
}

public class BookService : IBookService
{
    private readonly BookstoreContext _context;
    
    public BookService(BookstoreContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetBook(string name)
    {
        return await _context.Books.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task LikeBook(int bookId, int userId)
    {
        var existingBookLike = await _context.BookLikes.FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

        if (existingBookLike != null)
        {
            return;
        }

        await _context.BookLikes.AddAsync(new BookLike
        {
            BookId = bookId,
            UserId = userId
        });

        await _context.SaveChangesAsync();
    }

    public async Task ReviewBook(int bookId, int userId, string reviewContent)
    {
        var existingBookReview = await _context.BookReviews.FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

        if (existingBookReview == null)
        {
            await _context.BookReviews.AddAsync(new BookReview
            {
                BookId = bookId,
                UserId = userId,
                ReviewContent = reviewContent
            });
        }
        else
        {
            existingBookReview.ReviewContent = reviewContent;
        }

        await _context.SaveChangesAsync();
    }
}