using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
//using Aspose.Cells;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace NCore
{
    public static class BsonHelper
    {

        static BsonHelper()
        {
            if (License.Validate("Global") == false) throw new Exception("ERR_INVALID_LICENSE");
        }

        public static T Deserialize<T>(BsonDocument d)
        {
            T ret = BsonSerializer.Deserialize<T>(d);
            return ret;
        }

        public static DateIsland GetDateInfo(this BsonDocument d, string field)
        {
            DateTime dt = BsonHelper.GetDateTime(d, field);
            return new DateIsland(dt);
        }

        public static List<BsonDocument> Normalize2Tree(this List<BsonDocument> docs,
            string idField,
            string parentField,
            Func<BsonDocument, BsonDocument> fnTransform,
            string itemELementIdTarget = "Items",
            string nodeParent = null)
        {
            List<BsonDocument> ret = new List<BsonDocument>();
            List<BsonDocument> items =
                nodeParent == null ?
                docs.Where(d => d.GetString(parentField).Equals("") || d.GetString(parentField).Equals("0")).ToList() :
                docs.Where(d => d.GetString(parentField).Equals(nodeParent)).ToList();
            foreach (var i in items)
            {
                var doc = fnTransform(i);
                var childs = Normalize2Tree(docs, idField, parentField, fnTransform, itemELementIdTarget, i.GetString(idField));
                if (childs.Count > 0)
                {
                    doc.Set(itemELementIdTarget, new BsonArray(childs));
                }
                ret.Add(doc);
            }
            return ret;
        }

        public static BsonDocument GetAsTree(this BsonDocument doc, string[] itemElementIdSources,
            Func<BsonDocument, BsonDocument> fnTransform,
            string itemELementIdTarget = "Items")
        {
            BsonDocument bdoc = new BsonDocument();
            bdoc = fnTransform(doc);
            var docItems = new List<BsonDocument>();
            foreach (var itemElementIdSource in itemElementIdSources)
            {
                if (doc.HasElement(itemElementIdSource) && doc.Get(itemElementIdSource).IsBsonArray)
                {
                    var sourceItems = doc.Get(itemElementIdSource).AsBsonArray;
                    foreach (var sourceItem in sourceItems)
                    {
                        var docItem = sourceItem.AsBsonDocument;
                        docItems.Add(fnTransform(docItem));
                    }
                }
            }
            bdoc.Set(itemELementIdTarget, new BsonArray(docItems));
            return bdoc;
        }

        public static BsonDocument FlattenMapReduceOutput(this BsonDocument doc)
        {
            BsonDocument ret = new BsonDocument();
            var idIsDoc = doc.GetValue("_id", 0).IsBsonDocument;
            var valueIsDoc = doc.GetValue("value", 0).IsBsonDocument;

            if (idIsDoc)
            {
                BsonDocument docId = doc.GetValue("_id").AsBsonDocument;
                foreach (var el in docId.Elements)
                {
                    ret.Set(el.Name, BsonHelper.Get(docId, el.Name));
                }
            }
            else
            {
                ret.Set("_id", doc.GetValue("_id"));
            }

            if (valueIsDoc)
            {
                BsonDocument docValue = doc.GetValue("value").AsBsonDocument;
                foreach (var el in docValue.Elements)
                {
                    ret.Set(el.Name, BsonHelper.Get(docValue, el.Name));
                }
            }
            else
            {
                ret.Set("value", doc.GetValue("value", 0));
            }

            return ret;
        }

        public static bool HasElement(this BsonDocument doc, string elementId)
        {
            return doc.Elements.FirstOrDefault(d => d.Name.ToLower().Equals(elementId.ToLower())) != null;
        }
        public static List<BsonDocument> Unwind(IEnumerable<BsonDocument> sources,
            string arrayIndexId,
            string parentIdType = "",
            IEnumerable<string> elementsToCopy = null,
            Func<BsonDocument, BsonDocument> fnTransform = null)
        {
            List<BsonDocument> ret = new List<BsonDocument>();
            if (elementsToCopy == null) elementsToCopy = new string[] { };
            foreach (var doc in sources)
            {
                List<BsonDocument> docs = doc.GetValue(arrayIndexId).AsBsonArray.Select(d => d.ToBsonDocument()).ToList();
                foreach (var child in docs)
                {
                    switch (parentIdType.ToLower())
                    {
                        case "string":
                            child.Set("_parentid", doc.GetValue("_id", "").ToString());
                            break;
                        case "int32":
                            child.Set("_parentid", doc.GetValue("_id", 0).ToInt32());
                            break;
                        case "int64":
                            child.Set("_parentid", doc.GetValue("_id", 0).ToInt64());
                            break;
                        case "double":
                            child.Set("_parentid", doc.GetValue("_id", 0).ToDouble());
                            break;
                        case "datetime":
                            child.Set("_parentid", doc.GetValue("_id", Tools.DefaultDate).ToUniversalTime());
                            break;
                        default:
                            child.Set("_parentid", doc.GetValue("_id"));
                            break;
                    }
                    foreach (var elementid in elementsToCopy)
                    {
                        child.Set(elementid, doc.GetValue(elementid));
                    }
                    if (fnTransform == null)
                    {
                        ret.Add(child);
                    }
                    else
                    {
                        ret.Add(fnTransform(child));
                    }
                }
            }
            return ret;
        }

        public static List<BsonDocument> Find(IEnumerable<BsonDocument> docs, BsonDocument doc, IEnumerable<string> fields)
        {
            List<BsonDocument> ret = null;
            List<BsonDocument> sources = docs.ToList();
            int fieldCount = fields.Count();
            int i = 0; bool loop = true;
            while (loop && i < fieldCount)
            {
                string field = fields.ElementAt(i);
                ret = sources.Where(d => BsonHelper.GetString(d, field).Equals(BsonHelper.GetString(doc, field))).ToList();
                if (ret == null || ret.Count == 0)
                {
                    loop = false;
                }
                else
                {
                    sources = ret;
                }
                i++;
            }
            if (ret == null) ret = new List<BsonDocument>();
            return ret;
        }

        public static void CopyElements(this BsonDocument source, ref BsonDocument target, IEnumerable<string> excludes = null)
        {
            if (excludes == null) excludes = new string[] { };
            string[] fields = source.Elements.Where(d => excludes.Contains(d.Name) == false).Select(d => d.Name).ToArray();
            int fieldCount = fields.Count();
            foreach (var field in fields)
            {
                target.Set(field, source.GetValue(field));
            }
        }

        public static string GetString(this BsonDocument doc, string fieldName, string defaultValue = "")
        {
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                var val = doc.GetValue(fieldName, "");
                string strVal = val.ToString();
                if (String.IsNullOrEmpty(strVal) && defaultValue != "") strVal = defaultValue;
                return strVal;
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                return BsonHelper.GetString(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
            }
        }

        public static Byte GetByte(this BsonDocument doc, string fieldName, Byte defaultValue = 0)
        {
            return (Byte)GetInt32(doc, fieldName, defaultValue);
        }

        public static Int16 GetInt16(this BsonDocument doc, string fieldName, Int16 defaultValue = 0)
        {
            return (Int16)GetInt32(doc, fieldName, defaultValue);
        }

        public static Int32 GetInt32(this BsonDocument doc, string fieldName, Int32 defaultValue = 0)
        {
            //return doc.GetValue(fieldName, defaultValue).ToInt32();
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                var val = doc.GetValue(fieldName, defaultValue);
                return val.IsNumeric ? val.ToInt32() : defaultValue;
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                return BsonHelper.GetInt32(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
            }
        }

        public static Int64 GetInt64(this BsonDocument doc, string fieldName, Int64 defaultValue = 0)
        {
            //return doc.GetValue(fieldName, defaultValue).ToInt64();
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                var val = doc.GetValue(fieldName, defaultValue);
                return val.IsNumeric ? val.ToInt64() : defaultValue;
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                return BsonHelper.GetInt64(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
            }
        }

        public static Double GetDouble(this BsonDocument doc, string fieldName, Double defaultValue = 0)
        {
            try
            {
                double ret = 0;
                string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length == 1)
                {
                    var val = doc.GetValue(fieldName);
                    if (val.IsNumeric)
                    {
                        ret = val.ToDouble();
                    }
                    else
                    {
                        var str = "";
                        var source = val.ToString();
                        var allowed = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '-' };
                        foreach (char c in source)
                        {
                            if (allowed.Contains(c))
                            {
                                str += c;
                            }
                        }
                        ret = Tools.ToDouble(str);
                    }
                }
                else
                {
                    var newindex = String.Join(".", fields.Skip(1).ToArray());
                    ret = BsonHelper.GetDouble(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
                }
                return ret;
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public static BsonValue Get(this BsonDocument doc, string fieldName)
        {
            try
            {
                string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length == 1)
                {
                    return doc.GetValue(fieldName);
                }
                else
                {
                    var newindex = String.Join(".", fields.Skip(1).ToArray());
                    return BsonHelper.Get(doc.GetValue(fields[0]).AsBsonDocument, newindex);
                }
            }
            catch (Exception e)
            {
                return BsonNull.Value;
            }
        }

        public static Decimal GetDecimal(this BsonDocument doc, string fieldName, Decimal defaultValue = 0)
        {
            return (Decimal)GetDouble(doc, fieldName, (Double)defaultValue);
        }

        public static Boolean GetBool(this BsonDocument doc, string fieldName, Boolean defaultValue = false)
        {
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                var value = doc.GetValue(fieldName, defaultValue);
                return value.IsBoolean ? value.ToBoolean() : defaultValue;
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                return BsonHelper.GetBool(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
            }
        }

        public static BsonDocument GetDoc(this BsonDocument doc, string fieldName, BsonDocument defaultValue = null)
        {
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                var value = doc.GetValue(fieldName, defaultValue);
                return value.IsBsonDocument ? value.ToBsonDocument() : defaultValue;
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                return BsonHelper.GetDoc(doc.GetValue(fields[0]).AsBsonDocument, newindex, defaultValue);
            }
        }

        public static DateTime GetDateTime(this BsonDocument doc, string fieldName, bool dateOnly = false)
        {
            DateTime def = Tools.DefaultDate;
            DateTime dt = def;
            string[] fields = fieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 1)
            {
                dt = Tools.DefaultDate;
                var val = doc.GetValue(fieldName, dt);
                if (val.IsValidDateTime)
                {
                    dt = val.ToUniversalTime();
                }
                else
                {
                    dt = Tools.ToDateTime(val.ToString(), dateOnly);
                }
            }
            else
            {
                var newindex = String.Join(".", fields.Skip(1).ToArray());
                dt = BsonHelper.GetDateTime(doc.GetValue(fields[0]).AsBsonDocument, newindex, dateOnly);
            }
            if (dateOnly) dt = Tools.ToDateTime(String.Format("{0:dd-MMM-yyyy}", dt));
            return dt;
        }

        public static List<BsonDocument> ListDocFromFile(string file, BsonDocument replaceDoc = null)
        {
            List<BsonDocument> ret = new List<BsonDocument>();
            StreamReader sr = new StreamReader(file);
            string srOut = sr.ReadToEnd();
            if (replaceDoc != null)
            {
                foreach (var el in replaceDoc.Elements)
                {
                    srOut = srOut.Replace(el.Name, replaceDoc.GetValue(el.Name, "").ToString());
                }
            }
            sr.Close();
            ret = BsonSerializer.Deserialize<List<BsonDocument>>(srOut);
            return ret;
        }

        //public static IMongoQuery Matches2Query(Dictionary<string, string> matches, Func<string, string> fnGetFieldName = null)
        //{
        //    List<IMongoQuery> queries = new List<IMongoQuery>();
        //    if (matches == null) matches = new Dictionary<string, string>();
        //    foreach (var key in matches.Keys)
        //    {
        //        if (matches[key].Trim() != "")
        //        {
        //            List<Object> bvals = new List<object>();
        //            List<string> values = matches[key].Split(Tools.SplitDelimeter, StringSplitOptions.RemoveEmptyEntries).ToList();
        //            foreach (var value in values)
        //            {
        //                bvals.Add(value);
        //                if (Tools.IsNumber(value)) bvals.Add(Tools.ToDouble(value));
        //                if (Tools.IsDate(value)) bvals.Add(Tools.ToDateTime(value));
        //            }
        //            var bsonValues = new BsonArray(bvals);
        //            string fieldName = fnGetFieldName == null ? key : fnGetFieldName(key);
        //            if (fieldName != "") queries.Add(Query.In(fieldName, bsonValues));
        //        }
        //    }
        //    IMongoQuery q = (queries.Count == 0) ? Query.Null : Query.And(queries);
        //    return q;
        //}

        //public static BsonDocument Matches2QueryDoc(Dictionary<string, string> matches, Func<string, string> fnGetFieldName = null)
        //{
        //    var q = Matches2Query(matches, fnGetFieldName);
        //    return (q == null) ? new BsonDocument() : q.ToBsonDocument();
        //}

        public static BsonDocument ChangeFieldType(this BsonDocument doc, string fieldId, string type, string format = "{0}")
        {
            if (doc.GetValue(fieldId).IsString == false &&
                type.ToLower() != "string" &&
                (!doc.GetValue(fieldId).IsBsonNull)) return doc;

            var valueStr = doc.GetValue(fieldId).IsBsonNull ? "" : doc.GetValue(fieldId).ToString();
            switch (type.ToLower())
            {
                case "string":
                    if (doc.GetValue(fieldId).IsBsonNull)
                    {
                        doc.Set(fieldId, "");
                    }
                    else
                    {
                        if (Tools.IsBool(valueStr))
                        {
                            doc.Set(fieldId, String.Format(format, doc.GetValue(fieldId).ToBoolean()));
                        }
                        else if (doc.GetValue(fieldId).IsValidDateTime)
                        {
                            doc.Set(fieldId, String.Format(format, doc.GetValue(fieldId).ToUniversalTime()));
                        }
                        else doc.Set(fieldId, String.Format(format, doc.GetValue(fieldId).ToDouble()));
                    }
                    break;

                case "int16":
                    if (!Tools.IsNumber(valueStr))
                    {
                        doc.Set(fieldId, 0);
                    }
                    else
                    {
                        doc.Set(fieldId, Tools.ToInt16(doc.GetValue(fieldId).AsString));
                    }
                    break;

                case "int32":
                    if (!Tools.IsNumber(valueStr))
                    {
                        doc.Set(fieldId, 0);
                    }
                    else
                    {
                        doc.Set(fieldId, Tools.ToInt32(doc.GetValue(fieldId).AsString));
                    }
                    break;

                case "int64":
                    if (!Tools.IsNumber(valueStr))
                    {
                        doc.Set(fieldId, 0);
                    }
                    else
                    {
                        doc.Set(fieldId, Tools.ToInt64(doc.GetValue(fieldId).AsString));
                    }
                    break;

                case "double":
                    if (!Tools.IsNumber(valueStr))
                    {
                        doc.Set(fieldId, 0);
                    }
                    else
                    {
                        doc.Set(fieldId, Tools.ToDouble(doc.GetValue(fieldId).AsString));
                    }
                    break;

                case "datetime":
                    if (!Tools.IsDate(valueStr))
                    {
                        doc.Set(fieldId, Tools.DefaultDate);
                    }
                    else
                    {
                        doc.Set(fieldId, Tools.ToDateTime(doc.GetValue(fieldId).AsString));
                    }
                    break;

                case "timestamp":
                    doc.Set(fieldId, Tools.TimeStampStr2Time(doc.GetValue(fieldId).AsString));
                    break;
            }
            return doc;
        }
    }
}
