using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ProtocolUtils.SqlUtils;
using System.Data;
using ProtocolUtils.DateUtils;

namespace ProtocolUtils.Lbs
{
    public class LbsUpdateData
    {
        public static void UpDateData(string devid, string postionStr, string recTime, string lat, string lng, string connetString, string speed, string dec)
        {
            long reclongTime = DataTimeTran.ConvertDateTimeInt(DateTime.Now);
            long gpsTime = DataTimeTran.ConvertDateTimeInt(DateTime.Parse(recTime).AddHours(8));
            // string commandCount = "select devid from  ReceiveDataLasts where DevId='"+devid+"'";
            string commandUpdate = "UPDATE ReceiveDataLasts  SET [ReceiveTime] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,[GpsTime] = '" + recTime + "',[GpsPos] = '" + postionStr + "',[GpsPlat] = '" + lat + "',[GpsPlog] = '" + lng + "',[GpsSpeed] = '" + speed + "' WHERE DevId='" + devid + "'";
            string commandText = "INSERT INTO ReceiveDatas([ReceiveDataId],[DevId],[ReceiveTime],[CollectTime],[TotalWorkTime],[TotalMileage],[GpsPlog],[GpsPlat],[GpsTime],[GpsSpeed],GpsPos)VALUES('" + Guid.NewGuid() + "','" + devid + "'," + reclongTime + ",0,0,0," + lng + "," + lat + "," + gpsTime + ",'" + speed + "','" + postionStr + "')";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
                    //DataTable dt = SqlHelper.ExecuteDataTable(conn, commandCount);
                    //if (dt!=null && dt.Rows.Count>0)
                    //{
                    //    SqlHelper.ExecuteNonQuery(conn, commandUpdate);
                    //}
                    SqlHelper.ExecuteNonQuery(conn, commandUpdate);
                    SqlHelper.ExecuteNonQuery(conn, commandText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("写入数据异常" + ex);
                }
                finally
                {
                    conn.Close();
                }

            }

        }
    }
}
