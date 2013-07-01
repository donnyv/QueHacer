using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace QueHacer.Core
{
    public class AppLogic
    {
        static string dbasePath = System.AppDomain.CurrentDomain.BaseDirectory + @"_Database\Todo.jdb";

        static Logging.LogItem DefaultValues()
        {
            Logging.LogItem item = Settings.DefaultValues();
            item.LogFile = typeof(AppLogic).Name;
            return item;
        }

        public static Tuple<bool, string, dynamic> AddTask(ToDodb.Tasks task)
        {
            try
            {
                var db = new enosql.EnosqlDatabase(dbasePath);
                db.GetCollection("Tasks").Insert<ToDodb.Tasks>(task);
                return new Tuple<bool, string, dynamic>(true, "success", new object());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(false, "Could not create task!", new object());
            }
        }

        public static Tuple<bool, string, dynamic> DeleteTask()
        {
            return new Tuple<bool, string, dynamic>(true, "success", new object());
        }

        public static Tuple<bool, string, dynamic> UpdateTask()
        {
            return new Tuple<bool, string, dynamic>(true, "success", new object());
        }

        public static string GetTodoDBjson()
        {
            var db = "var todoDB = [];";
            return db;
        }
    }
}
