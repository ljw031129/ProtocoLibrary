using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Models
{
    public class CommandJsonData
    {
        public string AddressType { get; set; }
        public string IpOrDomain { get; set; }
        public string WorkStatue { get; set; }
        public string TootalWorkTime { get; set; }
        public string WorkModel { get; set; }
        public Timing Timing { get; set; }
        public string PostionCount { get; set; }
        public string WorkTime { get; set; }
        //间隔时间-追车模式
        public string IntervalTime { get; set; }
    }
    public class Timing
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}
