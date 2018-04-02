using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

using MongoDB.Bson;
using MongoDB.Driver;

namespace NCore
{
    public class DateIsland
    {
        public DateIsland()
        {

        }

        public DateIsland(DateTime date)
        {
            DateId = date;
            this.RecalcDate();
        }

        public DateTime _id
        {
            get
            {
                return DateId;
            }
        }
        public DateTime DateId { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Qtr { get; set; }

        private int _qtrId, _monthId, _weekId;

        public int QtrId
        {
            get
            {
                if (_qtrId == 0) _qtrId = Year * 100 + Qtr;
                return _qtrId;
            }
            set
            {
                _qtrId = value;
            }
        }
        public int MonthId
        {
            get
            {
                if (_monthId == 0) _monthId = Year * 100 + Month;
                return _monthId;
            }
            set
            {
                _monthId = value;
            }
        }
        public int WeekId
        {
            get
            {
                if (_weekId == 0) _weekId = Year * 100 + Week;
                return _weekId;
            }
            set
            {
                _weekId = value;
            }
        }
        public DateTime DateOnly
        {
            get
            {
                return Convert.ToDateTime(String.Format("{0:dd-MMM-yyyy}", DateId));
            }
        }

        public DateTime TimeOnly
        {
            get
            {
                return Convert.ToDateTime(String.Format("{0:hh:mm:ss}", DateId));
            }
        }

        public static List<DateIsland> Populate(DateTime from, DateTime to)
        {
            from = Tools.ToUTC(from);
            to = Tools.ToUTC(to).AddDays(1).AddMilliseconds(-1);
            return AddDates(null, new DateTime[] { from, to });
        }

        //public static List<DateIsland> Populate(int[] weeks = null, int[] months = null, int[] quarters = null, int[] years = null)
        //{
        //    List<IMongoQuery> qs = new List<IMongoQuery>();
        //    if (weeks != null && weeks.Count() > 0) qs.Add(Query.In("WeekId", new BsonArray(weeks)));
        //    if (months != null && months.Count() > 0) qs.Add(Query.In("MonthId", new BsonArray(months)));
        //    if (quarters != null && quarters.Count() > 0) qs.Add(Query.In("QuarterId", new BsonArray(quarters)));
        //    if (years != null && years.Count() > 0) qs.Add(Query.In("Year", new BsonArray(years)));
        //    return DataHelper.Populate<DateIsland>("DateIsland", qs.Count == 0 ? null : Query.And(qs)).OrderBy(d => d.DateId).ToList();
        //}

        public static List<DateIsland> AddDates(List<DateIsland> ret, IEnumerable<DateTime> sources)
        {
            if (ret == null) ret = new List<DateIsland>();
            List<DateTime> dates = sources.Where(d => d.Year.CompareTo(1901) > 0).GroupBy(d => d).Select(d => d.Key).ToList();
            var existDates = ret.Where(d => d.DateId.CompareTo(dates.Min()) >= 0 && d.DateId.CompareTo(dates.Max()) <= 0);
            DateTime dtMin = new DateTime(sources.Min().Ticks, DateTimeKind.Utc);
            DateTime dtMax = new DateTime(sources.Max().Ticks, DateTimeKind.Utc);
            DateTime dt = dtMin;
            while (dt.CompareTo(dtMax) <= 0)
            {
                DateIsland dtExist = existDates.FirstOrDefault(d => d.DateId.Equals(dt));
                if (dtExist == null)
                {
                    dtExist = new DateIsland { DateId = dt };
                    dtExist.RecalcDate();
                    ret.Add(dtExist);
                }
                dt = dt.AddDays(1);
            }
            return ret;
        }

        public void RecalcDate()
        {
            DateId = Tools.ToUTC(DateId);
            Month = DateId.Month;
            Year = DateId.Year;
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            Week = cal.GetWeekOfYear(DateId, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            if (Week >= 50 && Month == 1) Week = 0;
            Qtr = (int)Math.Ceiling((Decimal)DateId.Month / (Decimal)3.0);
        }
    }
}
