using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QueHacer.Core.Attributes
{
    public class JsonFilter : ActionFilterAttribute
    {
        public string Param { get; set; }
        public Type RootType { get; set; }

        static Logging.LogItem DefaultValues()
        {
            Logging.LogItem item = Settings.DefaultValues();
            item.LogFile = typeof(JsonFilter).Name;
            return item;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!(filterContext.HttpContext.Request.ContentType ?? string.Empty).Contains("application/json")) return;

                filterContext.HttpContext.Request.InputStream.Position = 0;
                using (StreamReader reader = new StreamReader(filterContext.HttpContext.Request.InputStream))
                {
                    var value = reader.ReadToEnd();
                    
                    //var o = JsonConvert.DeserializeObject(value, RootType);
                    filterContext.ActionParameters[Param] = JsonConvert.DeserializeObject(value, RootType);
                    
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
            }
        }
    }
}
