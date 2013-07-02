using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

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
                return new Tuple<bool, string, dynamic>(true, "Error creating task!", null);
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
                return new Tuple<bool, string, dynamic>(true, "Error deleting task!", null);
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
                return new Tuple<bool, string, dynamic>(true, "Error updating task!", null);
            }
        }

        public static Tuple<bool, string, List<ToDodb.Tasks>> FindTaskById2(string _id)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_id))
                    return new Tuple<bool, string,List<ToDodb.Tasks>>(true, "Requires an id", null);

                var db = new enosql.EnosqlDatabase(dbasePath);
                var ret = db.GetCollection<ToDodb.Tasks>().FindById<ToDodb.Tasks>(_id);
                
                if (ret.IsError)
                    Logging.Log(new Logging.LogItem() { Msg = ret.Msg }, DefaultValues);

                return new Tuple<bool, string, List<ToDodb.Tasks>>(ret.IsError, ret.IsError ? "failed database error" : "success", ret.Data);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, List<ToDodb.Tasks>>(true, "Error finding task!", null);
            }
        }

        public static Tuple<bool, string, dynamic> FindTaskById(string _id)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_id))
                    return new Tuple<bool, string, dynamic>(true, "Requires an id", null);

                var db = new enosql.EnosqlDatabase(dbasePath);
                var ret = db.GetCollection<ToDodb.Tasks>().FindById(_id);

                if (ret.IsError)
                    Logging.Log(new Logging.LogItem() { Msg = ret.Msg }, DefaultValues);

                return new Tuple<bool, string, dynamic>(ret.IsError, ret.IsError ? "failed database error" : "success", ret.Json);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, DefaultValues);
                return new Tuple<bool, string, dynamic>(true, "Error finding task!", null);
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
