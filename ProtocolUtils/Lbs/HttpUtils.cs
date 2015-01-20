using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ProtocolUtils.Lbs
{
    public class HttpUtils
    {
        /// <summary>
        /// Sending POST request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static string HTTP_POST(string Url, string Data)
        {
            string Out = String.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
            try
            {
                req.Method = "POST";
                req.Timeout = 1000;
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;
                using (System.IO.Stream sendStream = req.GetRequestStream())
                {
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();
                }
                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream ReceiveStream = res.GetResponseStream();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8))
                {
                    //Char[] read = new Char[256];
                    //int count = sr.Read(read, 0, 256);

                    //while (count > 0)
                    //{
                    //    String str = new String(read, 0, count);
                    //    Out += str;
                    //    count = sr.Read(read, 0, 256);
                    //}
                    //读取接收的数据
                    Out = sr.ReadToEnd();
                    sr.Close();
                    ReceiveStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            return Out;
        }

        /// <summary>
        /// Sending GET request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static string HTTP_GET(string Url, string Data)
        {
            string Out = String.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + (string.IsNullOrEmpty(Data) ? "" : "?" + Data));
            try
            {
                System.Net.WebResponse resp = req.GetResponse();
                using (System.IO.Stream stream = resp.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                    {
                        Out = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            return Out;
        }
    }
}
