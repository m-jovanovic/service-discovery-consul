using Carter;

namespace Newsletter.Api.Articles;

public class GetReportingArticle
{
    public sealed class Client(HttpClient httpClient)
    {
        public async Task<Response?> GetAsync(Guid id)
        {
            var response = await httpClient.GetFromJsonAsync<Response>($"api/articles/{id}");

            return response;
        }
    }

    public class Response
    {
        public Guid Id { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? PublishedOnUtc { get; set; }

        public List<ArticleEventResponse> Events { get; set; } = new();
    }

    public class ArticleEventResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public int EventType { get; set; }
    }
}

public class GetReportingArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/articles/{id}/reporting", async (Guid id, GetReportingArticle.Client client) =>
        {
            var article = await client.GetAsync(id);

            if (article is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(article);
        });
    }
}
