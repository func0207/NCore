using System;
using NCore;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;

namespace NCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {

            #region License -- NetCore still not working with 256 Rijendael Decrpyt -- temp remark
            //NCore.License.ConfigFolder = System.IO.Directory.GetCurrentDirectory() + @"\License";
            //NCore.License.LoadLicense();
            #endregion

            #region Test Filter
            var fields = new List<string>() { "_id", "LastUpdate", "Name" };
            var qType1 = new FilterDefinitionBuilder<BsonDocument>().Eq("Name", "ARMADA HWU");

            BsonDocument qType2 = new BsonDocument {
                            { "Name" , "ARMADA HWU" },
                            { "isOfficeLocation" , false }
                          };

            var qType3 = @"{ 'Name': 'ARMADA HWU' }";
            var sort = new SortDefinitionBuilder<BsonDocument>().Descending("RigName");
            var result2 = DataHelper.Populate("WEISRigNames", q: qType3, take: 10,  fields: fields.ToArray(), sort:sort);
            #endregion

            #region Test Save (upsert true)
            DataHelper.Save("WEISRigNames", new BsonDocument().Set("_id", "YOGA").Set("Name", "YOGA RIG"));
            DataHelper.Save("WEISRigNames", new BsonDocument().Set("_id", "YOGA").Set("Name", "YOGA RIG 111"));
            #endregion


            #region Test Delete
            qType1 = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", "YOGA");
            DataHelper.Delete("1111111");
            #endregion

            //Console.WriteLine(result2[0].GetElement("WellName").Value);
            Console.ReadLine();
        }
    }
}
