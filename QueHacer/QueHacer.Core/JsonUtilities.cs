using System.Web.Mvc;

using Newtonsoft.Json;


namespace QueHacer.Core
{
    public class JsonUtilities
    {
        public class ResultClass
        {
            public ResultClass()
            {

            }

            public ResultClass(bool IsError)
            {
                this.IsError = IsError;
            }

            public ResultClass(object Data)
            {
                this.Data = Data;
            }

            public ResultClass(bool IsError, string StatusMsg)
            {
                this.IsError = IsError;
                this.StatusMsg = StatusMsg;
            }

            public ResultClass(bool IsError, string StatusMsg, object Data)
            {
                this.IsError = IsError;
                this.StatusMsg = StatusMsg;
                this.Data = Data;
            }

            public bool IsError = false;
            public string StatusMsg = string.Empty;
            public object Data = null;

            [JsonIgnore]
            public string JsonResult
            {
                get
                {
                    return JsonConvert.SerializeObject(this);
                }
            }

            [JsonIgnore]
            public ContentResult JsonContentResult
            {
                get
                {
                    return (new ContentResult() { Content = this.JsonResult, ContentType = MimeTypes.application_json });
                }
            }
        }

        /// <summary>
        /// returns JSON string "{'status': true or false}"
        /// </summary>
        /// <param name="exists"></param>
        /// <returns></returns>
        public static string status(bool exists)
        {
            return "{\"status\":" + exists.ToString().ToLower() + "}";
        }

        public static ContentResult StatusContentResult(bool exists)
        {
            return (new ContentResult() { Content = status(exists), ContentType = MimeTypes.application_json });
        }

        public static ResultClass Result(bool IsError){
            return new ResultClass(IsError);
        }

        public static ResultClass Result(object Data)
        {
            if (Data == null)
                return new ResultClass(true);
            else
                return new ResultClass(Data);
        }

        public static ResultClass Result(bool IsError, string StatusMsg)
        {
            return new ResultClass(IsError, StatusMsg);
        }

        public static ResultClass Result(bool IsError, string StatusMsg, object Data)
        {
            return new ResultClass(IsError, StatusMsg, Data);
        }

        public static ContentResult JsonContentResult(string json){
            return (new ContentResult() { Content = json, ContentType = MimeTypes.application_json });
        }
    }
}
