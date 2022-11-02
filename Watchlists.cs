using Asp.Versioning.Conventions;
using CapitalBroker.ViewModels;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CapitalBroker
{
    public static class Watchlists
    {
        public static IEndpointRouteBuilder MapWatchListsEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            routes.MapGet("/Watchlists", GetAllWatchlists).Produces(200);
            routes.MapPost("/CreateWatchlist", CreateWatchlist)
                .Accepts<NewWatchlist>("application/json")
                .Produces(200).Produces(400);
            routes.MapGet("/SingleWatchList/{watchlistId}", SingleWatchlist).Produces(200).Produces(404);
            routes.MapPut("/AddMarketToWatchlist/{watchlistId}", AddMarketToWatchlist)
                .Accepts<string>("application/json")
                .Produces(200).Produces(404);
            routes.MapDelete("/Watchlist/{watchlistId}", DeleteWatchlist).Produces(200).Produces(404);
            routes.MapDelete("/Watchlist/{watchlistId}/{epic}", RemoveMarketFromWatchlist).Produces(200).Produces(404);

            return routes;
        }

        public static async Task<IResult> GetAllWatchlists()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> CreateWatchlist(NewWatchlist watchlist)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists", Method.Post);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            string body = JsonSerializer.Serialize(watchlist);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }
            return Results.Ok(output);
        }

        public static async Task<IResult> SingleWatchlist(string watchlistId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists/{watchlistId}", Method.Get);
            request.AddUrlSegment("watchlistId", watchlistId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(response.Content);
            }
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> AddMarketToWatchlist(string watchlistId, string epic)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists/{watchlistId}", Method.Put);
            request.AddUrlSegment("watchlistId", watchlistId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            JsonObject obj = new JsonObject();
            obj.Add("epic", epic);
            string body = JsonSerializer.Serialize(obj);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(output);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(output);
            }
            return Results.Ok(output);
        }

        public static async Task<IResult> DeleteWatchlist(string watchlistId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists/{watchlistId}", Method.Delete);
            request.AddUrlSegment("watchlistId", watchlistId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(output);
            }
            return Results.Ok(output);
        }

        // Working Orders
        public static async Task<IResult> RemoveMarketFromWatchlist(string watchlistId, string epic)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/watchlists/{watchlistId}/{epic}", Method.Delete);
            request.AddUrlSegment("watchlistId", watchlistId);
            request.AddUrlSegment("epic", epic);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(output);
            }
            return Results.Ok(output);
        }
    }
}
