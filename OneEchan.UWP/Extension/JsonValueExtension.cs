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
        public static JsonArray GetNamedArray(this IJsonValue value, string name)
        {
            try
            {
                return value.GetObject()[name].GetArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JsonArray GetNamedArray(this IJsonValue value, string name, JsonArray defaultValue)
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

        public static JsonObject GetNamedObject(this IJsonValue value, string name)
        {
            try
            {
                return value.GetObject()[name].GetObject();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JsonObject GetNamedObject(this IJsonValue value, string name, JsonObject defaultValue)
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

        public static double GetNamedNumber(this IJsonValue value, string name)
        {
            try
            {
                return value.GetObject()[name].GetNumber();
            }
            catch (Exception)
            {
                return -1d;
            }
        }

        public static double GetNamedNumber(this IJsonValue value, string name, double defaultValue)
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

        public static bool GetNamedBoolean(this IJsonValue value, string name)
        {
            try
            {
                return value.GetObject()[name].GetBoolean();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool GetNamedBoolean(this IJsonValue value, string name, bool defaultValue)
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

        public static string GetNamedString(this IJsonValue value, string name)
        {
            try
            {
                return value.GetObject()[name].GetString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetNamedString(this IJsonValue value, string name, string defaultValue)
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
