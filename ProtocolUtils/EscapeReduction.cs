using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils
{
    /// <summary>
    /// 标志位转义处理类，包括转义和还原
    /// </summary>
    public static class EscapeReduction
    {
        //待转化编码
        private static string DeCodeFirst = "7E";
        //转发为编码
        private static string TransCodeFirst = "7D02";
        //7D转换
        private static string DeCodeSecond = "7D";
        private static string TransCodeSecond = "7D01";
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static string EscapeCode(string hexStr)
        {
            if (hexStr.IndexOf(DeCodeFirst) != -1)
            {
                hexStr = hexStr.Replace(DeCodeSecond, TransCodeSecond);
                hexStr = hexStr.Replace(DeCodeFirst, TransCodeFirst);
            }

            return "7E2323" + hexStr + "7E";
        }
        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="hexStr"></param>
        public static string ReductionCode(string hexStr)
        {
            hexStr = hexStr.Trim("7E".ToCharArray()).TrimStart("2323".ToCharArray());
            hexStr = hexStr.Replace(TransCodeFirst, DeCodeFirst);
            hexStr = hexStr.Replace(TransCodeSecond, DeCodeSecond);
            return hexStr;
        }
    }
}
