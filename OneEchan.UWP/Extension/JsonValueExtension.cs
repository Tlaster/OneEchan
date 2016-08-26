using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace OneEchan.Extension
{
    public static class JsonValueExtension
    {
        public static JsonArray GetNamedArray(this IJsonValue value, string name, JsonArray defaultValue = null)
        {
            try
            {
                return value.GetObject()[name].GetArray();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static JsonObject GetNamedObject(this IJsonValue value, string name, JsonObject defaultValue = null)
        {
            try
            {
                return value.GetObject()[name].GetObject();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static double GetNamedNumber(this IJsonValue value, string name, double defaultValue = -1d)
        {
            try
            {
                return value.GetObject()[name].GetNumber();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static bool GetNamedBoolean(this IJsonValue value, string name, bool defaultValue = false)
        {
            try
            {
                return value.GetObject()[name].GetBoolean();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string GetNamedString(this IJsonValue value, string name, string defaultValue = null)
        {
            try
            {
                return value.GetObject()[name].GetString();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
