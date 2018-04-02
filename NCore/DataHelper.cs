using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Collections.Generic;
namespace NCore
{
    public class DataHelper
    {
        static DataHelper()
        {
            if (License.Validate("Global") == false) throw new Exception("ERR_INVALID_LICENSE");
        }
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

        private static string fieldsBsonBuilder(string [] fields)
        {
            List<string> data = new List<string>();
            foreach(var f in fields)
                data.Add(f + ": 1");
            string ret = string.Join(", ", data);
            return "{ " + ret + " }";
        }

        public static List<BsonDocument> Populate(string memoryId,
           FilterDefinition<BsonDocument> q = null, int take = 0, int skip = 0,
           string [] fields = null,
           SortDefinition<BsonDocument> sort = null,
           string collectionName = "", bool memoryObject = false, bool forceReadDb = false)
        {
            var ret = new List<BsonDocument>();
            if (collectionName.Equals(""))
                collectionName = memoryId;

            //if (memoryObject == true && forceReadDb == false)
            //    ret = MemoryHelper.Populate<BsonDocument>(memoryId);
            //bool saveToMemory = false;
            if (ret.Count == 0)
            {
                string projectFiedlBson = string.Empty;
                if (fields != null)
                    projectFiedlBson = fieldsBsonBuilder(fields);

                 var cursor = q == null ?
                    GetDb().GetCollection<BsonDocument>(collectionName).Find(_ => true) :
                    GetDb().GetCollection<BsonDocument>(collectionName).Find<BsonDocument>(q);

                if (fields != null && fields.Length > 0)
                    cursor.Project(projectFiedlBson);
                if (sort != null)
                    cursor.Sort(sort);
                if (take == 0)
                {
                    ret = cursor.ToList();
                }
                else
                {
                    cursor.Skip(skip);
                    cursor.Limit(take);
                    return cursor.ToList();
                }
                //if (memoryObject == true)
                //    saveToMemory = true;
            }

            //if (saveToMemory)
            //    MemoryHelper.Save(memoryId, ret.Select(d=>(object)d).ToList());

            return ret;
        }

    }
}
