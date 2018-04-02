using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Collections.Generic;

namespace NCore
{
    public static class MemoryHelper
    {
        private static Dictionary<string, MemoryObject> _datas;

        public static Dictionary<string, MemoryObject> MemoryData
        {
            get
            {
                if (_datas == null) _datas = new Dictionary<string, MemoryObject>();
                return _datas;
            }
        }

        public static void Clear(string key, string group = "")
        {
            key = BuildKey(group, key);
            Clear(new string[] { key });
        }

        private static string BuildKey(string key, string group = "")
        {
            return String.Format("{0}_{1}", group, key);
        }

        public static void Clear(string[] keys = null, string group = "")
        {
            if (keys == null || keys.Count() == 0)
                keys = MemoryData.Keys.Select(d => d).ToArray();
            else
                keys = keys.Select(d => BuildKey(group, d)).ToArray();
            foreach (string k in keys)
            {
                if (MemoryData.ContainsKey(k)) _datas.Remove(k);
            }
        }

        public static bool HasMemory(string key)
        {
            return MemoryData.ContainsKey(key);
        }

        public static MemoryObject Get(string key, string group = "")
        {
            key = BuildKey(group, key);
            MemoryObject ret = new MemoryObject();
            if (MemoryData.ContainsKey(key))
            {
                ret = MemoryData[key];
            }
            return ret;
        }

        public static List<MemoryObject> Populate(string group = "")
        {
            List<MemoryObject> ret = new List<MemoryObject>();
            var ds = MemoryData;
            var keys = MemoryData.Keys.Where(d => d.StartsWith(BuildKey(group, ""))).Select(d => d);
            foreach (string k in keys)
            {
                ret.Add(ds[k]);
            }
            return ret;
        }

        public static void Save(string key, MemoryObject obj, string group = "")
        {
            key = BuildKey(group, key);
            if (MemoryData.ContainsKey(key))
            {
                MemoryData[key] = obj;
            }
            else
            {
                MemoryData.Add(key, obj);
            }
        }

        public static void Save(this object obj, string key, string group = "")
        {
            var mobj = new MemoryObject
            {
                Group = group,
                Key = key,
                Value = obj,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                LastUsed = DateTime.Now
            };
            Save(key, mobj, group);
        }

        public static object[] Values(this IEnumerable<MemoryObject> objs)
        {
            return objs.Select(d => d.Value).ToArray();
        }
    }

    public class MemoryObject
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime LastUsed { get; set; }
    }
}
