using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var usuario = context.HttpContext.User;

        if (!usuario.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = usuario.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim?.Value != "Administrador" && roleClaim?.Value != "0")
        {
            context.Result = new JsonResult(new { message = "O usuário não possui acesso administrativo." })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}