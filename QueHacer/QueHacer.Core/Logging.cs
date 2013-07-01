/*
   Author: Donny Velazquez
  
 * Revisions
 * ------------------------------------
 * 12/26/10 - Released :Donny V.
 * 03/16/11 - Stream lined and refactored functions. Made it a static function and added reflection to get
 *            calling function name.
 * 08/01/12 - added ability to write to windows event 
 * ------------------------------------
 * 
	******EXAMPLE CODE********
    public class Settings
    {
        public static Logging.LogItem DefaultValues()
        {
            Logging.LogItem item = new Logging.LogItem();
            item.LogFolder = System.AppDomain.CurrentDomain.BaseDirectory + @"_Logs";
            item.LogState = true;
            item.LogMsgType = Logging.eLogMsgType.ExceptionError;
            item.CustomLogWrite = Directorydb.WriteLogToDatabase; // your custom function to store error log

            // When project is in debug mode it will write to a file.
            // When in release mode will use users custom function.
            // You don't need to do this, you could just leave it in file mode, up to developer.
            #if DEBUG
                item.LogWriteType = Logging.eLogWriteType.File;
            #else
                item.LogWriteType = Logging.eLogWriteType.Custom;
            #endif

            return item;
        }
    }
	public class cGenericFunctions
	{
        static Logging.LogItem DefaultValues()
        {
            Logging.LogItem item = Settings.DefaultValues();
            item.LogFile = typeof(cGenericFunctions).Name;
            return item;
        }
 
		public string StripServerSettings()
	        {
	            try
	            {
	                cSettings Settings = new cSettings();
	                XmlDocument XDoc = new XmlDocument();
	                XDoc.LoadXml(Settings.MapServicesTemplate);

	                XmlNode XNode = XDoc.DocumentElement.SelectSingleNode("/Services/SiteInfo");

	                //Clean up
	                XDoc = null;

	                return XNode.OuterXml;
	            }
	            catch (Exception ex)
	            {
	                Logging.Log(ex, DefaultValues);

	                return string.Empty;
	            }
	        }
	}
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;


public class Logging
{
    public enum eLogWriteType
    {
        Unknown,
        File,
        WindowsEvent,
        Custom
    }

    public enum eLogMsgType
    {
        Unknown,
        ExceptionError,
    }

    public delegate LogItem DefaultValuesDelegate();
    public delegate void CustomLogWriteDelegate(LogItem item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item">Logitem contains all info needed to create a log</param>
    public static void Log(LogItem item)
    {
        Log(null, item, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ex">Use the exception to pull all info for the Logitem</param>
    /// <param name="DefaultValues">This function outputs the default LogItem values</param>
    public static void Log(Exception ex, DefaultValuesDelegate DefaultValues)
    {
        Log(ex, null, DefaultValues);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item">Logitem contains all info needed to create a log</param>
    /// <param name="DefaultValues">This function outputs the default LogItem values</param>
    public static void Log(LogItem item, DefaultValuesDelegate DefaultValues)
    {
        Log(null, item, DefaultValues);
    }

    /// <summary>
    /// Function name is the only property that can't be over written with the LogItem.
    /// It uses reflection to get the function name.
    /// </summary>
    /// <param name="ex">Use the exception to pull all info for the Logitem</param>
    /// <param name="item">Logitem contains all info needed to create a log.</param>
    /// <param name="DefaultValues">This function outputs the default LogItem values</param>
    public static void Log(Exception ex, LogItem item, DefaultValuesDelegate DefaultValues)
    {
        if (ex != null)
        {
            if (item == null)
            {
                item = new LogItem();
                item.Msg = "\r\n[System Error]" + ex.Message;

                string[] ret = PullFunctionAndLineNumber(ex.StackTrace, item);
                item.FunctionName = ret[0];
                item.ErrorLineNumber = ret[1];
            }
            else
            {
                string[] ret = PullFunctionAndLineNumber(ex.StackTrace, item);
                item.FunctionName = ret[0];
                item.ErrorLineNumber = ret[1];

                if (string.IsNullOrEmpty(item.Msg))
                    item.Msg = "\r\n[System Error]" + ex.Message;
                else
                    item.Msg = "\r\n" + item.Msg + "\r\n[System Error]" + ex.Message;
            }
        }

        if (DefaultValues != null)
        {
            LogItem _DefaultItem = DefaultValues();

            if (string.IsNullOrEmpty(item.LogFolder))
                item.LogFolder = _DefaultItem.LogFolder;

            if (string.IsNullOrEmpty(item.LogFile))
                item.LogFile = _DefaultItem.LogFile;

            if (item.LogState == null)
                item.LogState = _DefaultItem.LogState;

            if (item.LogMsgType == eLogMsgType.Unknown)
                item.LogMsgType = _DefaultItem.LogMsgType;

            if (item.LogWriteType == eLogWriteType.Unknown)
                item.LogWriteType = _DefaultItem.LogWriteType;

            if (item.CustomLogWrite == null)
                item.CustomLogWrite = _DefaultItem.CustomLogWrite;
        }

        WriteLog(item);
    }

    static void WriteLog(LogItem _LogItem)
    {
        try
        {
            switch (_LogItem.LogWriteType)
            {
                case eLogWriteType.Custom:
                    if (_LogItem.CustomLogWrite != null)
                        _LogItem.CustomLogWrite(_LogItem);
                    else
                        WriteLogFile(_LogItem);
                    break;
                case eLogWriteType.File:
                    WriteLogFile(_LogItem);
                    break;
                case eLogWriteType.WindowsEvent:
                    WriteToWindowEventLog(_LogItem);
                    break;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("OrignalError:" + _LogItem.GetFormatedMsg + " Class:" + _LogItem.LogFile +
            "\n\r[Error:]Logging.WriteLog" + "\r\n" + "[" + DateTime.Now + "] " + "[Msg:]" + ex.Message);
        }
    }

    static void WriteLogFile(LogItem _LogItem)
    {
        if (!Directory.Exists(_LogItem.LogFolder))
        {
            if (Directory.CreateDirectory(_LogItem.LogFolder).Exists)
            {
                WriteFile(_LogItem);
            }
            else
            {
                throw new Exception("Log folder " + _LogItem.LogFolder + " could not be created!");
            }
        }
        else
        {
            WriteFile(_LogItem);
        }
    }

    static void WriteFile(LogItem _LogItem)
    {
        try
        {
            StreamWriter FSO = File.AppendText(_LogItem.LogFolder + "\\" + _LogItem.LogFile + ".log");
            FSO.Write("[" + DateTime.Now + "] " + _LogItem.GetFormatedMsg + "\r\n");
            FSO.Close();
        }
        catch (Exception ex)
        {
            throw new Exception("[Error:]Logging.WriteFile" + "\r\n" + "[" + DateTime.Now + "] " + "[Msg:]" + ex.Message);
        }
    }

    static string[] PullFunctionAndLineNumber(string StackTrace, LogItem _LogItem)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(StackTrace))
                return new string[] { "", "" };

            string LastLine = StackTrace.Substring(StackTrace.LastIndexOf("\r\n at ") + 6);
            string[] FunctionLine = LastLine.Split(new string[] { " in " }, StringSplitOptions.None);
            string FunctionName = FunctionLine.Length > 0 ? FunctionLine[0] : string.Empty;

            if (FunctionName == string.Empty)
                return new string[] { "", "" };

            FunctionName = FunctionName.Substring(FunctionName.LastIndexOf(".") + 1);


            string lineNum = string.Empty;
            int LastLineInt = LastLine.LastIndexOf(":");
            if (LastLineInt == -1)
                return new string[] { FunctionName, lineNum };


            string lineNumSubstring = LastLine.Substring(LastLineInt);
            if (string.IsNullOrWhiteSpace(lineNumSubstring))
                return new string[] { FunctionName, lineNum };


            string[] lineNumLine = lineNumSubstring.Split(" ".ToCharArray());
            lineNum = lineNumLine.Length > 0 ? lineNumLine[1] : string.Empty;


            return new string[] { FunctionName, lineNum };
        }
        catch (Exception ex)
        {
            throw new Exception("OrignalError:" + StackTrace + " Class:" + _LogItem.LogFile +
            "\n\r[Error:]Logging.PullFunctionAndLineNumber" + "\r\n" + "[" + DateTime.Now + "] " + "[Msg:]" + ex.Message);
            //return new string[] { "Error" };
        }
    }

    static void WriteToWindowEventLog(LogItem _LogItem)
    {
        try
        {
            if (!EventLog.SourceExists(_LogItem.ApplicationName))
                EventLog.CreateEventSource(_LogItem.ApplicationName, "Application");

            EventLog.WriteEntry(_LogItem.ApplicationName, _LogItem.GetFormatedMsg, EventLogEntryType.Error);
        }
        catch (Exception ex)
        {
            throw new Exception("[Error:]Logging.WriteFile" + "\r\n" + "[" + DateTime.Now + "] " + "[Msg:]" + (ex.Message == null ? string.Empty : ex.Message));
        }
    }

    public class LogItem
    {
        public LogItem()
        {
            this.LogMsgType = eLogMsgType.Unknown;
            this.LogWriteType = eLogWriteType.Unknown;
        }

        /// <summary>
        /// Folder where log files will be written too.
        /// Make sure application has read/write rights
        /// </summary>
        public string LogFolder { get; set; }


        public bool? LogState { get; set; }

        /// <summary>
        /// Used when writing logs to windows event viewer
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Name of function exeception was thrown on.
        /// If not set will be parsed from exception message.
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Line where error occured.
        /// Parsed from exception message
        /// </summary>
        public string ErrorLineNumber { get; set; }

        /// <summary>
        /// Log message
        /// </summary>
        public string Msg { get; set; }
        public List<KeyValuePair<string, string>> AddtionalValues = null;

        /// <summary>
        /// Name of the file the logs are being saved too.
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Log message type
        /// </summary>
        public eLogMsgType LogMsgType { get; set; }

        /// <summary>
        /// Returns a formated log message with function name and line number of error if exists
        /// </summary>
        public string GetFormatedMsg
        {
            get
            {
                if (this.ErrorLineNumber == string.Empty)
                    return "[" + this.LogMsgType.ToString() + "]" + "[" + this.FunctionName + "()]\n" + this.Msg + "\n";
                else
                    return "[" + this.LogMsgType.ToString() + "]" + "[" + this.FunctionName + "():line" + this.ErrorLineNumber + "]\n" + this.Msg + "\n";
            }
        }

        /// <summary>
        /// Sets where the log will be written too.
        /// e.g. file, database, windows event, etc.
        /// </summary>
        public eLogWriteType LogWriteType { get; set; }

        /// <summary>
        /// Reference of custom function to store error log.
        /// </summary>
        public CustomLogWriteDelegate CustomLogWrite = null;
    }
}