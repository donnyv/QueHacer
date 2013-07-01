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
        class User
        {
            public string _id { get; set; }
            public string UserName { get; set; }
        }
        public static Tuple<bool, string, dynamic> AddTask()
        {
            //var db = new enosql.EnosqlDatabase(dbasePath);
            //db.GetCollection("test").Insert<User>(new User() { UserName = "Raven" });

            return new Tuple<bool, string, dynamic>(true, "success", new object());
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
