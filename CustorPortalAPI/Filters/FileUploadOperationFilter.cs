using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CustorPortalAPI.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFileCollection))
                .ToList();

            if (fileParameters.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    },
                                    ["uploaderKey"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["version"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["isCurrent"] = new OpenApiSchema { Type = "boolean" }
                                },
                                Required = new HashSet<string> { "file", "uploaderKey", "version" }
                            }
                        }
                    }
                };

                // Preserve path parameters (like projectId), remove only non-path parameters
                var pathParameters = operation.Parameters
                    .Where(p => p.In == ParameterLocation.Path)
                    .ToList();
                operation.Parameters.Clear();
                foreach (var param in pathParameters)
                {
                    operation.Parameters.Add(param);
                }
            }
        }
    }
}