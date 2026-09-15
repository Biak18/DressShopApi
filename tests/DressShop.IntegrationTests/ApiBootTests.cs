using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DressShop.IntegrationTests;

// No /health endpoint in this project by design (Supabase/Postgres,
// connection string comes from user-secrets). This test only verifies
// the API boots and serves its OpenAPI document.
public class ApiBootTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task OpenApiDocument_IsServed()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
