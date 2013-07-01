using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QueHacer.Core;

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

        [HttpPost]
        public ContentResult AddTask(ToDodb.Tasks task)
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
