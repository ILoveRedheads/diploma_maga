using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CSService.API.Swagger;

public sealed class AddAuthorizeResponses<T> : IOperationFilter where T : Attribute
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (context.MethodInfo.GetCustomAttributes(true).Any(x => x.GetType() == typeof(T) ||
            context.ApiDescription.TryGetMethodInfo(out var methodInfo) &&
            methodInfo.MemberType == MemberTypes.Method &&
            methodInfo.DeclaringType?.GetCustomAttribute<T>(true) is not null)) {
            var schema = context.SchemaGenerator.GenerateSchema(typeof(IActionResult), context.SchemaRepository);
            var responses = operation.Responses;
            CheckAndAdd(responses, schema, HttpStatusCode.Unauthorized);
        }
    }

    private static void CheckAndAdd(IDictionary<string, OpenApiResponse> responses, OpenApiSchema schema, HttpStatusCode httpStatus) {
        var num = (int)httpStatus;
        var key = num.ToString();
        var message = httpStatus.ToString();

        if (!responses.ContainsKey(key)) responses[key] = AddResponse(message, schema);
    }

    private static OpenApiResponse AddResponse(string message, OpenApiSchema schema) => new() {
        Description = message,
        Content = new Dictionary<string, OpenApiMediaType> {
            ["application/json"] = new OpenApiMediaType {
                Schema = schema
            }
        }
    };
}
