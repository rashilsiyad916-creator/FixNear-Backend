using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FixNear.Filters
{
    public class ApiResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult &&
                objectResult.StatusCode is >= 200 and < 300)
            {
                objectResult.Value = new { data = objectResult.Value };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
