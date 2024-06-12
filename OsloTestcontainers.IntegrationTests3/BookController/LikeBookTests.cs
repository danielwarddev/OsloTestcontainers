using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using OsloTestcontainers.Api.Controllers;
using OsloTestcontainers.Database;

namespace OsloTestcontainers.IntegrationTests3.BookController;

[CollectionDefinition(nameof(DatabaseTestCollection2))]
public class DatabaseTestCollection2 : ICollectionFixture<IntegrationTestFactory>;

[Collection(nameof(DatabaseTestCollection2))]
public class LikeBookTests : DatabaseTest
{
    private readonly Fixture _fixture = new();

    public LikeBookTests(IntegrationTestFactory factory) : base(factory) {}

    [Fact]
    public async Task When_BookLike_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var expectedLike = _fixture.Create<BookLike>();
        
        await Client.PutAsJsonAsync("book/like", new LikeBookRequest
        {
            BookId = expectedLike.BookId,
            UserId = expectedLike.UserId
        });

        var allBookLikes = DbContext.BookLikes.ToList();
        allBookLikes.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedLike, 
                options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task When_BookLike_Exists_In_Database_Already_Then_Does_Nothing()
    {
        var existingBookLike = _fixture.Create<BookLike>();
        await Insert(existingBookLike);

        await Client.PutAsJsonAsync("book/like", new LikeBookRequest
        {
            BookId = existingBookLike.BookId,
            UserId = existingBookLike.UserId
        });

        var allBookLikes = DbContext.BookLikes.ToList();
        allBookLikes.Should().ContainSingle();
    }
}