using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CSService.API;

internal sealed class SupportsRestfulApi : IResultFilter
{
    public static SupportsRestfulApi Instance => new();

    public void OnResultExecuted(ResultExecutedContext context) {
        if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor) return;

        var returnType = actionDescriptor.MethodInfo.ReturnType;
        if (returnType == typeof(void) || returnType == typeof(Task)) {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
        } else if (context.Result is ObjectResult { Value: null }) {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }

    public void OnResultExecuting(ResultExecutingContext context) { }
}
