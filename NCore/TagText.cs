using System;
using System.Collections.Generic;
using System.Text;

namespace NCore
{
    public class TagInfo
    {
        public TagInfo()
        {
            TagType = "General";
            Value = "";
        }
        public string TagType { get; set; }
        public object Value { get; set; }
    }
    public class TagText
    {
        public string TagTitle { get; set; }
        private string _value;
        public string TagValue
        {
            get
            {
                if (_value == null) _value = "";
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public bool DefaultTag { get; set; }
    }

    public class TagDate
    {
        private DateTime _last, _executeStart, _executeEnd, _next;
        public string TagTitle { get; set; }
        public DateTime Last
        {
            get { return _last; }
            set { _last = Tools.ToUTC(value); }
        }
        public DateTime ExecuteStart
        {
            get { return _executeStart; }
            set { _executeStart = Tools.ToUTC(value); }
        }
        public DateTime ExecuteEnd
        {
            get { return _executeEnd; }
            set { _executeEnd = Tools.ToUTC(value); }
        }
        public DateTime Next
        {
            get { return _next; }
            set { _next = Tools.ToUTC(value); }
        }
        public bool DefaultTag { get; set; }
    }
}
