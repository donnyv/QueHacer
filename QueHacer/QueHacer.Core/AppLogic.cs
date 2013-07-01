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
                return new Tuple<bool, string, dynamic>(false, "success", new object());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Could not create task!", new object());
            }
        }

        public static Tuple<bool, string, dynamic> DeleteTask(string id)
        {
            try
            {
                var db = new enosql.EnosqlDatabase(dbasePath);
                db.GetCollection("Tasks").Remove(id);
                return new Tuple<bool, string, dynamic>(false, "success", new object());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Could not delete task!", new object());
            }
        }

        public static Tuple<bool, string, dynamic> UpdateTask()
        {
            return new Tuple<bool, string, dynamic>(true, "success", new object());
        }

        public static string GetTodoDBjson()
        {
            var dbJSON = "var todoDB = {};";

            var db = new enosql.EnosqlDatabase(dbasePath);
            var ret = db.GetCollection<ToDodb.Tasks>().FindAll();
            dbJSON += "todoDB.Tasks = " + ret.Json + ";";
            return dbJSON;
        }
    }
}
