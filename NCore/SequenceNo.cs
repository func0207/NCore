using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NCore
{
    public class SequenceNo
    {
        public string _id { get; set; }
        public string Title { get; set; }
        public int NextNo { get; set; }
        public string Format { get; set; }

        public ResultInfo Reset(int nextNo, string format = "")
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                NextNo = nextNo;
                if (format.Equals("") == false) Format = format;
                DataHelper.Save("SequenceNos", new BsonDocument[] { this.ToBsonDocument() });
                ri.Data = this;
            }
            catch (Exception e)
            {
                ri.PushException(e);
            }
            return ri;
        }

        public string ClaimAsString(string format = "", bool commit = true)
        {
            string ret = "";
            SequenceNo sn = DataHelper.Populate<SequenceNo>("SequenceNos", Query.EQ("_id", _id)).FirstOrDefault();
            if (sn != null)
            {
                NextNo = sn.NextNo;
            }
            if (String.IsNullOrEmpty(format)) format = Format;
            ret = (format.Equals("")) ? NextNo.ToString() :
                String.Format(format, NextNo);
            if (commit)
            {
                this.NextNo++;
                DataHelper.Save("SequenceNos", new BsonDocument[] { this.ToBsonDocument() });
            }
            return ret;
        }

        public int ClaimAsInt(bool commit = true)
        {
            int ret = 0;
            SequenceNo sn = DataHelper.Populate<SequenceNo>("SequenceNos", Query.EQ("_id", _id)).FirstOrDefault();
            if (sn != null)
            {
                NextNo = sn.NextNo;
            }
            ret = NextNo;
            if (commit)
            {
                this.NextNo++;
                DataHelper.Save("SequenceNos", new BsonDocument[] { this.ToBsonDocument() });
            }
            return ret;
        }

        public static SequenceNo Get(string id)
        {
            SequenceNo ret = DataHelper.Populate<SequenceNo>("SequenceNos", Query.EQ("_id", id)).FirstOrDefault();
            if (ret == null)
            {
                ret = new SequenceNo
                {
                    _id = id,
                    Title = id,
                    NextNo = 1,
                    Format = ""
                };
            }
            return ret;
        }
    }
}
