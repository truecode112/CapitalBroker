using Asp.Versioning.Conventions;
using CapitalBroker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;

namespace CapitalBroker
{
    public static class MarketsInfo
    {
        public static IEndpointRouteBuilder MapMarketsInfoEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            //Markets
            routes.MapGet("/AllTopLevelMarketCategories", GetAllTopLevelMarketCategories).Produces(200);
            routes.MapGet("/AllCategorySubNodes/{nodeId}", GetCategorySubNodes).Produces(200);
            routes.MapGet("/MarketsDetails", GetMarketsDetails).Produces(200).Produces(400);
            routes.MapGet("/GetSingleMarketDetails/{epic}", GetSingleMarketDetails)
                .Produces(200).Produces(404);

            //Prices
            routes.MapGet("/GetHistoricalPrices/{epic}", GetHistoricalPrices)
                .Produces(200).Produces(400);

            //Client Sentiment
            routes.MapGet("/GetClientSentimentsForMarkets", GetClientSentimentForMarkets)
                .Produces(200).Produces(404);
            routes.MapGet("/GetClientSentimentForMarket/{marketId}", GetClientSentimentForMarket)
                .Produces(200).Produces(404);
            return routes;
        }

        // Markets
        public static async Task<IResult> GetAllTopLevelMarketCategories()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/marketnavigation", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetCategorySubNodes(string nodeId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/marketnavigation/{nodeId}", Method.Get);
            request.AddUrlSegment("nodeId", nodeId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetMarketsDetails([FromQuery(Name ="searchTerm")] string searchTerm,
            [FromQuery(Name ="epics")] string epics)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/markets", Method.Get);
            request.AddQueryParameter("searchTerm", searchTerm);
            request.AddQueryParameter("epics", epics);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetSingleMarketDetails(string epic)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/markets/{epic}", Method.Get);
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

        //Historical Prices
        public static async Task<IResult> GetHistoricalPrices(
            string epic,
            [FromQuery(Name="resolution")] string resolution = "",
            [FromQuery(Name = "max")] int max = 10,
            [FromQuery(Name = "from")] string from= "",
            [FromQuery(Name = "to")] string to = "")
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/prices/{epic}", Method.Get);
            request.AddUrlSegment("epic", epic);
            if (!string.IsNullOrEmpty(resolution))
                request.AddQueryParameter("resolution", resolution);
            request.AddQueryParameter("max", max);
            if (!string.IsNullOrEmpty(from))
                request.AddQueryParameter("from", from);
            if (!string.IsNullOrEmpty(to))
                request.AddQueryParameter("to", to);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(output);
            }
            return Results.Ok(output);
        }

        // Client sentiment
        public static async Task<IResult> GetClientSentimentForMarkets([FromQuery(Name ="marketIds")] string marketIds)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/clientsentiment", Method.Get);
            request.AddQueryParameter("marketIds", marketIds);
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

        public static async Task<IResult> GetClientSentimentForMarket(string marketId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/clientsentiment/{marketId}", Method.Get);
            request.AddUrlSegment("marketId", marketId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(response.Content);
            }
            return Results.Ok(output);
        }
    }
}
