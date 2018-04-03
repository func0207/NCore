using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Web;

namespace NCore
{
    public class MapReduceDocument
    {
        public long Count { get; set; }
        public List<BsonDocument> Result { get; set; }
    }

    public class JoinItems
    {
        public BsonDocument Document1 { get; set; }
        public BsonDocument Document2 { get; set; }
    }
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

        private static string fieldsBsonBuilder(string[] fields)
        {
            List<string> data = new List<string>();
            foreach (var f in fields)
                data.Add(f + ": 1");
            string ret = string.Join(", ", data);
            return "{ " + ret + " }";
        }

        public static List<BsonDocument> Populate(string memoryId,
           FilterDefinition<BsonDocument> q = null, int take = 0, int skip = 0,
           string[] fields = null,
           SortDefinition<BsonDocument> sort = null,
           string collectionName = "", bool memoryObject = false, bool forceReadDb = false)
        {
            var ret = new List<BsonDocument>();
            if (collectionName.Equals(""))
                collectionName = memoryId;

            //if (memoryObject == true && forceReadDb == false)
            //{
            //    var re = MemoryHelper.Populate(collectionName);
            //    foreach (var r in re)
            //        ret.Add(r.ToBsonDocument());
            //}
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

        public static List<T> Populate<T>(string memoryId,
           FilterDefinition<BsonDocument> _q = null, int take = 0, int skip = 0,
           string[] fields = null,
           SortDefinition<BsonDocument> sort = null,
           string collectionName = "", bool memoryObject = false, bool forceReadDb = false)
        {

            var docs = DataHelper.Populate(memoryId, q: _q, take: take, skip: skip, fields: fields, sort: sort, collectionName: collectionName,
                memoryObject: memoryObject, forceReadDb: forceReadDb);
            List<T> ret = new List<T>();
            foreach (var doc in docs)
            {
                try
                {
                    ret.Add(BsonSerializer.Deserialize<T>(doc));
                }
                catch (Exception exc)
                {
                    throw new Exception("Unable to deserialize BsonDocument => " + JsonConvert.SerializeObject(doc.ToDictionary()));
                }
            }
            return ret;
        }

        public static BsonDocument Get(string collectionId, int id)
        {
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            return Get(collectionId, q);
        }

        public static BsonDocument Get(string collectionId, string id)
        {
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            return Get(collectionId, q);
        }

        public static BsonDocument Get(string collectionId, double id)
        {
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            return Get(collectionId, q);
        }

        public static BsonDocument Get(string collectionId, DateTime id)
        {
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            return Get(collectionId, q);
        }

        public static BsonDocument Get(string collectionId,
            FilterDefinition<BsonDocument> q = null, SortDefinition<BsonDocument> sort = null)
        {
            BsonDocument ret = null;
            var docs = Populate(collectionId, q, take: 1, sort: sort);
            if (docs.Count > 0)
            {
                ret = docs[0];
            }
            return ret;
        }

        public static T Get<T>(string collectionId, 
           FilterDefinition<BsonDocument> q = null,
           SortDefinition<BsonDocument> sort = null)
        {
            T ret = default(T);
            var docs = Populate(collectionId, q, take: 1, sort: sort);
            if (docs.Count > 0)
            {
                //var json = docs[0].ToJsonResult();
                ret = BsonSerializer.Deserialize<T>(docs[0]);
            }
            return ret;
        }

        public static T Get<T>(string collectionId, string _id)
        {
            T ret = default(T);
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", _id);
            var docs = Populate(collectionId, q, take: 1);
            if (docs.Count > 0)
            {
                ret = BsonSerializer.Deserialize<T>(docs[0]);
            }
            return ret;
        }

        public static T Get<T>(string collectionId, int _id)
        {
            T ret = default(T);
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", _id);
            var docs = Populate(collectionId, q, take: 1);
            if (docs.Count > 0)
            {
                ret = BsonSerializer.Deserialize<T>(docs[0]);
            }
            return ret;
        }

        public static T Get<T>(string collectionId, object _id)
        {
            T ret = default(T);
            if (_id.GetType().ToString().ToLower().Contains("datetime"))
            {
                ret = Get<T>(collectionId, Tools.ToUTC((DateTime)_id));
            }
            else if (_id.GetType().ToString().ToLower().Contains("int16"))
            {
                ret = Get<T>(collectionId, (Int16)_id);
            }
            else if (_id.GetType().ToString().ToLower().Contains("int32"))
            {
                ret = Get<T>(collectionId, (Int32)_id);
            }
            else if (_id.GetType().ToString().ToLower().Contains("int64"))
            {
                ret = Get<T>(collectionId, (Int64)_id);
            }
            else if (_id.GetType().ToString().ToLower().Contains("decimal"))
            {
                ret = Get<T>(collectionId, (Decimal)_id);
            }
            else if (_id.GetType().ToString().ToLower().Contains("double"))
            {
                ret = Get<T>(collectionId, (Double)_id);
            }
            else
            {
                ret = Get<T>(collectionId, _id.ToString());
                if (Tools.IsNumber(_id) && ret == null) ret = Get<T>(collectionId, (int)_id);
                if (Tools.IsDate(_id) && ret == null) ret = Get<T>(collectionId, Tools.ToUTC((DateTime)_id));
            }


            return ret;
        }

        public static T Get<T>(string collectionId, DateTime _id)
        {
            T ret = default(T);
            var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", _id);
            var docs = Populate(collectionId, q, take: 1);
            if (docs.Count > 0)
            {
                ret = BsonSerializer.Deserialize<T>(docs[0]);
            }
            return ret;
        }



        public static List<JoinItems> Join(List<BsonDocument> docs1, List<BsonDocument> docs2,
          Func<BsonDocument, object> fn1, Func<BsonDocument, object> fn2 = null,
          bool leftJoin = false)
        {
            List<JoinItems> items = new List<JoinItems>();
            if (docs1 == null || docs1.Count == 0) return items;
            if (fn2 == null) fn2 = fn1;
            if (leftJoin == false)
            {
                items = (from doc1 in docs1
                         join doc2 in docs2 on fn1(doc1) equals fn2(doc2)
                         select new JoinItems
                         {
                             Document1 = doc1,
                             Document2 = doc2
                         }).ToList();
            }
            else
            {
                items = (from doc1 in docs1
                         join doc2 in docs2 on fn1(doc1) equals fn2(doc2) into joinDocs
                         from joinDoc in joinDocs.DefaultIfEmpty()
                         select new JoinItems
                         {
                             Document1 = doc1,
                             Document2 = joinDoc == null ? null : joinDoc
                         }).ToList();
            }
            return items;
        }

       

        public static Task MassUpdate(string collection,
           FilterDefinition<BsonDocument> q = null,
           UpdateDefinition<BsonDocument> ipd = null)
        {
            var db = GetDb();
            return Task.Run(() => GetDb().GetCollection<BsonDocument>(collection).UpdateMany(q, ipd));
        }

        public static Task MassInsert(string collection, List<BsonDocument> docs)
        {
            var db = GetDb();
            return Task.Run(() => GetDb().GetCollection<BsonDocument>(collection).InsertMany(docs));
        }

        public static Task MassInsert<T>(string collection, List<T> docs)
        {
            var db = GetDb();
            return Task.Run(() => GetDb().GetCollection<T>(collection).InsertMany(docs));
        }


        public static BsonDocument FindAndModify(string tablename, BsonDocument doc)
        {
            var db = GetDb();
            return doc;
        }

        public static void DropTable(string tablename)
        {
            var db = GetDb(); 
            if (db.GetCollection<BsonDocument>(tablename) != null)
                db.DropCollection(tablename);
        }
       

        public static bool TableExist(string tablename)
        {
            if (GetDb().GetCollection<BsonDocument>(tablename) != null)
                return true;
            return false;
        }


        public static List<BsonDocument> Aggregate(string collectionId, List<BsonDocument> args)
        {
            var ret = GetDb().GetCollection<BsonDocument>(collectionId).Aggregate<BsonDocument>(args).ToList<BsonDocument>();
            return ret;
        }

        internal static List<BsonDocument> AggregateFromFile(string collection, string file, FilterDefinition<BsonDocument> q = null, BsonDocument docReplace = null)
        {
            //List<BsonDocument> pipesAggr = BsonHelper.ListDocFromFile(file,docReplace);
            List<BsonDocument> pipes = new List<BsonDocument>();
            if (q != null) pipes.Add(new BsonDocument().Set("$match", q.ToBsonDocument()));
            pipes.AddRange(BsonHelper.ListDocFromFile(file, docReplace));
            var results = DataHelper.Aggregate(collection, pipes);
            return results;
        }

        internal static List<BsonDocument> Aggregate(string collectionName, BsonDocument docMatch = null, BsonDocument docGroup = null)
        {
            List<BsonDocument> pipelines = new List<BsonDocument>();
            if (docMatch != null) pipelines.Add(new BsonDocument { { "$match", docMatch } });
            if (docGroup == null) docGroup = new BsonDocument { { "_id", "Total" }, { "Count", new BsonDocument { { "$sum", 1 } } } };
            pipelines.Add(new BsonDocument { { "$group", docGroup } });
            return Aggregate(collectionName, pipelines);
        }

        //public static List<BsonDocument> Aggregate(string collectionName, List<BsonDocument> pipelines)
        //{
        //    //AggregateArgs args = new AggregateArgs();
        //    //args.Pipeline = pipelines;
        //    //List<BsonDocument> ret = GetDb().GetCollection(collectionName).Aggregate(args).ToList();
        //    return Aggregate(collectionName, pipelines);
        //}


        public static Dictionary<string, object>[] ToDictionaryArray(IEnumerable<BsonDocument> objs)
        {
            List<Dictionary<string, object>> ret = new List<Dictionary<string, object>>();
            foreach (var obj in objs)
            {
                ret.Add(obj.ToDictionary());
            }
            return ret.ToArray();
        }

        public static void Save(string collectionName, IEnumerable<object> datas,
            bool pushToMemory = false, string memoryId = null, bool overwriteMemory = false)
        {
            List<BsonDocument> memoryObjects = null;
            if (overwriteMemory == true || pushToMemory == true) memoryObjects = new List<BsonDocument>();
            if (pushToMemory == true)
            {
                if (memoryId == null)
                    memoryId = collectionName;
                memoryObjects = MemoryHelper.Populate(memoryId).Select(d => d.ToBsonDocument()).ToList();
            }
            var cols = GetDb().GetCollection<BsonDocument>(collectionName);
            foreach (BsonDocument data in datas)
            {
                cols.Save(data["_id"].ToString());
                if (pushToMemory == true)
                {
                    BsonDocument old = memoryObjects.FirstOrDefault(d => d["_id"] == data["_id"]);
                    if (old != null)
                    {
                        memoryObjects.Remove(old);
                    }
                    memoryObjects.Add(data);
                }
            }
        }

        public static void Save(string collectionName, BsonDocument data,
            bool pushToMemory = false, string memoryId = null, bool overwriteMemory = false)
        {
            BsonDocument[] docs = new BsonDocument[] { data };
            Save(collectionName, docs, pushToMemory, memoryId, overwriteMemory);
        }

        public static void Update(string collectionName,
            FilterDefinition<BsonDocument> where = null,
            UpdateDefinition<BsonDocument> update = null,
            UpdateOptions flag = null
           )
        {
            GetDb().GetCollection<BsonDocument>(collectionName).UpdateMany(where, update, flag);
        }




        #region ODBC
        //public static List<BsonDocument> OdbcRead(string ConnectionString, string CommandText,
        //   bool returnDoc = false,
        //   Func<BsonDocument, BsonDocument> FnProcess = null,
        //   bool saveToDb = false,
        //   string tableName = "",
        //   bool getSchema = false,
        //   int loadId = 0)
        //{
        //    List<BsonDocument> docs = new List<BsonDocument>();
        //    OdbcConnection conn = new OdbcConnection(ConnectionString);
        //    conn.ConnectionTimeout = 3600;

        //    //BsonDocument processingDoc = null;
        //    try
        //    {
        //        OdbcCommand cmd = new OdbcCommand(CommandText, conn);
        //        conn.Open();
        //        OdbcDataReader dr = cmd.ExecuteReader();

        //        if (getSchema)
        //        {
        //            for (int i = 0; i < dr.FieldCount; i++)
        //            {
        //                var doc = new BsonDocument();
        //                string fieldType = dr.GetFieldType(i).ToString();
        //                string fieldName = dr.GetName(i);
        //                doc.Set("FieldName", fieldName);
        //                doc.Set("FieldType", fieldType);
        //                docs.Add(doc);
        //            }
        //        }
        //        else
        //        {
        //            while (dr.Read())
        //            {
        //                var doc = new BsonDocument();
        //                for (int i = 0; i < dr.FieldCount; i++)
        //                {
        //                    string fieldType = dr.GetFieldType(i).ToString().ToLower();
        //                    string fieldName = dr.GetName(i);
        //                    if (dr.IsDBNull(i))
        //                    {
        //                        doc.Set(fieldName, BsonNull.Value);
        //                    }
        //                    else
        //                    {
        //                        if (fieldType.Contains("date"))
        //                        {
        //                            doc.Set(fieldName, Tools.ToUTC(dr.GetDateTime(i)));
        //                        }
        //                        else if (fieldType.Contains("bool"))
        //                        {
        //                            doc.Set(fieldName, dr.GetBoolean(i));
        //                        }
        //                        else if (fieldType.Contains("byte"))
        //                        {
        //                            doc.Set(fieldName, dr.GetByte(i));
        //                        }
        //                        else if (fieldType.Contains("int16"))
        //                        {
        //                            doc.Set(fieldName, dr.GetInt16(i));
        //                        }
        //                        else if (fieldType.Contains("int32"))
        //                        {
        //                            doc.Set(fieldName, dr.GetInt32(i));
        //                        }
        //                        else if (fieldType.Contains("int64"))
        //                        {
        //                            doc.Set(fieldName, dr.GetInt64(i));
        //                        }
        //                        else if (fieldType.Contains("time"))
        //                        {
        //                            doc.Set(fieldName, Tools.ToUTC(Convert.ToDateTime(dr.GetTime(i))));
        //                        }
        //                        else if (
        //                                fieldType.Contains("double") ||
        //                                fieldType.Contains("float") ||
        //                                fieldType.Contains("money") ||
        //                                fieldType.Contains("decimal")
        //                            )
        //                        {
        //                            doc.Set(fieldName, dr.GetDouble(i));
        //                        }
        //                        else
        //                        {
        //                            doc.Set(fieldName, dr.GetString(i));
        //                        }
        //                    }
        //                }
        //                if (FnProcess != null)
        //                {
        //                    doc = FnProcess(doc);
        //                }

        //                if (saveToDb && tableName != "")
        //                {
        //                    DataHelper.Save(tableName, doc);
        //                }
        //                if (returnDoc)
        //                {
        //                    docs.Add(doc);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (conn != null)
        //        {
        //            if (conn.State != ConnectionState.Closed)
        //            {
        //                conn.Close();
        //                conn = null;
        //            }
        //        }
        //        throw new Exception(Tools.PushException(e, true));
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //        {
        //            if (conn.State != ConnectionState.Closed)
        //            {
        //                conn.Close();
        //                conn = null;
        //            }
        //        }
        //    }
        //    return docs;
        //}

        //public static Task<List<BsonDocument>> OdbcReadAsync(string connectionString, string commandText,
        //    bool returnDoc = false,
        //    Func<BsonDocument, BsonDocument> fnProcess = null,
        //    bool saveToDb = false,
        //    string tableName = "",
        //    bool getSchema = false,
        //    int loadId = 0)
        //{
        //    return Task.Run(() => OdbcRead(connectionString, commandText, returnDoc, fnProcess, saveToDb, tableName, getSchema, loadId));
        //}
        #endregion

        #region Populate Grid
        //public static BsonDocument PopulateForGrid(string tableName,
        //   Dictionary<string, string> references = null,
        //   System.Web.HttpContext httpCtx = null)
        //{
        //    int take = 0;
        //    int skip = 0;
        //    if (httpCtx.Request.QueryString.AllKeys.FirstOrDefault(d => d.ToLower().Equals("take")) != null)
        //        take = Tools.ToInt32(httpCtx.Request.QueryString["take"]);
        //    if (httpCtx.Request.QueryString.AllKeys.FirstOrDefault(d => d.ToLower().Equals("skip")) != null)
        //        skip = Tools.ToInt32(httpCtx.Request.QueryString["skip"]);
        //    var orderby = httpCtx.Request.QueryString["sort[0][field]"];
        //    var orderdirection = httpCtx.Request.QueryString["sort[0][dir]"];
        //    SortByBuilder sort = orderby == null ?
        //                SortBy.Ascending(new string[] { "_id" }) :
        //                orderdirection.Equals("asc") ? SortBy.Ascending(new string[] { orderby }) :
        //                SortBy.Descending(new string[] { orderby });

        //    List<BsonDocument> pipes = new List<BsonDocument>();
        //    var qdoc = BsonHelper.Matches2QueryDoc(references);
        //    pipes.Add(new BsonDocument().Set("$match", qdoc));
        //    pipes.Add(BsonSerializer.Deserialize<BsonDocument>("{$group:{_id:0,count:{$sum:1}}}"));
        //    var count = DataHelper.Aggregate(tableName, pipes)[0].GetValue("count").ToDouble();

        //    return new
        //    {
        //        Count = count,
        //        Data = DataHelper.Populate(tableName, BsonHelper.Matches2Query(references), take: take, skip: skip, sort: sort)
        //    }.ToBsonDocument();
        //}
        #endregion

        #region MapReduce
        //public static MapReduceDocument MapReduce(string collectionname,
        //   string sfnMap, string sfnReduce,
        //   string sfnFinalize = null,
        //   MapReduceOutputOptions outputMode = MapReduceOutputOptions.Inline,
        //   string outputCollection = null,
        //   bool isOutputSharded = false,
        //   bool flatten = true)
        //{
        //    BsonJavaScript fnMap = new BsonJavaScript(sfnMap);
        //    BsonJavaScript fnReduce = new BsonJavaScript(sfnReduce);
        //    BsonJavaScript fnFinalize = sfnFinalize == null ? null : new BsonJavaScript(sfnFinalize);
        //    return MapReduce(collectionname, fnMap, fnReduce, fnFinalize, outputMode, outputCollection, isOutputSharded);
        //}

        //public static MapReduceDocument MapReduce(string collectionname,
        //    BsonJavaScript fnMap, BsonJavaScript fnReduce,
        //    BsonJavaScript fnFinalize = null,
        //    MapReduceOutputMode outputMode = MapReduceOutputMode.Inline,
        //    string outputCollection = null,
        //    bool isOutputSharded = false)
        //{
        //    if (!TableExist(collectionname)) return new MapReduceDocument
        //    {
        //        Count = 0,
        //        Result = new List<BsonDocument>()
        //    };

        //    var docs = new List<BsonDocument>();
        //    var coll = GetDb().GetCollection(collectionname);
        //    var mrArgs = new MapReduceArgs
        //    {
        //        MapFunction = fnMap,
        //        ReduceFunction = fnReduce
        //    };
        //    if (fnFinalize != null) mrArgs.FinalizeFunction = fnFinalize;
        //    mrArgs.OutputMode = outputMode;
        //    if (outputMode != MapReduceOutputMode.Inline)
        //    {
        //        if (outputCollection == null) throw new Exception("Collection name is mandatory for non-inline MapReduce");
        //        mrArgs.OutputCollectionName = outputCollection;
        //        mrArgs.OutputIsSharded = isOutputSharded;
        //    }
        //    var db = GetDb();
        //    db.RequestStart();
        //    var ret = db.GetCollection(collectionname).MapReduce(mrArgs);
        //    var err = db.GetLastError();
        //    if (err.Ok == false) throw new Exception(err.ErrorMessage);
        //    db.RequestDone();

        //    var results = ret.GetResults().ToList();
        //    return new MapReduceDocument
        //    {
        //        Count = ret.OutputCount,
        //        Result = docs
        //    };
        //}
        #endregion

        #region Delete
        public static void Delete(string collectionName, FilterDefinition<BsonDocument> q = null)
        {
            if (q == null)
                GetDb().GetCollection<BsonDocument>(collectionName).DeleteMany("{}");
            else
                GetDb().GetCollection<BsonDocument>(collectionName).DeleteMany(q);
        }

        //public static void Delete(string collectionName, string id)
        //{
        //    var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
        //    Delete(collectionName, q);
        //}

        //public static void Delete(string collectionName, int id)
        //{
        //    var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
        //    Delete(collectionName, q);
        //}

        //public static void Delete(string collectionName, DateTime id)
        //{
        //    var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
        //    Delete(collectionName, q);
        //}
        #endregion
    }
}
