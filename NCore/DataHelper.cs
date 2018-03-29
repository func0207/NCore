using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace NCore
{
    public class DataHelper
    {
        internal static IMongoDatabase _db;

        public static IMongoDatabase GetDb()
        {
            if (_db == null)
            {
                var isUseConStr = System.Configuration.ConfigurationManager.AppSettings["EncryptConStr"];
                if (isUseConStr != null && isUseConStr != "")
                {
                    var constr = License.Decyp(isUseConStr);
                    var cs = String.Format("mongodb://{0}", constr);
                    var conn = new MongoClient(cs);
                    _db = conn.GetDatabase(ConfigurationSettings.AppSettings["ServerDb"]);
                }
                else
                {
                    var cs = String.Format("mongodb://{0}", ConfigurationSettings.AppSettings["ServerHost"]);
                    var conn = new MongoClient(cs);
                    _db = conn.GetDatabase(ConfigurationSettings.AppSettings["ServerDb"]);
                }

            }
            return _db;
        }


    }
}
