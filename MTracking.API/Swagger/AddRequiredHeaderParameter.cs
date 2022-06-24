using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace MTracking.API.Swagger
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Local-DateTime",
                In = ParameterLocation.Header,
                Example = new OpenApiString(DateTimeOffset.Now.ToString("dd.MM.yyyy HH:mm:ss zzz")),
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }

    }
}

