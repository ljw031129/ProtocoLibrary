using ProtocolUtils.Models;
using ProtocolUtils.SqlUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SuperSocket.Common;

namespace ProtocolUtils.Lbs
{
    public class DataBaseOperate
    {
        /// <summary>
        /// 根据IMEI号获取当前设备等待发送的指令
        /// </summary>
        /// <param name="connetString"></param>
        /// <param name="IMEI"></param>
        public static DataTable GetCurrentCmd(string connetString, string IMEI)
        {

            string commandText = "SELECT TOP 1 *  FROM TerminalEquipmentCommandCurrents where IMEI='" + IMEI + "'";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
                    return SqlHelper.ExecuteDataTable(conn, commandText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("写入数据异常" + ex);
                }
                finally
                {
                    conn.Close();
                }
                return null;

            }
        }

        public static void UpdateStatus(string connetString, string serNumId, string IMEI, string sendStr)
        {
            SqlParameter[] delCurrent = new SqlParameter[1];
            delCurrent[0] = new SqlParameter("@SerNum", SqlDbType.Int);
            delCurrent[0].Value = serNumId;
            string commanddelCurrentText = "DELETE FROM TerminalEquipmentCommandCurrents where SerNum=@SerNum";

            SqlParameter[] insertCommand = new SqlParameter[3];
            insertCommand[0] = new SqlParameter("@IMEI", SqlDbType.VarChar, 50);
            insertCommand[0].Value = IMEI;
            insertCommand[1] = new SqlParameter("@OperateTime", SqlDbType.BigInt);
            insertCommand[1].Value = DateUtils.ConvertDateTimeInt(DateTime.Now);
            insertCommand[2] = new SqlParameter("@OperateDataHex", SqlDbType.VarChar, 500);
            insertCommand[2].Value = sendStr;
            string insertCommandText =
                @"INSERT INTO TerminalEquipmentCommands
                           ([TerminalEquipmentCommandId]
                           ,[IMEI]
                           ,[OperateTime]
                           ,[Dtype]           
                           ,[OperateDataHex]
                           ,[OperateStatue]          
                          )
                     VALUES
                           (NEWID()
                           ,@IMEI
                           ,@OperateTime
                           ,3          
                           ,@OperateDataHex
                           ,'3'
                          )";

            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, commanddelCurrentText, delCurrent);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, insertCommandText, insertCommand);

                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Console.WriteLine("数据写入异常");
                        throw;

                    }
                }
            }
        }

        public static void InsertStatus(string connetString, string serNumId, string IMEI, string status)
        {


            SqlParameter[] insertCommand = new SqlParameter[3];
            insertCommand[0] = new SqlParameter("@IMEI", SqlDbType.VarChar, 50);
            insertCommand[0].Value = IMEI;
            insertCommand[1] = new SqlParameter("@OperateTime", SqlDbType.BigInt);
            insertCommand[1].Value = DateUtils.ConvertDateTimeInt(DateTime.Now);
            insertCommand[2] = new SqlParameter("@OperateStatue", SqlDbType.VarChar, 50);
            insertCommand[2].Value = status;
            string insertCommandText =
                @"INSERT INTO TerminalEquipmentCommands
                           ([TerminalEquipmentCommandId]
                           ,[IMEI]
                           ,[OperateTime]
                           ,[Dtype]           
                           ,[OperateDataHex]
                           ,[OperateStatue]          
                          )
                     VALUES
                           (NEWID()
                           ,@IMEI
                           ,@OperateTime
                           ,4         
                           ,''
                           ,@OperateStatue
                          )";

            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, insertCommandText, insertCommand);

                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Console.WriteLine("数据写入异常");
                        throw;

                    }
                }
            }
        }

        /// <summary>
        /// 新版数据解析函数
        /// </summary>
        /// <param name="revData"></param>
        /// <param name="revTime"></param>
        public static void RevDataNew(string connetString, string connetStringLbs, string revData, string revTime)
        {
            //将数据库中存储的数据转化成为字节数组
            byte[] requestInfo = Byte2Hex.HexString2Bytes(revData);
            //byte[] zInfo = StrToToHexByte(revData);
            //zInfo[7] = 0x08;
            //数据解析部分
            byte[] protocolVersion = requestInfo.CloneRange(8, 1); //协议版本号
            byte[] devId = requestInfo.CloneRange(9, 8);
            byte[] softVersion = requestInfo.CloneRange(17, 1);
            byte[] hardVersion = requestInfo.CloneRange(18, 1);
            byte[] equipmentTime = requestInfo.CloneRange(19, 6);
            byte[] equipmentStatue = requestInfo.CloneRange(25, 1);
            byte[] equipmentWorkModel = requestInfo.CloneRange(26, 1);
            byte[] equipmentWorkTime = requestInfo.CloneRange(27, 1);
            byte[] equipmentSleepTime = requestInfo.CloneRange(28, 2);
            byte[] equipmentIntervalTime = requestInfo.CloneRange(30, 2);
            byte[] equipmentVol = requestInfo.CloneRange(32, 2);
            byte[] equipmentTotalTime = requestInfo.CloneRange(34, 4);
            byte[] equipmentBlindCounts = requestInfo.CloneRange(38, 2);
            //启动次数
            byte[] startCounts = requestInfo.CloneRange(40, 2);

            byte[] equipmentWarning = requestInfo.CloneRange(60, 1);
            byte[] equipmentBlindSign = requestInfo.CloneRange(61, 1);
            byte[] equipmentIsLocation = requestInfo.CloneRange(62, 1); //定位信息
            byte[] southNorthSign = requestInfo.CloneRange(63, 1);
            byte[] eastWestSign = requestInfo.CloneRange(68, 1);
            byte[] latitude = requestInfo.CloneRange(64, 4); //纬度
            byte[] longitude = requestInfo.CloneRange(69, 4); //经度
            byte[] speed = requestInfo.CloneRange(73, 2);
            byte[] direction = requestInfo.CloneRange(75, 2);
            byte[] hight = requestInfo.CloneRange(77, 3);
            byte[] satelliteUseCount = requestInfo.CloneRange(80, 1);
            byte[] satelliteSeeCount = requestInfo.CloneRange(81, 1);
            byte[] signalGsm = requestInfo.CloneRange(82, 1);
            byte[] serverInformationLac = requestInfo.CloneRange(83, 2);
            byte[] serverInformationCell = requestInfo.CloneRange(85, 2);
            byte[] nearbyInformationOne = requestInfo.CloneRange(87, 4);
            byte[] nearbyInformationTwo = requestInfo.CloneRange(91, 4);
            byte[] nearbyInformationThree = requestInfo.CloneRange(95, 4);
            string devid = Encoding.ASCII.GetString(devId);
            //数据解析
            var ae = new AllEntity(70);
            var ge = new GpsEntity { ID = devid };

            ae.DictALLEnti["A1"] = Convert.ToInt32(Byte2Hex.Bytes2HexString(protocolVersion), 16).ToString();

            ae.DictALLEnti["A3"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(softVersion)), 16).ToString();
            ae.DictALLEnti["A4"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(hardVersion)), 16).ToString();
            //ae.DictALLEnti["A5"] = equipmentTime[0] + 2000 + "-" + equipmentTime[1] + "-" + equipmentTime[2] + " " +
            //                       equipmentTime[3] + ":" + equipmentTime[4] + ":" + equipmentTime[5];
            ae.DictALLEnti["A6"] = equipmentStatue[0].ToString();
            ae.DictALLEnti["A7"] = equipmentWorkModel[0].ToString();
            ae.DictALLEnti["A8"] = equipmentWorkTime[0].ToString();
            ae.DictALLEnti["A9"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentSleepTime)), 16).ToString();
            ae.DictALLEnti["A10"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentIntervalTime)), 16).ToString();
            ae.DictALLEnti["A11"] = (Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentVol)), 16) * 0.001).ToString("0.000");
            // ae.DictALLEnti["A12"] = Convert.ToInt32((byteToHexStr(equipmentTotalTime)), 16).ToString();
            ae.DictALLEnti["A13"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentBlindCounts)), 16).ToString();
            ae.DictALLEnti["A15"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentWarning)), 16).ToString();
            ae.DictALLEnti["A16"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentBlindSign)), 16).ToString();
            ae.DictALLEnti["A28"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(startCounts)), 16).ToString();
            ae.DictALLEnti["A17"] = Encoding.ASCII.GetString(equipmentIsLocation);
            //判断定位信息
            if (ae.DictALLEnti["A17"] == "A")
            {
                //纠偏后的数据
                double clats = 0;
                double clngs = 0;
                double lat = (Convert.ToInt32((Byte2Hex.Bytes2HexString(latitude)), 16) / 1000000.0); //+ (plats % 10000000) / 6000000.0;
                double lng = (Convert.ToInt32((Byte2Hex.Bytes2HexString(longitude)), 16) / 1000000.0); //+ (plogs % 10000000) / 6000000.0;
                LbsUtils.Correct_PlogPlat_Baidu(lat, lng, connetStringLbs, out clats, out clngs);
                ge.IsPos = "定位";
                ge.Plat = Convert.ToDecimal(clats);
                ge.Plog = Convert.ToDecimal(clngs);
                ge.Pos = LbsUtils.Gpsoo(clats.ToString(), clngs.ToString());
                ge.GPSMast = "";
            }
            else
            {
                double lat = 0.00;
                double lng = 0.00;
                string address = "";
                LbsUtils.LocateSqlServerMapApi(Convert.ToUInt32((Byte2Hex.Bytes2HexString(serverInformationCell)), 16),
                    Convert.ToUInt32((Byte2Hex.Bytes2HexString(serverInformationLac)), 16), connetStringLbs, out lat, out lng, out address);
                //本地没有数据调用聚合数据
                if (lat == 0 && lng == 0)
                {
                    //获取基站信息
                    LbsUtils.LocateGooleMapApi(Convert.ToUInt32((Byte2Hex.Bytes2HexString(serverInformationCell)), 16),
                        Convert.ToUInt32((Byte2Hex.Bytes2HexString(serverInformationLac)), 16), out lat, out lng);
                }
                ge.IsPos = "不定位";
                ge.Plat = Convert.ToDecimal(lat);
                ge.Plog = Convert.ToDecimal(lng);
                ge.Pos = address == "" ? LbsUtils.Gpsoo(lat.ToString(), lng.ToString()) : address;
                ge.GPSMast = "";
            }
            //  ConsoleApplicationGPS.Program.SendTranData(zInfo);
            ge.GpsTime = equipmentTime[0] + 2000 + "-" + equipmentTime[1] + "-" + equipmentTime[2] + " " +
                                   equipmentTime[3] + ":" + equipmentTime[4] + ":" + equipmentTime[5];
            //省份信息--省份信息不解析
            ge.Province = "";
            ae.DictALLEnti["A18"] = Encoding.ASCII.GetString(southNorthSign);
            ae.DictALLEnti["A20"] = Encoding.ASCII.GetString(eastWestSign);
            ae.DictALLEnti["A22"] = (Convert.ToInt32((Byte2Hex.Bytes2HexString(speed)), 16) * 0.1).ToString("0.0");
            ae.DictALLEnti["A24"] = (Convert.ToInt32((Byte2Hex.Bytes2HexString(direction)), 16) * 0.1).ToString("0.0");
            //海拔高度
            var heightBit = Convert.ToString(Convert.ToInt32(Byte2Hex.Bytes2HexString(hight), 16), 2).PadLeft(24, '0');
            if (heightBit != "0")
            {
                var heightFh = heightBit.Substring(0, 1) == "1" ? "-" : "+";
                heightBit = heightBit.Substring(1);
                ae.DictALLEnti["A25"] = heightFh + (Convert.ToInt32(heightBit, 2) * 0.1).ToString("0.0");
            }
            else
            {
                ae.DictALLEnti["A25"] = "0";
            }
            ae.DictALLEnti["A26"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(satelliteUseCount)), 16).ToString();
            ae.DictALLEnti["A27"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(satelliteSeeCount)), 16).ToString();
            //ae.DictALLEnti["A28"] = Convert.ToInt32((byteToHexStr(signalGsm)), 16).ToString();

            ae.DictALLEnti["A62"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(signalGsm)), 16).ToString();
            ae.DictALLEnti["A63"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(serverInformationLac)), 16).ToString();
            ae.DictALLEnti["A64"] = Convert.ToInt32((Byte2Hex.Bytes2HexString(serverInformationCell)), 16).ToString();
            //解析出设备编号
            ge.Rtime = revTime; //回传时间
            ge.isWorkS = -1;
            var totalTime = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentTotalTime)), 16);
            int h = totalTime / 3600;
            int f = (totalTime / 60) % 60;
            int m = totalTime % 60;
            //关于盲区数据工作时间小于正常回传的工作时间，存储过程insertTrack正常数据更新工作时间
            ge.TotalTime = h + "时" + f + "分" + m + "秒";
            ge.WorkState = "不工作";

            ge.caijishijian = revTime;

            if (ae.DictALLEnti["A63"] == "0")
            {
                ge.IsPos = "不定位";
                ge.Plat = 0;
                ge.Plog = 0;
                ge.Pos = "";
                ge.Province = "";
            }
            //解析数据信息

            // insertTrack(connetString,ae, ge, -1, "");
            PostionModel tp = new PostionModel();
            tp.DataType = "1";
            tp.IMEI = devid;
            tp.Ptime = ge.GpsTime;
            tp.Rtime = revTime;
            tp.AccStatus = "无效";
            tp.BatteryVoltage = (Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentVol)), 16) * 0.001).ToString("0.000");
            tp.BlindDataCount = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentBlindCounts)), 16).ToString();
            tp.BlindSign = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentBlindSign)), 16).ToString();
            //定位标志A为已定位
            tp.IsPos = Encoding.ASCII.GetString(equipmentIsLocation);
            tp.East = Encoding.ASCII.GetString(eastWestSign);
            tp.North = Encoding.ASCII.GetString(southNorthSign);
            tp.Ghigh = ae.DictALLEnti["A25"];
            tp.GpsType = "无效";
            tp.GsDirection = (Convert.ToInt32((Byte2Hex.Bytes2HexString(direction)), 16) * 0.1).ToString("0.0");
            tp.GsmSignal = Convert.ToInt32((Byte2Hex.Bytes2HexString(signalGsm)), 16).ToString();
            tp.Gspeed = (Convert.ToInt32((Byte2Hex.Bytes2HexString(speed)), 16) * 0.1).ToString("0.0");
            tp.UseSatelliteCount = Convert.ToInt32((Byte2Hex.Bytes2HexString(satelliteUseCount)), 16).ToString();
            tp.SeeSatelliteCount = Convert.ToInt32((Byte2Hex.Bytes2HexString(satelliteSeeCount)), 16).ToString();

            tp.WorkModel = equipmentWorkModel[0].ToString();
            tp.WorkStatue = equipmentStatue[0].ToString();
            tp.WorkTime = equipmentWorkTime[0].ToString();
            tp.SleepTime = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentSleepTime)), 16).ToString();
            //间隔时间-追车模式
            tp.IntervalTime = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentIntervalTime)), 16).ToString();
            tp.TootalWorkTime = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentTotalTime)), 16).ToString();
            tp.Timing = Convert.ToInt32((Byte2Hex.Bytes2HexString(equipmentIntervalTime)), 16).ToString();
            tp.Lat = ge.Plat.ToString();
            tp.Lng = ge.Plog.ToString();
            tp.Pos = ge.Pos;
            tp.MCC = "0";
            tp.MNC = "0";
            tp.CID = Convert.ToInt32((Byte2Hex.Bytes2HexString(serverInformationCell)), 16).ToString();
            tp.LAC = Convert.ToInt32((Byte2Hex.Bytes2HexString(serverInformationLac)), 16).ToString();
            tp.StartCount = Convert.ToInt32((Byte2Hex.Bytes2HexString(startCounts)), 16).ToString();
            //无线数据写入
            LbsUpdateData.WirelessInsertRequest(connetString, tp);
            Console.WriteLine("正常解析：设备编号" + devid + DateTime.Now);
        }
        public static int insertTrack(string connetString, AllEntity ae, GpsEntity ge, int alarmType, string alarmStr)
        {
            int i = 0;
            SqlParameter[] sqlp = new SqlParameter[93];
            sqlp[0] = new SqlParameter("@ID", SqlDbType.VarChar, 20);
            sqlp[0].Value = ge.ID ?? "";
            sqlp[1] = new SqlParameter("@IsPos", SqlDbType.VarChar, 6);
            sqlp[1].Value = ge.IsPos ?? "";
            sqlp[2] = new SqlParameter("@GPSMast", SqlDbType.VarChar, 6);
            sqlp[2].Value = ge.GPSMast ?? "";
            sqlp[3] = new SqlParameter("@Pos", SqlDbType.VarChar, 100);
            sqlp[3].Value = ge.Pos ?? "";
            sqlp[4] = new SqlParameter("@Plat", SqlDbType.VarChar, 19);
            sqlp[4].Value = ge.Plat;
            sqlp[5] = new SqlParameter("@Plog", SqlDbType.VarChar, 19);
            sqlp[5].Value = ge.Plog;
            sqlp[6] = new SqlParameter("@GpsTime", SqlDbType.VarChar, 19);
            sqlp[6].Value = ge.GpsTime ?? "";
            sqlp[7] = new SqlParameter("@GatherTime", SqlDbType.VarChar, 19);
            sqlp[7].Value = ge.caijishijian ?? "";
            sqlp[8] = new SqlParameter("@WorkTime", SqlDbType.VarChar, 50);
            sqlp[8].Value = ge.TotalTime ?? "0";
            sqlp[9] = new SqlParameter("@WorkStatus", SqlDbType.VarChar, 8);
            sqlp[9].Value = ge.WorkState ?? "";// te.Voltage;
            sqlp[10] = new SqlParameter("@RTime", SqlDbType.VarChar, 19);
            sqlp[10].Value = ge.Rtime ?? "";
            sqlp[11] = new SqlParameter("@A1", SqlDbType.VarChar, 20);
            sqlp[11].Value = ae.DictALLEnti["A1"] ?? "";// te.OilTemerature;
            sqlp[12] = new SqlParameter("@A2", SqlDbType.VarChar, 20);
            sqlp[12].Value = ae.DictALLEnti["A2"] ?? "";// te.OilTemerature;
            sqlp[13] = new SqlParameter("@A3", SqlDbType.VarChar, 20);
            sqlp[13].Value = ae.DictALLEnti["A3"] ?? "";//te.GprsSignal;
            sqlp[14] = new SqlParameter("@A4", SqlDbType.VarChar, 20);
            sqlp[14].Value = ae.DictALLEnti["A4"] ?? "";
            sqlp[15] = new SqlParameter("@A5", SqlDbType.VarChar, 20);
            sqlp[15].Value = ae.DictALLEnti["A5"] ?? "";//te.EngineSpeed;
            sqlp[16] = new SqlParameter("@A6", SqlDbType.VarChar, 20);
            sqlp[16].Value = ae.DictALLEnti["A6"] ?? "";// te.Alarm;
            sqlp[17] = new SqlParameter("@A7", SqlDbType.VarChar, 20);
            sqlp[17].Value = ae.DictALLEnti["A7"] ?? "";//te.DataCaiJiTime;
            sqlp[18] = new SqlParameter("@A8", SqlDbType.VarChar, 20);
            sqlp[18].Value = ae.DictALLEnti["A8"] ?? "";
            sqlp[19] = new SqlParameter("@A9", SqlDbType.VarChar, 20);
            sqlp[19].Value = ae.DictALLEnti["A9"] ?? "";
            sqlp[20] = new SqlParameter("@A10", SqlDbType.VarChar, 20);
            sqlp[20].Value = ae.DictALLEnti["A10"] ?? "";
            sqlp[21] = new SqlParameter("@A11", SqlDbType.VarChar, 20);
            sqlp[21].Value = ae.DictALLEnti["A11"] ?? "";//te.ControlAlarm;
            sqlp[22] = new SqlParameter("@A12", SqlDbType.VarChar, 20);
            sqlp[22].Value = ae.DictALLEnti["A12"] ?? "";
            sqlp[23] = new SqlParameter("@A13", SqlDbType.VarChar, 20);
            sqlp[23].Value = ae.DictALLEnti["A13"] ?? "";
            sqlp[24] = new SqlParameter("@A14", SqlDbType.VarChar, 20);
            sqlp[24].Value = ae.DictALLEnti["A14"] ?? "";
            sqlp[25] = new SqlParameter("@A15", SqlDbType.VarChar, 20);
            sqlp[25].Value = ae.DictALLEnti["A15"] ?? "";
            sqlp[26] = new SqlParameter("@A16", SqlDbType.VarChar, 20);
            sqlp[26].Value = ae.DictALLEnti["A16"] ?? "";
            sqlp[27] = new SqlParameter("@A17", SqlDbType.VarChar, 20);
            sqlp[27].Value = ae.DictALLEnti["A17"] ?? "";// te.PumpB;
            sqlp[28] = new SqlParameter("@A18", SqlDbType.VarChar, 19);
            sqlp[28].Value = ae.DictALLEnti["A18"] ?? "";
            sqlp[29] = new SqlParameter("@A19", SqlDbType.VarChar, 20);
            sqlp[29].Value = ae.DictALLEnti["A19"] ?? "";
            sqlp[30] = new SqlParameter("@A20", SqlDbType.VarChar, 20);
            sqlp[30].Value = ae.DictALLEnti["A20"] ?? "";
            sqlp[31] = new SqlParameter("@A21", SqlDbType.VarChar, 20);
            sqlp[31].Value = ae.DictALLEnti["A21"] ?? "";// te.Mileage;
            sqlp[32] = new SqlParameter("@A22", SqlDbType.VarChar, 20);
            sqlp[32].Value = ae.DictALLEnti["A22"] ?? "";// te.Mileage;
            sqlp[33] = new SqlParameter("@A23", SqlDbType.VarChar, 20);
            sqlp[33].Value = ae.DictALLEnti["A23"] ?? "";
            sqlp[34] = new SqlParameter("@A24", SqlDbType.VarChar, 20);
            sqlp[34].Value = ae.DictALLEnti["A24"] ?? "";
            sqlp[35] = new SqlParameter("@A25", SqlDbType.VarChar, 20);
            sqlp[35].Value = ae.DictALLEnti["A25"] ?? "";
            sqlp[36] = new SqlParameter("@A26", SqlDbType.VarChar, 20);
            sqlp[36].Value = ae.DictALLEnti["A26"] ?? "";
            sqlp[37] = new SqlParameter("@A27", SqlDbType.VarChar, 20);
            sqlp[37].Value = ae.DictALLEnti["A27"] ?? "";
            sqlp[38] = new SqlParameter("@A28", SqlDbType.VarChar, 20);
            sqlp[38].Value = ae.DictALLEnti["A28"] ?? "";
            sqlp[39] = new SqlParameter("@A29", SqlDbType.VarChar, 20);
            sqlp[39].Value = ae.DictALLEnti["A29"] ?? "";
            sqlp[40] = new SqlParameter("@A30", SqlDbType.VarChar, 20);
            sqlp[40].Value = ae.DictALLEnti["A30"] ?? "";
            sqlp[41] = new SqlParameter("@A31", SqlDbType.VarChar, 20);
            sqlp[41].Value = ae.DictALLEnti["A31"] ?? "";
            sqlp[42] = new SqlParameter("@A32", SqlDbType.VarChar, 20);
            sqlp[42].Value = ae.DictALLEnti["A32"] ?? "";
            sqlp[43] = new SqlParameter("@A33", SqlDbType.VarChar, 20);
            sqlp[43].Value = ae.DictALLEnti["A33"] ?? "";
            sqlp[44] = new SqlParameter("@A34", SqlDbType.VarChar, 20);
            sqlp[44].Value = ae.DictALLEnti["A34"] ?? "";
            sqlp[45] = new SqlParameter("@A35", SqlDbType.VarChar, 20);
            sqlp[45].Value = ae.DictALLEnti["A35"] ?? "";
            sqlp[46] = new SqlParameter("@A36", SqlDbType.VarChar, 20);
            sqlp[46].Value = ae.DictALLEnti["A36"] ?? "";
            sqlp[47] = new SqlParameter("@A37", SqlDbType.VarChar, 20);
            sqlp[47].Value = ae.DictALLEnti["A37"] ?? "";
            sqlp[48] = new SqlParameter("@A38", SqlDbType.VarChar, 20);
            sqlp[48].Value = ae.DictALLEnti["A38"] ?? "";
            sqlp[49] = new SqlParameter("@A39", SqlDbType.VarChar, 20);
            sqlp[49].Value = ae.DictALLEnti["A39"] ?? "";
            sqlp[50] = new SqlParameter("@A40", SqlDbType.VarChar, 20);
            sqlp[50].Value = ae.DictALLEnti["A40"] ?? "";
            sqlp[51] = new SqlParameter("@A41", SqlDbType.VarChar, 20);
            sqlp[51].Value = ae.DictALLEnti["A41"] ?? "";
            sqlp[52] = new SqlParameter("@A42", SqlDbType.VarChar, 20);
            sqlp[52].Value = ae.DictALLEnti["A42"] ?? "";
            sqlp[53] = new SqlParameter("@A43", SqlDbType.VarChar, 20);
            sqlp[53].Value = ae.DictALLEnti["A43"] ?? "";
            sqlp[54] = new SqlParameter("@A44", SqlDbType.VarChar, 20);
            sqlp[54].Value = ae.DictALLEnti["A44"] ?? "";
            sqlp[55] = new SqlParameter("@A45", SqlDbType.VarChar, 20);
            sqlp[55].Value = ae.DictALLEnti["A45"] ?? "";//te.ControlAlarm;
            sqlp[56] = new SqlParameter("@A46", SqlDbType.VarChar, 20);
            sqlp[56].Value = ae.DictALLEnti["A46"] ?? "";
            sqlp[57] = new SqlParameter("@A47", SqlDbType.VarChar, 20);
            sqlp[57].Value = ae.DictALLEnti["A47"] ?? "";
            sqlp[58] = new SqlParameter("@A48", SqlDbType.VarChar, 20);
            sqlp[58].Value = ae.DictALLEnti["A48"] ?? "";
            sqlp[59] = new SqlParameter("@A49", SqlDbType.VarChar, 20);
            sqlp[59].Value = ae.DictALLEnti["A49"] ?? "";
            sqlp[60] = new SqlParameter("@A50", SqlDbType.VarChar, 20);
            sqlp[60].Value = ae.DictALLEnti["A50"] ?? "";
            sqlp[61] = new SqlParameter("@A51", SqlDbType.VarChar, 20);
            sqlp[61].Value = ae.DictALLEnti["A51"] ?? "";// te.PumpB;
            sqlp[62] = new SqlParameter("@A52", SqlDbType.VarChar, 20);
            sqlp[62].Value = ae.DictALLEnti["A52"] ?? "";
            sqlp[63] = new SqlParameter("@A53", SqlDbType.VarChar, 20);
            sqlp[63].Value = ae.DictALLEnti["A53"] ?? "";
            sqlp[64] = new SqlParameter("@A54", SqlDbType.VarChar, 20);
            sqlp[64].Value = ae.DictALLEnti["A54"] ?? "";
            sqlp[65] = new SqlParameter("@A55", SqlDbType.VarChar, 20);
            sqlp[65].Value = ae.DictALLEnti["A55"] ?? "";// te.Mileage;
            sqlp[66] = new SqlParameter("@A56", SqlDbType.VarChar, 20);
            sqlp[66].Value = ae.DictALLEnti["A56"] ?? "";// te.Mileage;
            sqlp[67] = new SqlParameter("@A57", SqlDbType.VarChar, 20);
            sqlp[67].Value = ae.DictALLEnti["A57"] ?? "";
            sqlp[68] = new SqlParameter("@A58", SqlDbType.VarChar, 20);
            sqlp[68].Value = ae.DictALLEnti["A58"] ?? "";
            sqlp[69] = new SqlParameter("@A59", SqlDbType.VarChar, 20);
            sqlp[69].Value = ae.DictALLEnti["A59"] ?? "";
            sqlp[70] = new SqlParameter("@A60", SqlDbType.VarChar, 20);
            sqlp[70].Value = ae.DictALLEnti["A60"] ?? "";
            sqlp[71] = new SqlParameter("@A61", SqlDbType.VarChar, 20);
            sqlp[71].Value = ae.DictALLEnti["A61"] ?? "";
            sqlp[72] = new SqlParameter("@A62", SqlDbType.VarChar, 20);
            sqlp[72].Value = ae.DictALLEnti["A62"] ?? "";
            sqlp[73] = new SqlParameter("@A63", SqlDbType.VarChar, 20);
            sqlp[73].Value = ae.DictALLEnti["A63"] ?? "";
            sqlp[74] = new SqlParameter("@A64", SqlDbType.VarChar, 20);
            sqlp[74].Value = ae.DictALLEnti["A64"] ?? "";
            sqlp[75] = new SqlParameter("@A65", SqlDbType.VarChar, 20);
            sqlp[75].Value = ae.DictALLEnti["A65"] ?? "";
            sqlp[76] = new SqlParameter("@A66", SqlDbType.VarChar, 20);
            sqlp[76].Value = ae.DictALLEnti["A66"] ?? "";
            sqlp[77] = new SqlParameter("@A67", SqlDbType.VarChar, 20);
            sqlp[77].Value = ae.DictALLEnti["A67"] ?? "";
            sqlp[78] = new SqlParameter("@A68", SqlDbType.VarChar, 20);
            sqlp[78].Value = ae.DictALLEnti["A68"] ?? "";
            sqlp[79] = new SqlParameter("@A69", SqlDbType.VarChar, 20);
            sqlp[79].Value = ae.DictALLEnti["A69"] ?? "";
            sqlp[80] = new SqlParameter("@A70", SqlDbType.VarChar, 20);
            sqlp[80].Value = ae.DictALLEnti["A70"] ?? "";
            sqlp[81] = new SqlParameter("@returnValue", SqlDbType.Int);
            sqlp[81].Direction = ParameterDirection.Output;
            sqlp[82] = new SqlParameter("@Province", SqlDbType.VarChar, 20);
            sqlp[82].Value = ge.Province ?? "";//te.ControlAlarm;
            sqlp[83] = new SqlParameter("@ACCState", SqlDbType.VarChar, 20);
            sqlp[83].Value = ge.KeyState ?? "";//te.ControlAlarm;
            sqlp[84] = new SqlParameter("@Lock1", SqlDbType.VarChar, 20);
            sqlp[84].Value = ge.Lock1 ?? "";//te.ControlAlarm;
            sqlp[85] = new SqlParameter("@Lock2", SqlDbType.VarChar, 20);
            sqlp[85].Value = ge.Lock2 ?? "";//te.ControlAlarm;
            sqlp[86] = new SqlParameter("@Lock3", SqlDbType.VarChar, 20);
            sqlp[86].Value = ge.Lock3 ?? "";//te.ControlAlarm;
            sqlp[87] = new SqlParameter("@Lock4", SqlDbType.VarChar, 20);
            sqlp[87].Value = ge.Lock4 ?? "";//te.ControlAlarm;
            sqlp[88] = new SqlParameter("@OutState", SqlDbType.VarChar, 20);
            sqlp[88].Value = ge.WkState ?? "";//te.ControlAlarm;
            sqlp[89] = new SqlParameter("@SleepState", SqlDbType.VarChar, 20);
            sqlp[89].Value = ge.SleepState ?? "";//te.ControlAlarm;
            sqlp[90] = new SqlParameter("@controlAlarm", SqlDbType.VarChar, 20);
            sqlp[90].Value = ge.controlAlarm ?? "";//te.ControlAlarm;
            sqlp[91] = new SqlParameter("@isWorkS", SqlDbType.Int);
            sqlp[91].Value = ge.isWorkS;//te.ControlAlarm;
            sqlp[92] = new SqlParameter("@Num", SqlDbType.Int);
            sqlp[92].Value = ge.Num;
            // sqlp[93] = new SqlParameter("@DevPrimitiveWorkTime", SqlDbType.Decimal);
            // sqlp[93].Value = ge.DevPrimitiveWorkTime;
            string commandText = "insertTrack";
            SqlParameter[] sqlp1 = new SqlParameter[4];
            sqlp1[0] = new SqlParameter("@devID", SqlDbType.VarChar, 50);
            sqlp1[0].Value = ge.ID;
            sqlp1[1] = new SqlParameter("@alarmType", SqlDbType.Int);
            sqlp1[1].Value = alarmType;
            sqlp1[2] = new SqlParameter("@alarmStr", SqlDbType.VarChar, 500);
            sqlp1[2].Value = alarmStr;
            sqlp1[3] = new SqlParameter("@returnValue", SqlDbType.Int);
            sqlp1[3].Direction = ParameterDirection.Output;
            string commandText1 = "SP_AlarmInfo_Insert";
            SqlParameter[] sqlp2 = new SqlParameter[3];
            sqlp2[0] = new SqlParameter("@devID", SqlDbType.VarChar, 50);
            sqlp2[0].Value = ge.ID;
            sqlp2[1] = new SqlParameter("@OldTotalTime", SqlDbType.Decimal);
            sqlp2[1].Value = ge.OldTotalTime;
            sqlp2[2] = new SqlParameter("@returnValue", SqlDbType.Int);
            sqlp2[2].Direction = ParameterDirection.Output;
            string commandText2 = "SP_GPSDev_OldWorktime_Update";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                SqlTransaction st = null;
                try
                {
                    conn.Open();
                    st = conn.BeginTransaction("BCUP");
                    SqlHelper.ExecuteNonQuery(st, CommandType.StoredProcedure, commandText, sqlp);
                    i = (int)sqlp[81].Value;
                    if (i != 1)
                    {
                        st.Rollback();
                    }
                    if (alarmType != -1)
                    {
                        SqlHelper.ExecuteNonQuery(st, CommandType.StoredProcedure, commandText1, sqlp1);//报警，预警
                        i = (int)sqlp1[3].Value;
                        if (i != 1)
                        {
                            st.Rollback();
                        }
                    }
                    if (ge.IsTotalTime)
                    {
                        SqlHelper.ExecuteNonQuery(st, CommandType.StoredProcedure, commandText2, sqlp2);
                        i = (int)sqlp2[2].Value;
                        if (i != 1)
                        {
                            st.Rollback();
                        }
                    }
                    st.Commit();
                }
                catch (Exception)
                {
                    st.Rollback();
                    i = -1;
                }
                finally
                {
                    conn.Close();
                }
            }
            return i;
        }
    }
}
