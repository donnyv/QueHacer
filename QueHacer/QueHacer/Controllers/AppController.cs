using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QueHacer.Core;
using QueHacer.Core.Attributes;

namespace QueHacer.Controllers
{
    public class AppController : Controller
    {

        public JavaScriptResult TodoDB()
        {
            return (new JavaScriptResult() { Script = AppLogic.GetTodoDBjson() });
        }

        [HttpGet]
        public ContentResult Task(string id)
        {
            return JsonUtilities.Result(false).JsonContentResult; 
        }



        //used to debug controller
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }
        //end used to debug controller




        [HttpPost]
        public ContentResult AddTask(ToDodb.Tasks NewTask)
        {
            AppLogic.AddTask();
            return JsonUtilities.Result(false).JsonContentResult;  
        }

        [HttpPost]
        public ContentResult DeleteTask()
        {
            return JsonUtilities.Result(false).JsonContentResult;
        }

        [HttpPost]
        public ContentResult UpdateTask()
        {
            return JsonUtilities.Result(false).JsonContentResult;
        }
    }
}
