using ProtocolUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Protocols
{
    public class CommandPackageUntils
    {
        public static string BuildCommandString(CommandJsonData cd)
        {
            string setValHex = "FFFFFFFFFF4D010200000000";
            //var type = 0;
            //IP或域名选择
            switch (cd.AddressType)
            {
                case "00":
                    setValHex += "00";
                    string addr = "";
                    setValHex += addr.PadLeft(58, '0');
                    break;
                case "01":
                    //IP转换
                    var re = cd.IpOrDomain.Split('.');
                    if (re.Length == 4)
                    {
                        setValHex += "01";
                        string address = Convert.ToString(Convert.ToInt32(re[0]), 16).PadLeft(2, '0').ToUpper() +
                                    Convert.ToString(Convert.ToInt32(re[1]), 16).PadLeft(2, '0').ToUpper() +
                                    Convert.ToString(Convert.ToInt32(re[2]), 16).PadLeft(2, '0').ToUpper() +
                                    Convert.ToString(Convert.ToInt32(re[3]), 16).PadLeft(2, '0').ToUpper();
                        setValHex += address.PadLeft(58, '0');//位数不够补齐29位
                    }

                    break;
                case "02":
                    //域名转换
                    setValHex += "02";
                    string yuming = Byte2Hex.Bytes2HexString(System.Text.Encoding.Default.GetBytes(cd.IpOrDomain));
                    setValHex += yuming.PadLeft(58, '0');//位数不够补齐29位
                    break;
            }
            //端口号
            setValHex += Convert.ToString(Convert.ToInt32(cd.IpOrDomain.Split(':')[1]), 16).PadLeft(4, '0').ToUpper();
            //北京时间默认
            setValHex += "000000000000";
            //激活状态
            switch (cd.WorkStatue)
            {
                case "00":
                    setValHex += "00";
                    break;
                case "01"://测试
                    setValHex += "01";
                    break;
                case "02"://待激活
                    setValHex += "02";
                    break;
                case "03"://已激活
                    setValHex += "03";
                    break;
                default:
                    setValHex += "00";
                    break;
            }
            //模式
            switch (cd.WorkModel)
            {
                case "01"://标准
                    //模式，工作，休眠（间隔时间），定位时间，间隔时间
                    setValHex += "01";
                    setValHex += Convert.ToString(Convert.ToInt32(cd.WorkTime), 16).PadLeft(2, '0').ToUpper();
                    setValHex += Convert.ToString(1440 / Convert.ToInt32(cd.PostionCount), 16).PadLeft(4, '0').ToUpper();
                    if (cd.Timing.value == "0")
                    {
                        setValHex += "FFFF";
                    }
                    else
                    {
                        setValHex += Convert.ToString(Convert.ToInt32(cd.Timing.value), 16).PadLeft(4, '0').ToUpper();
                    }
                    setValHex += "0000";
                    break;
                case "02"://精准
                    //模式，工作，休眠（间隔时间），定时时间，间隔时间
                    setValHex += "02";
                    setValHex += Convert.ToString(Convert.ToInt32(cd.WorkTime), 16).PadLeft(2, '0').ToUpper();
                    setValHex += Convert.ToString(1440 / Convert.ToInt32(cd.PostionCount), 16).PadLeft(4, '0').ToUpper();
                    if (cd.Timing.value == "0")
                    {
                        setValHex += "FFFF";
                    }
                    else
                    {
                        setValHex += Convert.ToString(Convert.ToInt32(cd.Timing.value), 16).PadLeft(4, '0').ToUpper();
                    }
                    setValHex += "0000";
                    break;
                case "03"://追车
                    //模式，工作，休眠（间隔时间），
                    setValHex += "03" + "00" + "0000";
                    //定时时间，
                    if (cd.Timing.value == "0")
                    {
                        setValHex += "FFFF";
                    }
                    else
                    {
                        setValHex += Convert.ToString(Convert.ToInt32(cd.Timing.value), 16).PadLeft(4, '0').ToUpper();
                    }
                    //间隔时间
                    setValHex += Convert.ToString(Convert.ToInt32(cd.IntervalTime), 16).PadLeft(4, '0').ToUpper();
                    break;
                case "00"://不设置
                    //模式，工作，休眠（间隔时间），定时时间，间隔时间
                    setValHex += "00";
                    setValHex += Convert.ToString(Convert.ToInt32(cd.WorkTime), 16).PadLeft(2, '0').ToUpper();
                    setValHex += Convert.ToString(Convert.ToInt32(1440 / Convert.ToInt32(cd.PostionCount)), 16).PadLeft(4, '0').ToUpper();
                    if (cd.Timing.value == "0")
                    {
                        setValHex += "FFFF";
                    }
                    else
                    {
                        setValHex += Convert.ToString(Convert.ToInt32(cd.Timing.value), 16).PadLeft(4, '0').ToUpper();
                    }
                    setValHex += "0000";
                    break;
                default:
                    //gongzuo模式，工作shijian，休眠（间隔时间），定时时间，间隔时间
                    setValHex += "00" + "00" + "0000" + "FFFF" + "0000";
                    break;
            }
            //累计工时
            if (cd.TootalWorkTime == "00")
            {
                setValHex += "FFFFFFFF";
            }
            else
            {
                setValHex += Convert.ToString(Convert.ToInt32(cd.TootalWorkTime), 16).PadLeft(8, '0').ToUpper();
            }
            //备用字节20
            setValHex += "0000000000000000000000000000000000000000";
            return setValHex;
        }
    }
}
