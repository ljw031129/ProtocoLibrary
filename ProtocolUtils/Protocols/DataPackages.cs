using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Protocols
{
    public static class DataPackages
    {
        /// <summary>
        /// 打包发送的Byte数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="bytesLength"></param>
        /// <returns></returns>
        public static string BuildRequestString(DataStruct ds, int bytesLength)
        {
            byte[] sendBytes = new byte[bytesLength];
            sendBytes[0] = ds.CmdCode;
            sendBytes[1] = ds.AnswerCode;
            ds.SerialNo.CopyTo(sendBytes, 2);
            ds.VINCode.CopyTo(sendBytes, 4);
            sendBytes[21] = ds.EncryptionType;
            ds.DataLength.CopyTo(sendBytes, 22);
            //数据单元不为空拷贝数据部分
            if (ds.DataByte != null)
            {
                ds.DataByte.CopyTo(sendBytes, 24);
            }
            //校验位计算           
            string sendStr = Byte2Hex.Bytes2HexString(sendBytes) + BCCCheck.GetBcc(sendBytes).ToString("X").ToUpper().PadLeft(2, '0');
            return EscapeReduction.EscapeCode(sendStr);
        }
        public static string PickResponseAnswerString(byte[] requestByte)
        {
            string requestStr = Byte2Hex.Bytes2HexString(requestByte);
            string result = string.Empty;
            byte[] requestEsByte = Byte2Hex.HexString2Bytes(EscapeReduction.ReductionCode(requestStr));
           
                switch (requestEsByte[1])
                {
                    case 1:
                        result = "成功";
                        break;
                    case 2:
                        result = "修改错误";
                        break;
                    case 3:
                        result = "IP地址不正确";
                        break;
                    case 4:
                        result = "用户没注册";
                        break;
                    case 5:
                        result = "密码错误";
                        break;
                    case 254:
                        result = "命令";
                        break;                   
                }
           
            return result;
        }
    }
}
