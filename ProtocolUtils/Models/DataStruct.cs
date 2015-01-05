using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils
{
    /// <summary>
    /// 上海市新能源汽车公共数据采集0TA数据协议结构类
    /// </summary>
    public class DataStruct
    {
        public DataStruct()
        {
            MarkCode = "7E";
            StartCode = "2323";
            EncryptionType = 0x00;
            VINCode = new byte[17];
        }
        public string MarkCode { get; set; }
        public string  StartCode { get; set; }
        public byte CmdCode { get; set; }
        public byte  AnswerCode { get; set; }
        public byte[] SerialNo { get; set; }
        public byte[] VINCode { get; set; }
        public byte EncryptionType { get; set; }
        public byte[] DataLength { get; set; }
        public byte[] DataByte { get; set; }
        public string BCCStr { get; set; }
    }
}
