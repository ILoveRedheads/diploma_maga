using System;
using System.Collections.Generic;
using CSService.Common.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CSService.API.Swagger;

public sealed class AddSecurityRequirements<T> : IOperationFilter where T : Attribute
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttributes(typeof(T), inherit: true).Length != 0) {
            var openApiSecurityScheme = new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = AuthorizationHeaders.MAC_ADDRESS
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement> {
                new() { [openApiSecurityScheme] = Array.Empty<string>() }
            };

            operation.Parameters.Add(new OpenApiParameter {
                Name = AuthorizationHeaders.MAC_ADDRESS,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
