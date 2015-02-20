using System;
using System.Collections;
using System.Collections.Generic;

namespace ProtocolUtils.Models
{
    public class geShiJson
    {
        public geShiJson() { }
        public string Name { get; set; }
        public int? StrSta { get; set; }
        public int? StrEnd { get; set; }
    }

    public class yuJinJson
    {
        public yuJinJson() { }
        public string Name { get; set; }
        public List<Hashtable> EarlyWarning { get; set; }
    }
    
    public class GpsEntity
    {
        public String ID { get; set; }
        public  const int ID_Pos = 0;
        public  const int ID_Length = 8;

        public String IsPos { get; set; }
        public String GPSMast { get; set; }
        public  const int IsPos_Pos = 26;
        public  const int IsPos_Length = 2;

        public String Pos { get; set; }

        public Decimal Plat { get; set; }
        public  const int Plat_Pos = 18;
        public  const int Plat_Length = 4;

        public Decimal Plog { get; set; }
        public  const int Plog_Pos = 22;
        public  const int Plog_Length = 4;

        public String GpsTime { get; set; }
        public  const int GpsTime_Pos = 54;
        public  const int GpsTime_Length = 12;

        public String Province { get; set; }
        public String caijishijian { get; set; }
        public String trackType { get; set; }
        public String TotalTime { get; set; }
        public String WorkState { get; set; }
        public String KeyState { get; set; }
        public String WkState { get; set; }//外壳
        public string Rtime { get; set; }
        public string SleepState { get; set; }

        public String Lock1 { get; set; }
        public String Lock2 { get; set; }
        public string Lock3 { get; set; }
        public string Lock4 { get; set; }
        public string controlAlarm { get; set; }

        public int isWorkS { get; set; }

        public int Num { get; set; }
        public double OldTotalTime { get; set; }//工作时间差
        public double DevPrimitiveWorkTime { get; set; }//设备回传的原始工作时间
        public bool IsTotalTime { get; set; }
    }

    public class AllEntity
    {
        public AllEntity() { }
        public AllEntity(int intNum)
        {
            DictALLEnti = new Dictionary<string, string>();
            for (int i = 1; i <= intNum; i++)
            {
                DictALLEnti.Add("A" + i, "");
            }
        }
        public Dictionary<string,string> DictALLEnti;
    }

    public class HistorInf
    {
        public String IsPos { get; set; }

        public String Pos { get; set; }

        public Decimal Plat { get; set; }

        public Decimal Plog { get; set; }

        public String GpsTime { get; set; }

        public String Province { get; set; }
        public String TotalTime { get; set; }

        public DateTime RTime { get; set; }

        public String WorkState { get; set; }

        public String caijishijian { get; set; }
    }
}
