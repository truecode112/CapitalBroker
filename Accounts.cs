using Asp.Versioning.Conventions;
using CapitalBroker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace CapitalBroker
{
    public static class Accounts
    {
        public static IEndpointRouteBuilder MapAccountsEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            //routes.MapGet("/GetEncryptionKey", GetEncryptionKey).Produces(200).WithMetadata(new RequiresAPIKeyAttribute());
            routes.MapGet("/GetAllAccounts", GetAllAccounts).Produces(200);
            routes.MapGet("/GetAccountPreferences", GetAccountPreferences).Produces(200);
            routes.MapPut("/UpdateAccountPreferences", UpdateAccountPreferences)
                .Accepts<AccountPreference>("application/json")
                .Produces(200).Produces(400);
            routes.MapGet("/GetAccountActivityHistory", GetAccountActivityHistory).Produces(200).Produces(400);
            routes.MapGet("/GetAccountTransactionHistory", GetAccountTransactionHistory).Produces(200);

            return routes;
        }

        public static async Task<IResult> GetAllAccounts()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/accounts", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetAccountPreferences()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/accounts/preferences", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> UpdateAccountPreferences(AccountPreference pref)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/accounts/preferences", Method.Put);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            request.AddHeader("Content-Type", "application/json");
            string body = JsonSerializer.Serialize(pref);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }

            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetAccountActivityHistory([FromQuery(Name = "from")] string from = "",
            [FromQuery(Name = "to")] string to = "", [FromQuery(Name = "lastPeriod")] int lastPeriod = 600,
            [FromQuery(Name = "detailed")] bool detailed = true, [FromQuery(Name = "dealId")] string dealId = "" ,
            [FromQuery(Name = "filter")] string filter = "" )
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/history/activity", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            if(!string.IsNullOrEmpty(from))
                request.AddQueryParameter("from", from, false);
            if (!string.IsNullOrEmpty(to))
                request.AddQueryParameter("to", to, false);
            request.AddQueryParameter("lastPeriod", lastPeriod, false);
            request.AddQueryParameter("detailed", detailed, false);
            if (!string.IsNullOrEmpty(dealId))
                request.AddQueryParameter("dealId", dealId, false);
            if (!string.IsNullOrEmpty(filter))
                request.AddQueryParameter("filter", filter, false);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> GetAccountTransactionHistory([FromQuery(Name = "from")] string from = "",
            [FromQuery(Name = "to")] string to = "", [FromQuery(Name = "lastPeriod")] int lastPeriod = 600,
            [FromQuery(Name = "type")] string type = "")
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/history/transactions", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            if (!string.IsNullOrEmpty(from))
                request.AddQueryParameter("from", from, false);
            if (!string.IsNullOrEmpty(to))
                request.AddQueryParameter("to", to, false);
            request.AddParameter("lastPeriod", lastPeriod, false);
            if (!string.IsNullOrEmpty(type))
                request.AddQueryParameter("dealId", type, false);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }
    }
}
