using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Backend.QCloud.Common
{
    internal class SHA1
    {
        public static string GetSHA1(string filePath)
        {
            var strResult = "";
            var strHashData = "";
            byte[] arrbytHashValue;
            var sha1 = System.Security.Cryptography.SHA1.Create();
            try
            {
                using (var oFileStream = new FileStream(filePath.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    arrbytHashValue = sha1.ComputeHash(oFileStream); //计算指定Stream 对象的哈希值
                    //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                    strHashData = BitConverter.ToString(arrbytHashValue);
                    //替换-
                    strHashData = strHashData.Replace("-", "");
                    strResult = strHashData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return strResult;
        }
    }
}
