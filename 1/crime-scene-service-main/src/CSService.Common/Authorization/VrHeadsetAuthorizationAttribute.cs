using System;
using System.Net;
using System.Threading.Tasks;
using CSService.Common.DataAccess;
using CSService.Common.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CSService.Common.Authorization;

[AttributeUsage(AttributeTargets.Method)]
public sealed class VrHeadsetAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaders.MAC_ADDRESS, out var macAddress)) {
            context.Result = CreateAnswer();
            return;
        }

        var sqlRepository = context.HttpContext.RequestServices.GetRequiredService<ISqlRepository>();
        var vrHeadset = await sqlRepository.Query<VrHeadset>(
            "select * from vr_headsets where mac_address = @MacAddress",
            new { MacAddress = macAddress });

        if (vrHeadset is null) {
            context.Result = CreateAnswer();
            return;
        }

        var vrHeadsetContext = context.HttpContext.RequestServices.GetRequiredService<IVrHeadsetContext>();
        vrHeadsetContext.SetCurrentVrHeadset(vrHeadset);
    }

    private static ObjectResult CreateAnswer() =>
        new(null) { StatusCode = (int)HttpStatusCode.Unauthorized };
}
