using Asp.Versioning.Conventions;
using CapitalBroker.Metadatas;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CapitalBroker
{
    public static class Session
    {
        public static IEndpointRouteBuilder MapSessionEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            //routes.MapGet("/GetEncryptionKey", GetEncryptionKey).Produces(200).WithMetadata(new RequiresAPIKeyAttribute());
            routes.MapGet("/GetEncryptionKey", GetEncryptionKey).Produces(200);
            routes.MapGet("/SessionDetails", GetSessionDetails).Produces(200);
            routes.MapPost("/CreateNewSession", CreateNewSession).Produces(200).Produces(400).Produces(401);
            routes.MapPut("/SwitchActiveAccount", SwitchActiveAccount).Produces(200).Produces(400);
            routes.MapDelete("/LogoutCurrentSession", LogoutCurrentSession).Produces(200);

            return routes;
        }

        public static async Task<IResult> GetEncryptionKey()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/session/encryptionKey", Method.Get);
            request.AddHeader("X-CAP-API-KEY", "3y2Ck6iMJlr3eqQ6");
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;

            //Utils.encryptPassword(output["encryptionKey"], long.Parse(output["timeStamp"]), "ghfdukqhs20191126");

            return Results.Ok(output);
        }
        
        public static async Task<IResult> CreateNewSession()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/session", Method.Post);
            request.AddHeader("X-CAP-API-KEY", "3y2Ck6iMJlr3eqQ6");
            request.AddHeader("Content-Type", "application/json");
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("identifier", "truecoder112@gmail.com");
            jsonObj.Add("password", "!!!qwerTYUIOP111");
            string body = JsonSerializer.Serialize(jsonObj);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Results.Unauthorized();
            }
            if (response.Headers != null)
            {
                IEnumerable<HeaderParameter> query = response.Headers.Where(h => (h.Name != null && h.Name.Equals("X-SECURITY-TOKEN")));
                if (query.Count() > 0)
                {
                    HeaderParameter hp = query.ElementAt(0);
                    Global.SECURITY_TOKEN = (hp.Value == null ? "" : hp.Value.ToString());
                }

                query = response.Headers.Where(h => (h.Name != null && h.Name.Equals("CST")));
                if (query.Count() > 0)
                {
                    HeaderParameter hp = query.ElementAt(0);
                    Global.CST_TOKEN = (hp.Value == null ? "" : hp.Value.ToString());
                }
            }
            
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> SwitchActiveAccount([FromHeader(Name = "accountId")][Required] string accountId)
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/session", Method.Put);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            request.AddHeader("Content-Type", "application/json");
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("accountId", accountId);
            string body = JsonSerializer.Serialize(jsonObj);
            request.AddStringBody(body, "application/json");
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Results.BadRequest(response.Content);
            }
            
            var output = response.Content;
            return Results.Ok(output);
        }

        public static async Task<IResult> LogoutCurrentSession()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/session", Method.Delete);
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

        public static async Task<IResult> GetSessionDetails()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/session", Method.Get);
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
    }
}
