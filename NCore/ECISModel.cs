using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;

namespace NCore
{
    public interface IECISModel
    {
        void PostGet();
        string TableName { get; }
    }

    public abstract class ECISModel : IECISModel
    {
        private List<TagInfo> _tags;

        private DateTime _lastUpdate;

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set
            { _lastUpdate = Tools.ToUTC(value); }
        }


        public List<TagInfo> Tags
        {
            get
            {
                if (_tags == null) _tags = new List<TagInfo>();
                return _tags;
            }
        }
        public int IntId
        {
            get
            {
                return _id == null ? 0 : Tools.ToInt(_id.ToString());
            }
        }
        public string StringId
        {
            get
            {
                return _id == null ? "" : _id.ToString();
            }
        }

        public static List<T> Populate<T>() where T : IECISModel, new()
        {
            //TypeInfo ti = typeof(T).GetTypeInfo();
            //string localTableName = ti.GetProperty("TableName").GetValue(new T(), null).ToString();
            var ot = new T();
            string localTableName = ot.TableName;
            return DataHelper.Populate<T>(localTableName);
        }
        public static List<T> Populate<T>(
           FilterDefinition<BsonDocument> q = null, int take = 0, int skip = 0,
           string[] fields = null,
           SortDefinition<BsonDocument> sort = null)
            where T : IECISModel, new()
        {
            //TypeInfo ti = typeof(T).GetTypeInfo();
            //string localTableName = ti.GetProperty("TableName").GetValue(new T(), null).ToString();
            var ot = new T();
            string localTableName = ot.TableName;
            return sort == null ?
                DataHelper.Populate<T>(localTableName, q,
                    take: take, skip: skip, fields: fields) :
                DataHelper.Populate<T>(localTableName, q,
                    sort: sort,
                    take: take, skip: skip, fields: fields);
        }
        public virtual void PostGet()
        {
            //-- do something
        }

        public static T Get<T>(object _id) where T : IECISModel, new()
        {
            T ret = new T(); //default(T);
            string localTableName = ret.TableName;
            ret = DataHelper.Get<T>(localTableName, _id);
            if (ret != null) ret.PostGet();
            return ret;
        }
        public T Update<T>(BsonDocument updates, string tableName = "") where T : IECISModel, new()
        {
            if (tableName.Equals("")) tableName = TableName;
            //var update = new UpdateDefinitionBuilder<T>();
            //foreach (var k in updates.Elements)
            //{
            //    update =  update.AddToSet<T>(, k.Value);
            //}
            DataHelper.Update(tableName, prepareEqId(), updates);
            T ret = ECISModel.Get<T>(_id);
            return ret;
        }
        public static T Get<T>(
           FilterDefinition<BsonDocument> q = null,
           SortDefinition<BsonDocument> sort = null
            )
            where T : IECISModel, new()
        {
            T ret = new T();
            string localTableName = ret.TableName;
            //TypeInfo ti = typeof(T).GetTypeInfo();
            //string localTableName = ti.GetProperty("TableName").GetValue(new T(), null).ToString();
            ret = sort == null ?
                DataHelper.Get<T>(localTableName, q) :
                DataHelper.Get<T>(localTableName, q, sort);
            //ret.PostGet();
            if (ret != null) ret.PostGet();
            return ret;
        }

        public bool AddTag(TagInfo tag)
        {
            var existingTag = Tags.FirstOrDefault(d => d.TagType.Equals(tag.TagType) && d.Value.Equals(tag.Value));
            if (existingTag != null) return false;
            _tags.Add(tag);
            return true;
        }

        public bool RemoveTag(TagInfo tag)
        {
            var existingTag = Tags.FirstOrDefault(d => d.TagType.Equals(tag.TagType) && d.Value.Equals(tag.Value));
            if (existingTag == null) return false;
            _tags.Remove(existingTag);
            return true;
        }

        public object _id { get; set; }
        [BsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public abstract string TableName { get; }
        [BsonIgnoreIfNull]
        public string Title { get; set; }

        public virtual BsonDocument PreSave(BsonDocument doc, string[] references = null)
        {
            var tempId = doc.GetString("_id", "");
            if (tempId == "" || tempId == "0" || doc.Get("_id") == BsonNull.Value || tempId == null)
            {
                doc.Set("_id", SequenceNo.Get(this.TableName).ClaimAsInt());
            }
            return doc;
        }

        public virtual BsonDocument PostSave(BsonDocument doc, string[] references = null)
        {
            return doc;
        }

        public void Save(string tablename = "", Func<BsonDocument, BsonDocument> fnUpdate = null, bool runPreSave = true, bool runPostSave = true, string[] references = null)
        {
            if (tablename.Equals("")) tablename = TableName;
            this.LastUpdate = DateTime.Now;
            var doc = runPreSave ? PreSave(this.ToBsonDocument(), references) : this.ToBsonDocument();
            if (fnUpdate != null)
            {
                doc = fnUpdate(doc);
            }
            if (BsonHelper.HasElement(doc, "_t")) doc.RemoveElement(doc.GetElement("_t"));
            DataHelper.Save(tablename, doc);
            if (runPostSave) doc = PostSave(doc, references);
        }

        private FilterDefinition<BsonDocument> prepareEqId()
        {
            FilterDefinition<BsonDocument> q = null;
            if (_id.GetType() == typeof(DateTime))
            {
                return new FilterDefinitionBuilder<BsonDocument>().Eq("_id", (DateTime)this._id);
            }
            else if (_id.GetType() == typeof(int))
            {
                return new FilterDefinitionBuilder<BsonDocument>().Eq("_id", (int)this._id);
            }
            else
            {
                return new FilterDefinitionBuilder<BsonDocument>().Eq("_id", this._id.ToString());
            }
            return q;
        }

        public void Delete()
        {
            if (_id == null) return;
            PreDelete();
            if (_id.GetType() == typeof(DateTime))
            {
                var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", (DateTime)this._id);
                DataHelper.Delete(TableName, q);
            }
            else if (_id.GetType() == typeof(int))
            {
                var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", (int)this._id);
                DataHelper.Delete(TableName, q);
            }
            else
            {
                var q = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", this._id.ToString());
                DataHelper.Delete(TableName, q);

            }
            PostDelete();
        }

        public virtual void PreDelete()
        {
            //-- do nothing
        }

        public virtual void PostDelete()
        {
            //-- do nothing
        }
    }
}
