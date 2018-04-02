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

            #region License
            NCore.License.ConfigFolder = System.IO.Directory.GetCurrentDirectory() + @"\License";
            NCore.License.LoadLicense();
            #endregion

            var fields = new List<string>() { "WellName", "RigName" };
            var qType1 = new FilterDefinitionBuilder<BsonDocument>().Eq("WellName", "APPO AC001");

            BsonDocument qType2 = new BsonDocument {
                            { "WellName" , "APPO AC001" },
                            { "RigName" , "UA CAT B VESSEL" }
                          };

            var qType3 = @"{ 'WellName': 'APPO AC001' }";
            var sort = new SortDefinitionBuilder<BsonDocument>().Descending("RigName");
            var result2 = DataHelper.Populate("WEISWellActivities", q: qType3, take: 10,  fields: fields.ToArray(), sort:sort);
            Console.WriteLine(result2[0].GetElement("WellName").Value);
            Console.ReadLine();
        }
    }
}
