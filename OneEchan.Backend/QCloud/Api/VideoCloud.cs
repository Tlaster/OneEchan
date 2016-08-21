using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using OneEchan.Backend.QCloud.Common;
using System.Net.Http;

namespace OneEchan.Backend.QCloud.Api
{
    public enum FolderPattern { File = 0, Folder, Both };
    public class VideoCloud
    {
        const string VIDEOAPI_CGI_URL = "http://web.video.myqcloud.com/files/v1/";
        private int appId;
        private string secretId;
        private string secretKey;
        private int timeOut;

        /// <summary>
        /// VideoCloud 构造方法
        /// </summary>
        /// <param name="appId">授权appid</param>
        /// <param name="secretId">授权secret id</param>
        /// <param name="secretKey">授权secret key</param>
        /// <param name="timeOut">网络超时,默认60秒</param>
        public VideoCloud(int appId, string secretId, string secretKey, int timeOut = 60)
        {
            this.appId = appId;
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.timeOut = timeOut * 1000;
        }

        /// <summary>
        /// 远程路径Encode处理
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        private string EncodeRemotePath(string remotePath)
        {
            if (remotePath == "/")
            {
                return remotePath;
            }
            var endWith = remotePath.EndsWith("/");
            string[] part = remotePath.Split('/');
            remotePath = "";
            foreach (var s in part)
            {
                if (s != "")
                {
                    if (remotePath != "")
                    {
                        remotePath += "/";
                    }
                    remotePath += WebUtility.UrlEncode(s);
                }
            }
            remotePath = (remotePath.StartsWith("/") ? "" : "/") + remotePath + (endWith ? "/" : "");
            return remotePath;
        }

        /// <summary>
        /// 标准化远程路径
        /// </summary>
        /// <param name="remotePath">要标准化的远程路径</param>
        /// <returns></returns>
        private string StandardizationRemotePath(string remotePath)
        {
            if (!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }
            if (!remotePath.EndsWith("/"))
            {
                remotePath += "/";
            }
            return remotePath;
        }

        /// <summary>
        /// 更新文件夹信息
        /// </summary>
        /// <param name="bucketName"> bucket名称</param>
        /// <param name="remotePath">远程文件夹路径</param>
        /// <param name="bizAttribute">更新信息</param>
        /// <returns></returns>
        public async Task<string> UpdateFolder(string bucketName, string remotePath, string bizAttribute)
        {
            remotePath = StandardizationRemotePath(remotePath);
            return await UpdateFile(bucketName, remotePath, bizAttribute, null, null);
        }

        /// <summary>
        /// 更新文件信息
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="bizAttribute">更新信息</param>
        /// <param name="title">标题</param>
        /// <param name="desc">描述</param>
        /// <returns></returns>
        public async Task<string> UpdateFile(string bucketName, string remotePath, string bizAttribute, string videoCover = null, string title = null, string desc = null)
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            int flag = 0;
            if (title != null && desc != null && bizAttribute != null && videoCover != null)
            {
                flag = 0x0f;
            }
            else
            {
                if (title != null)
                {
                    flag |= 0x02;
                }
                if (desc != null)
                {
                    flag |= 0x04;
                }
                if (bizAttribute != null)
                {
                    flag |= 0x01;
                }
                if (videoCover != null)
                {
                    flag |= 0x08;
                }
            }
            var data = new Dictionary<string, string>();
            data.Add("op", "update");
            data.Add("biz_attr", bizAttribute);
            if (videoCover != null)
            {
                data.Add("video_cover", videoCover);
            }
            if (title != null)
            {
                data.Add("video_title", title);
            }
            if (desc != null)
            {
                data.Add("video_desc", desc);
            }
            data.Add("flag", flag.ToString());

