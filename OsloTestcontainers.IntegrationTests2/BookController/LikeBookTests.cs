using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Api.Controllers;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests2.BookController;

public class LikeBookTests : IClassFixture<IntegrationTestFactory>, IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly BookstoreContext _dbContext;
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public LikeBookTests(IntegrationTestFactory factory)
    {
        _dbContext = factory.Db;
        _client = factory.CreateClient();
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task When_BookLike_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var expectedLike = _fixture.Create<BookLike>();
        
        await _client.PutAsJsonAsync("book/like", new LikeBookRequest
        {
            BookId = expectedLike.BookId,
            UserId = expectedLike.UserId
        });

        var allBookLikes = _dbContext.BookLikes.ToList();
        allBookLikes.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedLike, 
                options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task When_BookLike_Exists_In_Database_Already_Then_Does_Nothing()
    {
        var existingBookLike = _fixture.Create<BookLike>();
        await _dbContext.AddAsync(existingBookLike);
        await _dbContext.SaveChangesAsync();

        await _client.PutAsJsonAsync("book/like", new LikeBookRequest
        {
            BookId = existingBookLike.BookId,
            UserId = existingBookLike.UserId
        });

        var allBookLikes = _dbContext.BookLikes.ToList();
        allBookLikes.Should().ContainSingle();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}