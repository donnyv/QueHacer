using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueHacer.Core
{
    public class AppLogic
    {
        public static Tuple<bool, string, dynamic> AddTask()
        {
            return new Tuple<bool, string, dynamic>(true, "success", new object());
        }

        public static string GetTodoDBjson()
        {
            return string.Empty;
        }
    }
}
