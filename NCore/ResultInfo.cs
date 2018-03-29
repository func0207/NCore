using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.IO;

namespace NCore
{
    public class ResultInfo
    {
        public static ResultInfo FromFunction<T>(BsonDocument parms = null, Func<BsonDocument, T> fn = null)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                ri.Data = fn(parms);
            }
            catch (Exception ex)
            {
                ri.PushException(ex);
            }
            ri.CalcLapseTime();
            return ri;
        }
        public static ResultInfo FromFunction<T>(object[] parms = null, Func<object[], T> fn = null)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                ri.Data = fn(parms);
            }
            catch (Exception ex)
            {
                ri.PushException(ex);
            }
            ri.CalcLapseTime();
            return ri;
        }
        public static ResultInfo FromFunction<T>(Func<T> fn = null)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                ri.Data = fn();
            }
            catch (Exception ex)
            {
                ri.PushException(ex);
            }
            ri.CalcLapseTime();
            return ri;
        }
        public static ResultInfo Execute(BsonDocument parms = null, Func<BsonDocument, object> fn = null)
        {
            return Execute<object>(parms, fn);
        }
        public static ResultInfo Execute<T>(BsonDocument parms = null, Func<BsonDocument, T> fn = null)
        {
            ResultInfo ri = ResultInfo.FromFunction<T>(parms, fn);
            return ri;
        }
        public static ResultInfo Execute(Func<object> fn = null)
        {
            ResultInfo ri = ResultInfo.FromFunction<object>(fn);
            return ri;
        }
        public static ResultInfo Execute(object[] parms = null, Func<object[], object> fn = null)
        {
            return Execute<object>(parms, fn);
        }
        public static ResultInfo Execute<T>(object[] parms = null, Func<object[], T> fn = null)
        {
            var ri = FromFunction<T>(parms, fn);
            return ri;
        }

        private DateTime _initTime;
        public ResultInfo()
        {
            License.Validate("Global");
            this._initTime = DateTime.Now;
            this.Result = "OK";
        }
        public ResultInfo(String result)
        {
            Result = result;
        }
        public double CalcLapseTime()
        {
            this.Seconds = (DateTime.Now - _initTime).TotalSeconds;
            return this.Seconds;
        }
        public string ToJson()
        {
            this.CalcLapseTime();
            return Tools.ToJson(this);
        }
        public double Seconds { get; set; }
        public Object Data { get; set; }
        public String Result { get; set; }
        public String Message { get; set; }
        public String Trace { get; set; }

        private static StreamWriter sw;

        public void PushLog(string reference1, string logGroup = "")
        {
            string logPath = System.Configuration.ConfigurationSettings.AppSettings["LogPath"];
            string filename = logGroup.Equals("") ?
                "Log_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt" :
                "Log_" + logGroup.Replace(" ", "") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String filePath = Path.Combine(logPath, filename);
            if (sw == null)
            {
                if (!File.Exists(filePath))
                {
                    sw = File.CreateText(filePath);
                }
                else
                {
                    sw = File.AppendText(filePath);
                }
            }
            string logTxt = "";

            if (Result.Equals("OK") || Result.Equals(""))
            {
                logTxt = String.Format("{0:dd-MMM-yyyy HH:mm:ss} Ref: {1}\n" +
                         "Result:{2}\n\n", DateTime.Now, reference1, Result);
            }
            else
            {
                logTxt = String.Format("{0:dd-MMM-yyyy HH:mm:ss} Ref: {1}\n" +
                            "Result:{2}\n" +
                         "Message:{3}\n" +
                         "Trace:{4}\n\n", DateTime.Now, reference1, Result, Message, Trace);
            }
            sw.WriteLine(logTxt);
        }

        public static void CloseLog()
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
        }

        public ResultInfo PushException(Exception ex)
        {
            if (ex == null)
            {
                Result = "OK";
                Message = "";
                Trace = "";
            }
            else
            {
                if (ex.InnerException == null)
                {
                    Result = "NOK";
                    Message = ex.Message;
                    Trace = ex.StackTrace;
                }
                else
                {
                    PushException(ex.InnerException);
                }
            }
            return this;
        }
    }
}
