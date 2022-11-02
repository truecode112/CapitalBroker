using Asp.Versioning;
using CapitalBroker.Metadatas;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CapitalBroker.Filters
{
    public class AddAPIKeyHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            operation.Parameters ??= new List<OpenApiParameter>();
            var apiKeyMetadata = actionMetadata.Any(metadataItem => metadataItem is RequiresAPIKeyAttribute);
            if (apiKeyMetadata)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-CAP-API-KEY",
                    In = ParameterLocation.Header,
                    Description = "The API key obtained from Settings->API Integrations page on the Capital.com trading platform",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                    }
                });
            }
        }
    }
}
