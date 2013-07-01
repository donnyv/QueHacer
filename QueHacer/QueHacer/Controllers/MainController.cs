using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QueHacer.Core;

namespace QueHacer.Controllers
{
    public class MainController : Controller
    {
        // GET: /View/
        public ActionResult Index()
        {
            return View();
        }
    }
}
