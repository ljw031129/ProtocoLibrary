using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolUtils
{
    /**
  * 协议处理中的一些工具类
  * 
  * @author zhu@goome 2010/03/27
  *
  */
    public static class ProtocolUtil
    {
        /**
         * 把16进制的时间转换成常规日期时间
         * 
         * @param hexDateTime 终端上来的16进制时间
         * @return
         */
        public static String getSimpleFormatDateTime(String hexDateTime)
        {
            int[] iDateTiem = new int[6];

            for (int j = 1; j <= 6; j++)
            {
                iDateTiem[j - 1] = Convert.ToInt32(hexDateTime.Substring(
                        (j - 1) * 2, j * 2), 16);
            }

            String head = "20";
            if ((iDateTiem[0] + "").Length == 1)
                head = "200";
            return head + iDateTiem[0] + "-" + iDateTiem[1] + "-" + iDateTiem[2]
                    + " " + iDateTiem[3] + ":" + iDateTiem[4] + ":" + iDateTiem[5];
        }

        /**
     * 根据int获取相应的16进制的字符串<br>
     *  9-> 09,15->0F,补充到指定的长度
     * 
     * @param i 10进制整型数
     * @param length  目标16进制数据的长度(1字节是2位)
     * @return
     */
        public static String getHexIntString(int i, int length)
        {
            String str = Convert.ToString(i, 16).TrimStart('0');
            if (str.Length < length)
            {
                str = str.PadLeft(length, '0');
            }
            return str.ToUpper();
        }

        /**
         * 构造服务器返回数据
         * 
         * @param protocolID 协议号
         * @param sequenceNum 信息序号
         * @return 返回给终端的响应
         */
        public static String buildResponseString(String protocolID, int sequenceNum)
        {

            // 内容长度
            String contentLength = "05";
            // 序列号(2字节)
            // 转成16进制
            //		log.info("终端上来的序列号:" + sequenceNum);
            String xuliehao = Convert.ToString(sequenceNum, 16);
            //		log.info("返回终端的序列号:" + xuliehao);
            // 补足成4位，2字节
            xuliehao = xuliehao.PadLeft(4, '0');

            // 需要crc校验的位
            String crcString = contentLength + protocolID + xuliehao;
            String crcCode = getCrc16Code(crcString);

            // 组装返回给终端的数据包
            StringBuilder response = new StringBuilder();
            // 包头(2字节)
            response.Append("7878");
            // 内容长度(1字节)
            response.Append(contentLength);
            // 协议号(1字节)
            response.Append(protocolID);
            // 序列号(2字节)
            response.Append(xuliehao);
            // CRC校验码（2字节）
            response.Append(crcCode);
            response.Append("0D0A");
            return response.ToString().ToUpper();
        }

        /**
         * 构造服务器返回数据
         * 
         * @param protocolID
         *            协议号
         * @param sequenceNum
         *            信息序号
         * @param content 信息内容(已经转换成16进制的了)
         * @return 返回给终端的响应
         */
        public static String buildResponseString(String protocolID,
                int sequenceNum, String content)
        {

            int length = 5 + content.Length / 2;

            // 内容长度(1字节)
            String contentLength = getHexIntString(length, 2);
            // 序列号(2字节)
            // 转成16进制
            // log.info("终端上来的序列号:" + sequenceNum);
            String xuliehao = Convert.ToString(sequenceNum, 16);
            // log.info("返回终端的序列号:" + xuliehao);
            // 补足成4位，2字节
            xuliehao = xuliehao.PadLeft(4, '0');

            // 需要crc校验的位
            String crcString = contentLength + protocolID + content + xuliehao;
            String crcCode = getCrc16Code(crcString);

            // 组装返回给终端的数据包
            StringBuilder response = new StringBuilder();
            // 包头(2字节)
            response.Append("7878");
            // 内容长度(1字节)
            response.Append(contentLength);
            // 协议号(1字节)
            response.Append(protocolID);
            response.Append(content);
            // 序列号(2字节)
            response.Append(xuliehao);
            // CRC校验码（2字节）
            response.Append(crcCode);
            response.Append("0D0A");
            return response.ToString();
        }

        /**
         * 16进制字符串获取crc16校验码
         * 
         * @param crcString 终端传过来的16进制的字符串
         * @return CRC16的校验码
         */
        public static String getCrc16Code(String crcString)
        {
            // 转换成字节数组
            byte[] creBytes = Byte2Hex.HexString2Bytes(crcString);

            // 开始crc16校验码计算
            CRC16Util crc16 = new CRC16Util();
            crc16.reset();
            crc16.update(creBytes);
            int crc = crc16.getCrcValue();
            // 16进制的CRC码
            String crcCode = Convert.ToString(crc, 16).TrimStart('0');
            // 补足到4位
            if (crcCode.Length < 4)
            {
                crcCode = crcCode.PadLeft(4, '0');
            }
            return crcCode;
        }

        /**
     * 构造发送给终端的指令数据包<br>
     * 
     * @param content
     *            指令内容(转换后的16进制字节字符串)
     * @param serialNo
     *            指令流水号(4字节 16进制字符串)
     * @return
     */
        public static String buildSendCmdPackage(String content, String serialNo)
        {

            // 指令流水号 即服务器标志位
            // 包长度 协议号(1字节)+指令长度(1字节) + 指令流水号(4字节)+
            // 内容长度(N)+包序号(2字节)+crc校验(2字节)
            int length = 1 + 1 + 4 + content.Length / 2 + 4;
            StringBuilder sb = new StringBuilder();
            // 包长度
            sb.Append(ProtocolUtil.getHexIntString(length, 2));
            // 协议号:80
            sb.Append("80");
            // 指令内容长度(指令流水号(4字节)+内容长度(N))
            int contentLen = 4 + content.Length / 2;
            // 指令内容长度：1字节
            sb.Append(ProtocolUtil.getHexIntString(contentLen, 2));
            // 指令流水号(4字节)
            sb.Append(serialNo);
            // 信息内容
            sb.Append(content);
            // 信息序列号(随便写一个)
            sb.Append("00A0");
            // CRC校验码
            String crc = ProtocolUtil.getCrc16Code(sb.ToString());
            sb.Append(crc);
            sb.Append("0D0A");
            return "7878" + sb.ToString();
        }

    }
}
