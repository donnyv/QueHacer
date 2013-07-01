using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueHacer.Core
{
    public class ToDodb
    {
        public class Tasks
        {
            public string _id { get; set; }
            public string task { get; set; }
            public double duedate { get; set; }
            public string category { get; set; }
            public string status { get; set; }
        }
    }
}
