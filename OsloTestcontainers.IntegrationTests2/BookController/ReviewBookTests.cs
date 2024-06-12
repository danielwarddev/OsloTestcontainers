using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Api.Controllers;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests2.BookController;

public class BookReviewTests : IClassFixture<IntegrationTestFactory>, IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly BookstoreContext _dbContext;
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public BookReviewTests(IntegrationTestFactory factory)
    {
        _dbContext = factory.Db;
        _client = factory.CreateClient();
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task When_BookReview_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var expectedReview = _fixture.Create<BookReview>();

        await _client.PutAsJsonAsync("book/review", new ReviewBookRequest(
            expectedReview.BookId,
            expectedReview.UserId,
            expectedReview.ReviewContent
        ));

        var allBookReviews = _dbContext.BookReviews.ToList();
        allBookReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedReview, 
                options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task When_BookReview_Exists_In_Database_Already_Then_Updates_Review_Content()
    {
        var existingReview = _fixture.Create<BookReview>();
        await _dbContext.AddAsync(existingReview);
        await _dbContext.SaveChangesAsync();

        var updatedReview = existingReview;
        updatedReview.ReviewContent = "This book is totally cool";

        await _client.PutAsJsonAsync("book/review", new ReviewBookRequest(
            updatedReview.BookId,
            updatedReview.UserId,
            updatedReview.ReviewContent
        ));

        var allBookReviews = _dbContext.BookReviews.ToList();
        allBookReviews.Count.Should().Be(1);
        allBookReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(updatedReview,
            options => options.Excluding(x => x.Id));
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}