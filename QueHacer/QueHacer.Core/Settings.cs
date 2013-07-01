using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueHacer.Core
{
    public class Settings
    {
        public static Logging.LogItem DefaultValues()
        {
            Logging.LogItem item = new Logging.LogItem();
            item.LogFolder = System.AppDomain.CurrentDomain.BaseDirectory + @"_Logs";
            item.LogState = true;
            item.LogMsgType = Logging.eLogMsgType.ExceptionError;
            //item.CustomLogWrite = Directorydb.WriteLogToDatabase;

#if DEBUG
            item.LogWriteType = Logging.eLogWriteType.File;
#else
                item.LogWriteType = Logging.eLogWriteType.Custom;
#endif

            return item;
        }
    }
}
