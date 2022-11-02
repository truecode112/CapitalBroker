using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using RestSharp;
using Asp.Versioning.Conventions;

namespace CapitalBroker
{
    public static class General
    {
        public static IEndpointRouteBuilder MapGeneralEndPoints(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

            routes.MapGet("/GetServerTime", GetServerTime).Produces(200);
            routes.MapGet("/PingService", PingService).Produces(200);

            return routes;
        }

        public static async Task<IResult> GetServerTime()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/time", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            
            return Results.Ok(output);
        }

        public static async Task<IResult> PingService()
        {
            var client = new RestClient(Global.SERVER_URL);
            var request = new RestRequest("/api/v1/ping", Method.Get);
            request.AddHeader("X-SECURITY-TOKEN", Global.SECURITY_TOKEN);
            request.AddHeader("CST", Global.CST_TOKEN);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;

            return Results.Ok(output);
        }
    }
}
