using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils
{
    public static class BCCCheck
    {
        /// <summary>
        ///     异或校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte GetBcc(byte[] data)
        {
            if (data == null) return 0;

            int length = data.Length;
            if (length == 0) return 0;
            if (length == 1) return data[0];

            byte bcc = data[0];
            for (int i = 1; i < length; i++)
            {
                bcc ^= data[i];
            }

            return bcc;
        }
    }
}
