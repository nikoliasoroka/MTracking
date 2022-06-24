using Microsoft.AspNetCore.Mvc;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Abstractions.Generics;

namespace MTracking.API.Extensions
{
    public static class ActionResultExtension
    {
        public static IActionResult ToActionResult<T>(this IResult<T> result)
        {
            if (!result.Success)
                return new BadRequestObjectResult(result.ErrorInfo);

            return new OkObjectResult(result.Data);
        }

        public static IActionResult ToActionResult(this IResult result)
        {
            if (!result.Success)
                return new BadRequestObjectResult(result.ErrorInfo);

            return new OkResult();
        }
    }
}
