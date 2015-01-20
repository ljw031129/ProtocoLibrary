using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Models
{
    public class StatueModel
    {
        //判断当前上传数据的类型---定位数据/状态数据
        public string DataType { get; set; }
        public string Rtime { get; set; }
        public string DevId { get; set; }
        //电压等级
        public string Voltage { get; set; }
        //GSM信号轻度
        public string GsmSignal { get; set; }
        //油路状态
        public string OilSt { get; set; }
        //定位状态
        public string IsPos { get; set; }
        //报警状态 
        public string AlarmSt { get; set; }
        //已接电源充电
        public string ChargeSt { get; set; }
        //ACC状态
        public string AccSt { get; set; }
        //防御状态
        public string DefenseSt { get; set; }
    }
}
