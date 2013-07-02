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
                if (task == null || string.IsNullOrWhiteSpace(task.task))
                    return new Tuple<bool, string, dynamic>(true, "Empty task sent", null);

                var db = new enosql.EnosqlDatabase(dbasePath);
                var ret = db.GetCollection<ToDodb.Tasks>().Insert(task);

                if(ret.IsError)
                    Logging.Log(new Logging.LogItem() { Msg = ret .Msg}, DefaultValues);
                
                return new Tuple<bool, string, dynamic>(ret.IsError, ret.IsError ? "failed database error" : "success", null);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Could not create task!", null);
            }
        }

        public static Tuple<bool, string, dynamic> DeleteTask(string id)
        {
            try
            {
                var db = new enosql.EnosqlDatabase(dbasePath);
                var ret = db.GetCollection<ToDodb.Tasks>().Remove(id);

                if (ret.IsError)
                    Logging.Log(new Logging.LogItem() { Msg = ret.Msg }, DefaultValues);

                return new Tuple<bool, string, dynamic>(ret.IsError, ret.IsError ? "failed database error" : "success", null);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Could not delete task!", null);
            }
        }

        public static Tuple<bool, string, dynamic> UpdateTask(ToDodb.Tasks task)
        {
            try
            {
                if (task == null || string.IsNullOrWhiteSpace(task.task))
                    return new Tuple<bool, string, dynamic>(true, "Empty task sent", null);

                var db = new enosql.EnosqlDatabase(dbasePath);
                var ret = db.GetCollection<ToDodb.Tasks>().Update(task);

                if (ret.IsError)
                    Logging.Log(new Logging.LogItem() { Msg = ret.Msg }, DefaultValues);

                return new Tuple<bool, string, dynamic>(ret.IsError, ret.IsError ? "failed database error" : "success", null);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Could not update task!", null);
            }
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
