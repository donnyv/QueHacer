using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueHacer.Core
{
    public class AppLogic
    {
        public static Tuple<bool, string> AddTask()
        {
            return new Tuple<bool, string>(true, "success");
        }

        public static string GetTodoDBjson()
        {
            return string.Empty;
        }
    }
}
