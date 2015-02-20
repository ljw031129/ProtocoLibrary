using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ProtocolUtils.SqlUtils;
using System.Data;
using Newtonsoft.Json;
namespace ProtocolUtils.Lbs
{
    public class LbsUpdateData
    {
        public static void UpDateData(string devid, string postionStr, string rtime, string ptime, string lat, string lng, string connetString, string speed, string dec, string AccStatus)
        {
            long reclongTime = DateUtils.ConvertDateTimeInt(DateTime.Now);
            // string commandCount = "select devid from  ReceiveDataLasts where DevId='"+devid+"'";
            string commandUpdate = "UPDATE ReceiveDataLasts  SET [ReceiveTime] = " + reclongTime + " ,[GpsTime] = " + ptime + ",[GpsPos] = '" + postionStr + "',[GpsPlat] = '" + lat + "',[GpsPlog] = '" + lng + "',[GpsSpeed] = '" + speed + "',AccStatus='" + AccStatus + "' WHERE IMEI='" + devid + "'";
            string commandText = "INSERT INTO ReceiveDatas([ReceiveDataId],[IMEI],[ReceiveTime],[CollectTime],[TotalWorkTime],[TotalMileage],[GpsPlog],[GpsPlat],[GpsTime],[GpsSpeed],GpsPos,AccStatus)VALUES('" + Guid.NewGuid() + "','" + devid + "'," + reclongTime + ",0,0,0," + lng + "," + lat + "," + ptime + ",'" + speed + "','" + postionStr + "','" + AccStatus + "')";
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

        public static void WirelessInsertRequest(string connetString, Models.PostionModel tp)
        {
            long currentTime = DateUtils.ConvertDateTimeInt(DateTime.Now);
            string tpStr = JsonConvert.SerializeObject(tp);
            string commandText = "INSERT INTO TerminalEquipmentCommands ([TerminalEquipmentCommandId],[IMEI] ,[OperateTime],[Dtype],[ReceiveTData],[CommandFromTo])VALUES('" + Guid.NewGuid() + "','" + tp.IMEI + "'," + currentTime + ",'2','" + tpStr + "','c-s')";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
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
        public static void HistoryInsertRequest(string connetStringData, string history, string IMEI)
        {
            long currentTime = DateUtils.ConvertDateTimeInt(DateTime.Now);

            string commandText = "INSERT INTO ReceiveDataHistories([ReceiveDataHistoryId],[DataStr],[ReceiveTime],[IMEI])VALUES('" + Guid.NewGuid() + "','" + history + "','" + DateTime.Now + "','" + IMEI + "')";
            using (SqlConnection conn = new SqlConnection(connetStringData))
            {
                conn.Open();
                try
                {
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



        public static void UpdateStatueRequest(string connetStringData, string IMEI, string acc,string dianYuan)
        {
            long currentTime = DateUtils.ConvertDateTimeInt(DateTime.Now);
            string commandUpdate = "UPDATE ReceiveDataLasts  SET [ReceiveTime] = " + currentTime + " ,AccStatus='" + acc + "',AntennaStatus='" + dianYuan + "' WHERE IMEI='" + IMEI + "'";

            using (SqlConnection conn = new SqlConnection(connetStringData))
            {
                conn.Open();
                try
                {
                    SqlHelper.ExecuteNonQuery(conn, commandUpdate);
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

        public static void UpdateReceiveTimeRequest(string connetStringData, string IMEI)
        {
            long currentTime = DateUtils.ConvertDateTimeInt(DateTime.Now);
            string commandUpdate = "UPDATE ReceiveDataLasts  SET [ReceiveTime] = " + currentTime + " WHERE IMEI='" + IMEI + "'";

            using (SqlConnection conn = new SqlConnection(connetStringData))
            {
                conn.Open();
                try
                {
                    SqlHelper.ExecuteNonQuery(conn, commandUpdate);
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
