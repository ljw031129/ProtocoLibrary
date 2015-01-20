using Newtonsoft.Json.Linq;
using ProtocolUtils.SqlUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace ProtocolUtils.Lbs
{
    public class LbsUtils
    {
        /// <summary>
        /// HaoService基站数据解析服务
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="lac"></param>
        /// <param name="Lat"></param>
        /// <param name="Lng"></param>
        /// <returns></returns>
        public bool HaoServices(uint cellid, uint lac, out double Lat, out double Lng)
        {

            string Url = "http://api.haoservice.com/api/getlbs";
            string postDataStr = "mcc=460&mnc=0&cell_id=" + cellid + "&lac=" + lac + "&key=d0c717d82c05448a90eed80b2b84ddeb&type=2";


            string jsdata = HttpUtils.HTTP_POST(Url, postDataStr);
            if (jsdata != "")
            {
                JObject container = JObject.Parse(jsdata);
                //JSON字符串转化        

                string error_code = container["ErrCode"].ToString();

                var address = container["location"]["addressDescription"].ToString();

                var lon = container["location"]["longitude"].ToString();

                var lat = container["location"]["latitude"].ToString();

                Lng = Convert.ToDouble(lon);

                Lat = Convert.ToDouble(lat);
                return true;
            }
            else
            {
                Lng = 0;

                Lat = 0;
                return false;
            }
        }
        /// <summary>
        /// 聚合基站数据解析服务
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="lac"></param>
        /// <param name="Lat"></param>
        /// <param name="Lng"></param>
        /// <returns></returns>
        public static bool JuHePosition(uint cellid, uint lac, out double Lat, out double Lng)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://v.juhe.cn/cell/get?mnc=0&cell=" + cellid + "&lac=" + lac + "&hex=10&dtype=xml&key=9330441e2df3dbd50c6629d27be15e4d");
            request.Timeout = 1000;
            request.Method = "GET";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonstr = sr.ReadToEnd();
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(jsonstr);
                        XmlNodeList lngList = xdoc.SelectNodes("//LNG");
                        Lng = Convert.ToDouble(lngList[0].InnerText);
                        XmlNodeList latList = xdoc.SelectNodes("//LAT");
                        Lat = Convert.ToDouble(latList[0].InnerText);

                        sr.Close();
                    }
                    response.Close();
                }
                return true;

            }
            catch (Exception e)
            {

                Lat = 0;
                Lng = 0;
                return false;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
        /// <summary>
        /// baidu逆地理解析
        /// </summary>
        /// <param name="plat"></param>
        /// <param name="plog"></param>
        /// <returns></returns>
        public static string BaiDuPosition(string plat, string plog)
        {
            //获取天气和解析  
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://api.map.baidu.com/telematics/v2/reverseGeocoding?location=" + plog + "," + plat + "&ak=B1425ad31e405376388e67ceeececb4c");
            // request.Timeout = 5000;
            request.Method = "GET";
            string str = "";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonstr = sr.ReadToEnd();
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(jsonstr);
                        XmlNodeList xnList = xdoc.SelectNodes("//description");
                        str = xnList[0].InnerText;
                        sr.Close();
                    }
                    response.Close();
                }
                return str.Trim('\n').Trim();

            }
            catch (Exception e)
            {
                string aa = e.Message;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
            return str;
        }
        /// <summary>
        /// 逆地理解析服务，调用本地的数据库
        /// </summary>
        /// <param name="plat"></param>
        /// <param name="plog"></param>
        /// <returns></returns>
        public static string MapGeocoder(string plat, string plog)
        {
            string str = "";
            Encoding encoding = Encoding.GetEncoding("gb2312");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://202.85.209.80:9094/DiscInfo/getDisc?pointX=" + plog + "&pointY=" + plat);
            // HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:9094/DiscInfo/getDisc?pointX=" + plog + "&pointY=" + plat);
            request.Timeout = 0x3e8;
            request.Method = "GET";
            request.Proxy = null;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        Encoding encoding2 = Encoding.GetEncoding("utf-8");
                        string str3 = string.Empty;
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            while ((str3 = reader.ReadLine()) != null)
                            {
                                str = str + str3;
                            }
                            reader.Close();
                        }
                        stream.Close();
                    }
                    response.Close();
                }
                return str;
            }
            catch (Exception e)
            {
                string aa = e.Message;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
            return str;
        }
        /// <summary>
        /// 调用汽车在线逆地理解析
        /// </summary>
        /// <param name="plat"></param>
        /// <param name="plog"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        public static string Gpsoo(string plat, string plog, string mapType = "BAIDU")
        {
            //GOOGLE
            string jsonstr = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(" http://g.gpsoo.net/o.jsp?i=" + plog + "," + plat + "&map=" + mapType + "");
            request.Timeout = 1000;
            request.Method = "GET";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        jsonstr = sr.ReadToEnd();

                        sr.Close();
                    }
                    response.Close();
                }
                return jsonstr;
            }
            catch (Exception e)
            {

                return "";
            }

        }
        /// <summary>
        /// baidu纠偏数据库
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="connetString"></param>
        /// <param name="c_lat"></param>
        /// <param name="c_lng"></param>
        public static void Correct_PlogPlat_Baidu(double lat, double lng, string connetString, out double c_lat, out double c_lng)
        {

            string strLat = lat.ToString("0.00").Replace(".", "");
            string strLng = lng.ToString("0.00").Replace(".", "");
            DataTable dt = null;
            string commandText = "SELECT  TOP 1 [x_lng],[x_lat] ,[p_lng] ,[p_lat],[b_lng],[b_lat] FROM latlng_off_baidu WHERE x_lat='" + strLat + "'AND x_lng='" + strLng + "'";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
                    dt = SqlHelper.ExecuteDataTable(conn, commandText);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                c_lat = Convert.ToDouble(dt.Rows[0]["p_lat"].ToString()) + lat;
                c_lng = Convert.ToDouble(dt.Rows[0]["p_lng"].ToString()) + lng;
            }
            else
            {
                c_lat = lat;
                c_lng = lng;
            }
        }
        /// <summary>
        /// google，QQ地图纠偏处理
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="connetString"></param>
        /// <param name="c_lat"></param>
        /// <param name="c_lng"></param>
        public static void Correct_PlogPlat_Google(double lat, double lng, string connetString, out double c_lat, out double c_lng)
        {
            string strLat = lat.ToString("0.00").Replace(".", "");
            string strLng = lng.ToString("0.00").Replace(".", "");
            DataTable dt = null;
            string commandText = "SELECT  TOP 1 [X_LNG],[X_LAT],[OQ_LNG],[OQ_LAT],[Q_LNG],[Q_LAT] FROM x_offset WHERE x_lat='" + strLat + "'AND x_lng='" + strLng + "'";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
                    dt = SqlHelper.ExecuteDataTable(conn, commandText);
                }
                catch (Exception)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                c_lat = Convert.ToDouble(dt.Rows[0]["OQ_LNG"].ToString()) + lat;
                c_lng = Convert.ToDouble(dt.Rows[0]["OQ_LAT"].ToString()) + lng;
            }
            else
            {
                c_lat = lat;
                c_lng = lng;
            }
        }
        public static bool LocateGooleMapApi(uint cellid, uint lac, out double Lat, out double Lng)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://www.google.com/glm/mmap");
                request.Timeout = 1000;
                request.Method = "POST";
                byte[] byteArray =
                {
                    0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
                    0x65, 0x6E, // en
                    0x00, 0x07,
                    0x41, 0x6E, 0x64, 0x72, 0x6F, 0x69, 0x64,
                    0x00, 0x03,
                    0x31, 0x2E, 0x30, // 1.0
                    0x00, 0x03,
                    0x57, 0x65, 0x62, // web
                    0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, // CellID
                    0xFF, 0xFF, 0xFF, 0xFF, // LAC
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

                // write CellID
                byte[] intByte = BitConverter.GetBytes(cellid);
                byteArray[48] = intByte[3];
                byteArray[49] = intByte[2];
                byteArray[50] = intByte[1];
                byteArray[51] = intByte[0];

                // write LAC
                intByte = BitConverter.GetBytes(lac);
                byteArray[52] = intByte[3];
                byteArray[53] = intByte[2];
                byteArray[54] = intByte[1];
                byteArray[55] = intByte[0];

                // set request
                request.ContentLength = byteArray.Length;
                Stream postStream = request.GetRequestStream();
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                // Get the response.
                var response = (HttpWebResponse)request.GetResponse();

                // Read response
                Stream dataStream = response.GetResponseStream();
                var BR = new BinaryReader(dataStream);
                // skip 3 byte
                BR.ReadByte();
                BR.ReadByte();
                BR.ReadByte();

                // check state
                if (0 == BR.ReadInt32())
                {
                    // read lat
                    var tmpByte = new byte[4];
                    tmpByte[3] = BR.ReadByte();
                    tmpByte[2] = BR.ReadByte();
                    tmpByte[1] = BR.ReadByte();
                    tmpByte[0] = BR.ReadByte();
                    Lat = BitConverter.ToInt32(tmpByte, 0) / 1000000D;

                    // read lng
                    tmpByte[3] = BR.ReadByte();
                    tmpByte[2] = BR.ReadByte();
                    tmpByte[1] = BR.ReadByte();
                    tmpByte[0] = BR.ReadByte();
                    Lng = BitConverter.ToInt32(tmpByte, 0) / 1000000D;

                    BR.Close();
                    dataStream.Close();
                    response.Close();
                    return true;
                }
                BR.Close();
                dataStream.Close();
                response.Close();
                // Lat = 0;
                // Lng = 0;
                //调用聚合数据
                if (JuHePosition(cellid, lac, out Lat, out Lng))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                if (JuHePosition(cellid, lac, out Lat, out Lng))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 使用本地数据库解析转化为Baidu坐标基站数据
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="lac"></param>
        /// <param name="connetString"></param>
        /// <param name="Lat"></param>
        /// <param name="Lng"></param>
        /// <param name="address"></param>
        public static void LocateSqlServerMapApi(uint cellid, uint lac, string connetString, out double Lat, out double Lng, out string address)
        {
            DataTable dt = null;
            string commandText = "select [LAT_B],[LNG_B],[Address] FROM m_bs WHERE [LAC]='" + lac + "'AND [CID]='" + cellid + "'";
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                try
                {
                    dt = SqlHelper.ExecuteDataTable(conn, commandText);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                Lat = Convert.ToDouble(dt.Rows[0]["LNG_B"].ToString());
                Lng = Convert.ToDouble(dt.Rows[0]["LAT_B"].ToString());
                address = dt.Rows[0]["Address"].ToString();
            }
            else
            {
                Lat = 0;
                Lng = 0;
                address = "";
            }
        }
    }
}
