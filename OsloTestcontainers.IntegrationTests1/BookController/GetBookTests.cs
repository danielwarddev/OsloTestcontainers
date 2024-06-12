using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests1.BookController;

public class GetBookTests : IClassFixture<IntegrationTestFactory>
{
    private readonly Fixture _fixture = new();
    private readonly BookstoreContext _dbContext;
    private readonly HttpClient _client;

    public GetBookTests(IntegrationTestFactory factory)
    {
        _dbContext = factory.Db;
        _client = factory.CreateClient();
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
}