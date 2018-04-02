using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore
{
    [MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    public class Config : ECISModel
    {
        public override string TableName
        {
            get { return "SharedConfigTable"; }
        }

        public override void PostGet()
        {
            base.PostGet();
        }

        public string ConfigModule { get; set; }
        public object ConfigValue { get; set; }

        public static object GetConfigValue(string _id, object defaultValue = null)
        {
            object ret = defaultValue;
            Config cfg = Config.Get<Config>(_id);
            if (cfg != null)
            {
                ret = cfg.ConfigValue;
                if (ret == null) ret = defaultValue;
            }
            return ret;
        }
    }
}
