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
        
        // GET: /App/
        public ActionResult Index()
        {
            return View();
        }

        public JavaScriptResult TodoDB(string id)
        {
            return (new JavaScriptResult() { Script = AppLogic.GetTodoDBjson() });
        } 

        [HttpPost]
        public ContentResult AddTask()
        {
            object ret = null;
            var ErrorMsg = string.Empty;
            if (ret == null)
                return JsonUtilities.Result(true, ErrorMsg).JsonContentResult;
            else
                return JsonUtilities.Result(ret).JsonContentResult; 
        }
    }
}
