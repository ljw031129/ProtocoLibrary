using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolUtils
{
    /**
  * 字节数组与16进制字符串相互转换的工具类
  * 
  * @author zhu@goome
  *
  */
    public  static class Byte2Hex
    {


        /**
        * 整数到字节数组转换
        * 
        * @param n 要转换的整数
        * @return
        */
        public static byte[] int2bytes(int n)
        {
            byte[] ab = new byte[4];
            ab[0] = (byte)(0xff & n);
            ab[1] = (byte)((0xff00 & n) >> 8);
            ab[2] = (byte)((0xff0000 & n) >> 16);
            ab[3] = (byte)((0xff000000 & n) >> 24);
            return ab;
        }

        /**
         * 短整型到字节数组转换
        * @param n 短整型
        * @return
        */
        public static byte[] short2bytes(short n)
        {
            byte[] b = new byte[2];
            b[0] = (byte)((n & 0xFF00) >> 8);
            b[1] = (byte)(n & 0xFF);
            return b;
        }

        /**
        * 字节数组转换成短整型
        * 
        * @param b 字节数组
        * @return
        */
        public static short bytes2short(byte[] b)
        {
            short n = (short)(((b[0] < 0 ? b[0] + 256 : b[0]) << 8) + (b[1] < 0 ? b[1] + 256
                    : b[1]));
            return n;
        }

        /**
         * 字节数组到整数的转换
        * @param b 字节数组
        * @return
        */
        public static int bytes2int(byte[] b)
        {
            int s = 0;
            s = ((((b[0] & 0xff) << 8 | (b[1] & 0xff)) << 8) | (b[2] & 0xff)) << 8
                    | (b[3] & 0xff);
            return s;
        }

        /**
        * 字节转换到字符
        * 
        * @param b 字节
        * @return
        */
        public static char byte2char(byte b)
        {
            return (char)b;
        }

        /**
        * 16进制char转换成整型
        * 
        * @param c
        * @return
        */
        public static int parse(char c)
        {
            if (c >= 'a')
                return (c - 'a' + 10) & 0x0f;
            if (c >= 'A')
                return (c - 'A' + 10) & 0x0f;
            return (c - '0') & 0x0f;
        }

        /**
        * 字节数组转换成十六进制字符串
        * 
        * @param b
        * @return
        */
        public static String Bytes2HexString(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr.Trim();
        }

        /**
        * 十六进制字符串转换成字节数组
        * 
        * @param hexstr
        * @return
        */
        public static byte[] HexString2Bytes(String hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        ///     高低位转为正常
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String RenH2L(string str)
        {
            int isInt = str.Length % 2;
            if (isInt != 0)
            {
                str = str.Insert(str.Length - 1, "0"); // str.PadLeft(str.Length + isInt, '0');
            }
            var strTem = new StringBuilder("");
            int len = str.Length;

            for (int i = 0; i < len; i += 2)
            {
                strTem.Append(str.Substring(len - 2 - i, 2));
            }
            return strTem.ToString();
        }
        /// <summary>
        /// 移位算法
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="startPostion"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetPostionBitByBit(byte[] byteData, int startPostion, int length)
        {
            int valI=0;
            if (byteData.Length == 1)
            {
                valI = ((int)Convert.ToInt32(Bytes2HexString(byteData),16) & (Convert.ToInt16("1".PadLeft(startPostion + length, '1'), 2))) >> (startPostion);

            }
            else {
                valI = ((int)Convert.ToInt32(Bytes2HexString(byteData),16) & (Convert.ToInt32("1".PadLeft(startPostion + length, '1'), 2))) >> (startPostion);

            }
         
            return Convert.ToString(valI, 2).PadLeft(length, '0');
        }

        public static string GetPostionBitByBit(int byteData, int startPostion, int length)
        {
            int valI = ((int)byteData & (Convert.ToInt16("1".PadLeft(startPostion + length, '1'), 2))) >> (startPostion);
            return Convert.ToString(valI, 2).PadLeft(length, '0');
        }


    }

}
