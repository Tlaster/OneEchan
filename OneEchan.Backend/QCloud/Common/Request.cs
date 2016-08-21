using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OneEchan.Backend.QCloud.Common
{
    //internal enum HttpMethod { Get, Post };
    /// <summary>
    /// 请求调用类
    /// </summary>
    internal class Request
    {
        public static async Task<string> SendRequest(string url, Dictionary<string, string> data, HttpMethod requestMethod, Dictionary<string, string> header, int timeOut, string localPath = null, long offset = -1, int sliceSize = 0)
        {
            try
            {
                //ServicePointManager.Expect100Continue = false;
                if (requestMethod == HttpMethod.Get)
                {
                    var paramStr = "";
                    foreach (var key in data.Keys)
                    {
                        paramStr += $"{key}={WebUtility.UrlEncode(data[key].ToString())}&";
                    }
                    paramStr = paramStr.TrimEnd('&');
                    url += $"{(url.EndsWith("?") ? "&" : "?")}{paramStr}";
                }
                HttpWebRequest request;
                request = WebRequest.CreateHttp(url);
                request.Accept = "*/*";
                request.Headers[HttpRequestHeader.Connection] = "Keep-Alive";
                request.Headers[HttpRequestHeader.UserAgent] = "qcloud-dotnet-sdk";
                request.ContinueTimeout = timeOut;
                foreach (var key in header.Keys)
                {
                    if (key == "Content-Type")
                    {
                        request.ContentType = header[key];
                    }
                    else
                    {
                        request.Headers[key] = header[key];
                    }
                }
                if (requestMethod == HttpMethod.Post)
                {
                    request.Method = requestMethod.ToString().ToUpper();
                    using (var memStream = new MemoryStream())
                    {
                        if (header.ContainsKey("Content-Type") && header["Content-Type"] == "application/json")
                        {
                            var json = JsonConvert.SerializeObject(data);
                            var jsonByte = Encoding.GetEncoding("utf-8").GetBytes(json.ToString());
                            await memStream.WriteAsync(jsonByte, 0, jsonByte.Length);
                        }
                        else
                        {
                            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                            var beginBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                            var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                            request.ContentType = "multipart/form-data; boundary=" + boundary;

                            var strBuf = new StringBuilder();
                            foreach (var key in data.Keys)
                            {
                                strBuf.Append("\r\n--" + boundary + "\r\n");
                                strBuf.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n");
                                strBuf.Append(data[key].ToString());
                            }
                            var paramsByte = Encoding.GetEncoding("utf-8").GetBytes(strBuf.ToString());
                            await memStream.WriteAsync(paramsByte, 0, paramsByte.Length);

                            if (localPath != null)
                            {
                                await memStream.WriteAsync(beginBoundary, 0, beginBoundary.Length);
                                var fileInfo = new FileInfo(localPath);

                                using (var fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                                {
                                    const string filePartHeader =
                                        "Content-Disposition: form-data; name=\"fileContent\"; filename=\"{0}\"\r\n" +
                                        "Content-Type: application/octet-stream\r\n\r\n";
                                    var headerText = string.Format(filePartHeader, fileInfo.Name);
                                    var headerbytes = Encoding.UTF8.GetBytes(headerText);
                                    await memStream.WriteAsync(headerbytes, 0, headerbytes.Length);

                                    if (offset == -1)
                                    {
                                        var buffer = new byte[1024];
                                        int bytesRead;
                                        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                                        {
                                            await memStream.WriteAsync(buffer, 0, bytesRead);
                                        }
                                    }
                                    else
                                    {
                                        var buffer = new byte[sliceSize];
                                        int bytesRead;
                                        fileStream.Seek(offset, SeekOrigin.Begin);
                                        bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                                        await memStream.WriteAsync(buffer, 0, bytesRead);
                                    }
                                }
                            }
                            await memStream.WriteAsync(endBoundary, 0, endBoundary.Length);
                        }
                        request.Headers[HttpRequestHeader.ContentLength] = memStream.Length.ToString();
                        using (var requestStream = await request.GetRequestStreamAsync())
                        {
                            memStream.Position = 0;
                            var tempBuffer = new byte[memStream.Length];
                            await memStream.ReadAsync(tempBuffer, 0, tempBuffer.Length);
                            await requestStream.WriteAsync(tempBuffer, 0, tempBuffer.Length);
                        }
                    }
                }
                var response = await request.GetResponseAsync();
                using (var s = response.GetResponseStream())
                {
                    var reader = new StreamReader(s, Encoding.UTF8);
                    return await reader.ReadToEndAsync();
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    using (var s = we.Response.GetResponseStream())
                    {
                        var reader = new StreamReader(s, Encoding.UTF8);
                        return await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    throw we;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
