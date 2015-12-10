using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AnimateRaw.Shared.Helper
{

    internal static class JsonHelper
    {
        internal static string ToJson<T>(T data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, data);
                var jsonArray = ms.ToArray();
                return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
            }
        }

        internal static T FromJson<T>(string json)
        {
            var deserializer = new DataContractJsonSerializer(typeof(T));
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    return (T)deserializer.ReadObject(ms);
            }
            catch (SerializationException ex)
            {
                throw new SerializationException("Unable to deserialize JSON: " + json, ex);
            }
        }
    }
}