            var sign = Sign.SignatureOnce(appId, secretId, secretKey, (remotePath.StartsWith("/") ? "" : "/") + remotePath, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            header.Add("Content-Type", "application/json");
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut);
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件夹路径</param>
        /// <returns></returns>
        public async Task<string> DeleteFolder(string bucketName, string remotePath)
        {
            remotePath = StandardizationRemotePath(remotePath);
            return await DeleteFile(bucketName, remotePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <returns></returns>
        public async Task<string> DeleteFile(string bucketName, string remotePath)
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var data = new Dictionary<string, string>();
            data.Add("op", "delete");
            var sign = Sign.SignatureOnce(appId, secretId, secretKey, (remotePath.StartsWith("/") ? "" : "/") + remotePath, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            header.Add("Content-Type", "application/json");
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut);
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件夹路径</param>
        /// <returns></returns>
        public async Task<string> GetFolderStat(string bucketName, string remotePath)
        {
            remotePath = StandardizationRemotePath(remotePath);
            return await GetFileStat(bucketName, remotePath);
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <returns></returns>
        public async Task<string> GetFileStat(string bucketName, string remotePath)
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var data = new Dictionary<string, string>();
            data.Add("op", "stat");
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            return await Request.SendRequest(url, data, HttpMethod.Get, header, timeOut);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件夹路径</param>
        /// <param name="bizAttribute">附加信息</param>
        /// <returns></returns>
        public async Task<string> CreateFolder(string bucketName, string remotePath, string bizAttribute = "")
        {
            remotePath = StandardizationRemotePath(remotePath);
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var data = new Dictionary<string, string>();
            data.Add("op", "create");
            data.Add("biz_attr", bizAttribute);
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            header.Add("Content-Type", "application/json");
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut);
        }

        /// <summary>
        /// 目录列表,前缀搜索
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件夹路径</param>
        /// <param name="num">拉取的总数</param>
        /// <param name="context">透传字段,用于翻页,前端不需理解,需要往前/往后翻页则透传回来</param>
        /// <param name="order">默认正序(=0), 填1为反序</param>
        /// <param name="pattern">拉取模式:只是文件，只是文件夹，全部</param>
        /// <param name="prefix">读取文件/文件夹前缀</param>
        /// <returns></returns>
        public async Task<string> GetFolderList(string bucketName, string remotePath, int num, string context, int order, FolderPattern pattern, string prefix = "")
        {
            remotePath = StandardizationRemotePath(remotePath);
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath) + WebUtility.UrlEncode(prefix);
            var data = new Dictionary<string, string>();
            data.Add("op", "list");
            data.Add("num", num.ToString());
            data.Add("context", context);
            data.Add("order", order.ToString());
            string[] patternArray = { "eListFileOnly", "eListDirOnly", "eListBoth" };
            data.Add("pattern", patternArray[(int)pattern]);
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            return await Request.SendRequest(url, data, HttpMethod.Get, header, timeOut);
        }

        /// <summary>
        /// 单个文件上传
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="bizAttribute">附加信息</param>
        /// <param name="title">标题</param>
        /// <param name="desc">描述</param>
        /// <param name="magicContext">透传字段，业务设置回调url的话，会把这个字段通过回调url传给业务</param>
        /// <returns></returns>
        public async Task<string> UploadFile(string bucketName, string remotePath, string localPath, string videoCover = "", string bizAttribute = "", string title = "", string desc = "", string magicContext = "")
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var sha1 = SHA1.GetSHA1(localPath);
            var data = new Dictionary<string, string>();
            data.Add("op", "upload");
            data.Add("sha", sha1);
            data.Add("video_cover", videoCover);
            data.Add("video_title", title);
            data.Add("video_desc", desc);
            data.Add("magicContext", magicContext);
            data.Add("biz_attr", bizAttribute);
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut, localPath);
        }

