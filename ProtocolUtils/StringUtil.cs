using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolUtils
{
    /**
 * 字符串工具类
 * 
 *
 *
 */
    public class StringUtil
    {
        /**
         * 把一个str转换成Date
         * 
         * @param str
         * @return
         */
        public static DateTime getDateByStr(string str)
        {
            DateTime dt = Convert.ToDateTime(str);
            return dt;
        }

        /**
         * 把一个date格式化成string
         * 
         * @param date
         * @return
         */
        public static String getStrByDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
