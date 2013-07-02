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

        [HttpPost]
        public ContentResult AddTask(ToDodb.Tasks NewTask)
        {
            var ret = AppLogic.AddTask(NewTask);
            return JsonUtilities.Result(ret.Item1, ret.Item2, ret.Item3).JsonContentResult;  
        }

        [HttpPost]
        public ContentResult DeleteTask(string id)
        {
            var ret = AppLogic.DeleteTask(id);
            return JsonUtilities.Result(ret.Item1, ret.Item2, ret.Item3).JsonContentResult;
        }

        [HttpPost]
        public ContentResult UpdateTask(ToDodb.Tasks Task)
        {
            var ret = AppLogic.UpdateTask(Task);
            return JsonUtilities.Result(ret.Item1, ret.Item2, ret.Item3).JsonContentResult;
        }
    }
}