        /// <summary>
        /// 分片上传第一步
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="videoCover">视频封面Url</param>
        /// <param name="bizAttribute">附加信息</param>
        /// <param name="title">标题</param>
        /// <param name="desc">描述</param>
        /// <param name="magicContext">透传字段，业务设置回调url的话，会把这个字段通过回调url传给业务</param>
        /// <param name="sliceSize">切片大小（字节）</param>
        /// <returns></returns>
        public async Task<string> SliceUploadFirstStep(string bucketName, string remotePath, string localPath, string videoCover, string bizAttribute, string title,
            string desc, string magicContext, int sliceSize)
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var sha1 = SHA1.GetSHA1(localPath);
            var fileSize = new FileInfo(localPath).Length;
            var data = new Dictionary<string, string>();
            data.Add("op", "upload_slice");
            data.Add("sha", sha1);
            data.Add("filesize", fileSize.ToString());
            data.Add("slice_size", sliceSize.ToString());
            data.Add("video_cover", videoCover);
            data.Add("video_title", title);
            data.Add("video_desc", desc);
            data.Add("magicContext", magicContext);
            data.Add("biz_attr", bizAttribute);
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut);
        }

        /// <summary>
        /// 分片上传后续步骤
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="sessionId">分片上传会话ID</param>
        /// <param name="offset">文件分片偏移量</param>
        /// <param name="sliceSize">切片大小（字节）</param>
        /// <returns></returns>
        public async Task<string> SliceUploadFollowStep(string bucketName, string remotePath, string localPath,
                string sessionId, int offset, int sliceSize)
        {
            var url = VIDEOAPI_CGI_URL + appId + "/" + bucketName + EncodeRemotePath(remotePath);
            var data = new Dictionary<string, string>();
            data.Add("op", "upload_slice");
            data.Add("session", sessionId);
            data.Add("offset", offset.ToString());
            var expired = DateTime.Now.ToUnixTime() / 1000 + 60;
            var sign = Sign.Signature(appId, secretId, secretKey, expired, bucketName);
            var header = new Dictionary<string, string>();
            header.Add("Authorization", sign);
            return await Request.SendRequest(url, data, HttpMethod.Post, header, timeOut, localPath, offset, sliceSize);
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="videoCover">视频封面Url</param>
        /// <param name="bizAttribute">附加信息</param>
        /// <param name="title">标题</param>
        /// <param name="desc">描述</param>
        /// <param name="magicContext">透传字段，业务设置回调url的话，会把这个字段通过回调url传给业务</param>
        /// <param name="sliceSize">切片大小（字节）</param>
        /// <returns></returns>
        public async Task<string> SliceUploadFile(string bucketName, string remotePath, string localPath, string videoCover = "", string bizAttribute = "",
            string title = "", string desc = "", string magicContext = "", int sliceSize = 512 * 1024)
        {
            var result = await SliceUploadFirstStep(bucketName, remotePath, localPath, videoCover, bizAttribute, title, desc, magicContext, sliceSize);
            var obj = (JObject)JsonConvert.DeserializeObject(result);
            var code = (int)obj["code"];
            if (code != 0)
            {
                return result;
            }
            var data = obj["data"];
            if (data["access_url"] != null)
            {
                var accessUrl = data["access_url"];
                //Console.WriteLine("命中秒传：" + accessUrl);
                return result;
            }
            else
            {
                var sessionId = data["session"].ToString();
                sliceSize = (int)data["slice_size"];
                var offset = (int)data["offset"];
                var retryCount = 0;
                while (true)
                {
                    result = await SliceUploadFollowStep(bucketName, remotePath, localPath, sessionId, offset, sliceSize);
                    //Console.WriteLine(result);
                    obj = (JObject)JsonConvert.DeserializeObject(result);
                    code = (int)obj["code"];
                    if (code != 0)
                    {
                        //当上传失败后会重试3次
                        if (retryCount < 3)
                        {
                            //retryCount++;
                            //Console.WriteLine("重试....");
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        data = obj["data"];
                        if (data["offset"] != null)
                        {
                            offset = (int)data["offset"] + sliceSize;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return "";
        }

    }

}
