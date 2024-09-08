using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace TaskManagement.Filter
{
    public class AjaxOnlyAttribute: ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var request = routeContext.HttpContext.Request;
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" && request.Headers["Sec-Fetch-Mode"] == "cors";
        }
    }
}
