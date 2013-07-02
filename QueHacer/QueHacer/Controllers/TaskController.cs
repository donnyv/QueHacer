using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QueHacer.Core;

namespace QueHacer.Controllers
{
    public class TaskController : Controller
    {
        //
        // GET: /Task/
        [HttpGet]
        public ActionResult Index(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                return View(new List<ToDodb.Tasks>());

            var ret = AppLogic.FindTaskById2(id);

            if(ret.Item3 == null)
                return View(new List<ToDodb.Tasks>());

            return View(ret.Item3);
        }

    }
}
