using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Models
{
    public class PostionModel
    {
        //判断当前上传数据的类型---定位数据/状态数据
        public string DataType { get; set; }
        //终端编号
        public string DevId { get; set; }
        //回传时间
        public string Rtime { get; set; }
        //定位时间
        public string Ptime { get; set; }
        public string AccStatus { get; set; }
        //定位状态-是否定位
        public string IsPos { get; set; }
        //定位模式-实时/差分定位
        public string PostMode { get; set; }
        //纬度
        public string Lat { get; set; }
        //经度
        public string Lng { get; set; }
        //速度
        public string Gspeed { get; set; }
        //方向
        public string GsDirection { get; set; }
        //基站数据
        public string MCC { get; set; }
        public string MNC { get; set; }
        public string LAC { get; set; }
        public string CID { get; set; }
        //是否为东经
        public string East { get; set; }
        //是否为北纬
        public string North { get; set; }
        //区分实时还是补发数据
        public string SupplyData { get; set; }
        //定位类型，实时/差分
        public string GpsType { get; set; }
    }
}
