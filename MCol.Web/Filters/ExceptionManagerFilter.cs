using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MCol.Web.Filters
{
    public class ExceptionManagerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                var exceptionMessage = context.Exception.Message;
                var stackTrace = context.Exception.StackTrace;
                var controllerName = context.RouteData.Values["controller"].ToString();
                var actionName = context.RouteData.Values["action"].ToString();
                var logFilePath = "Logs/Log.txt";

                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }

                string message = $"\n\n Date: {DateTime.Now}, Controller: {controllerName}, Action: {actionName}, Error Message: {exceptionMessage}, Stack Trace: {stackTrace}";
                File.AppendAllText(logFilePath, message);

                context.ExceptionHandled = true;
                context.Result = new ViewResult()
                {
                    ViewName = "Error"
                };
            }
        }
    }
}
