using Asp.Versioning.Conventions;
using CapitalBroker.ViewModels;
using RestSharp;
using RestSharp.Serializers;
using System.Net.Mime;
using System.Text.Json;

namespace CapitalBroker
{
    public static class Trading
    {
        public static IEndpointRouteBuilder MapTradingEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            routes.MapGet("/GetPosOrderInfo/{dealReference}", GetPosOrderInfo).Produces(200).Produces(StatusCodes.Status404NotFound);

            //Positions
            routes.MapGet("/AllPositions", GetAllPositions).Produces(200);
            routes.MapPost("/CreatePosition", CreatePosition)
                .Accepts<NewTradingPosition>("application/json")
                .Produces(200).Produces(400);
            routes.MapGet("/SinglePosition/{dealId}", SinglePosition).Produces(200).Produces(404);
            routes.MapPut("/UpdatePosition/{dealId}", UpdatePosition)
                .Accepts<UpdateTradingPosition>("application/json")
                .Produces(200).Produces(400).Produces(404);
            routes.MapDelete("/ClosePosition/{dealId}", ClosePosition).Produces(200).Produces(404);

            //Orders
            routes.MapGet("/AllWorkingOrders", GetAllWorkingOrders).Produces(200);
            routes.MapPost("/CreateWorkingOrder", CreateWorkingOrder)
                .Accepts<NewWorkingOrder>("application/json")
                .Produces(200).Produces(400);
            routes.MapPut("/UpdateWorkingOrder/{dealId}", UpdateWorkingOrder)
                .Accepts<UpdateTradingWorkingOrder>("application/json")
                .Produces(200).Produces(404);
            routes.MapDelete("/DeleteWorkingOrder/{dealId}", DeleteWorkingOrder).Produces(200).Produces(404);
            return routes;
        }

        public static async Task<IResult> GetPosOrderInfo(string dealReference)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/confirms/{dealReference}", Method.Get);
            request.AddUrlSegment("dealReference", dealReference);
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

        // Positions
        public static async Task<IResult> GetAllPositions()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/positions", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> CreatePosition(NewTradingPosition position)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/positions", Method.Post);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            string body = JsonSerializer.Serialize(position);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }
            return Results.Ok(output);
        }

        public static async Task<IResult> SinglePosition(string dealId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/positions/{dealId}", Method.Get);
            request.AddUrlSegment("dealId", dealId);
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

        public static async Task<IResult> UpdatePosition(string dealId, UpdateTradingPosition position)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/positions/{dealId}", Method.Put);
            request.AddUrlSegment("dealId", dealId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            string body = JsonSerializer.Serialize(position);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(output);
            } else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(output);
            }
            return Results.Ok(output);
        }

        public static async Task<IResult> ClosePosition(string dealId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/positions/{dealId}", Method.Delete);
            request.AddUrlSegment("dealId", dealId);
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
        public static async Task<IResult> GetAllWorkingOrders()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/workingorders", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> CreateWorkingOrder(NewWorkingOrder order)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/workingorders", Method.Post);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            string body = JsonSerializer.Serialize(order);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }
            return Results.Ok(output);
        }

        public static async Task<IResult> UpdateWorkingOrder(string dealId, UpdateTradingWorkingOrder order)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/workingorders/{dealId}", Method.Put);
            request.AddUrlSegment("dealId", dealId);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            string body = JsonSerializer.Serialize(order);
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

        public static async Task<IResult> DeleteWorkingOrder(string dealId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/workingorders/{dealId}", Method.Delete);
            request.AddUrlSegment("dealId", dealId);
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
