using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests3.BookController;

[CollectionDefinition(nameof(DatabaseTestCollection))]
public class DatabaseTestCollection : ICollectionFixture<IntegrationTestFactory>;

[Collection(nameof(DatabaseTestCollection))]
public class GetBookTests : DatabaseTest
{
    private readonly Fixture _fixture = new();
    
    public GetBookTests(IntegrationTestFactory factory) : base(factory) {}

    [Fact]
    public async Task When_Book_Exists_Then_Returns_It()
    {
        var existingBook = _fixture.Create<Book>();
        await Insert(existingBook);
        
        var book = await Client.GetFromJsonAsync<Book>($"book/{existingBook.Name}");

        book.Should().NotBeNull();
    }
}