using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests2.BookController;

public class GetBookTests : IClassFixture<IntegrationTestFactory>, IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly BookstoreContext _dbContext;
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public GetBookTests(IntegrationTestFactory factory)
    {
        _dbContext = factory.Db;
        _client = factory.CreateClient();
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task When_Book_Exists_Then_Returns_It()
    {
        var existingBook = _fixture.Create<Book>();
        await _dbContext.AddAsync(existingBook);
        await _dbContext.SaveChangesAsync();
        
        var book = await _client.GetFromJsonAsync<Book>($"book/{existingBook.Name}");

        book.Should().NotBeNull();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}