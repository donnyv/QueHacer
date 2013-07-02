using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QueHacer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Task",
                url: "Task/{id}",
                defaults: new { controller = "Task", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "App",
                url: "App/{action}/{id}",
                defaults: new { controller = "App", action = UrlParameter.Optional, id = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}