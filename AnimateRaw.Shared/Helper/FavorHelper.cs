using AnimateRaw.Shared.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static AnimateRaw.Shared.Helper.JsonHelper;
#if WINDOWS_UWP
using Windows.Storage;
#else
using Android.App;
using Android.Content;
#endif

namespace AnimateRaw.Shared.Helper
{
    internal static class FavorHelper
    {
        private const string SAVE_DATABASE_NAME = "Favor.json";

#if WINDOWS_UWP
        public static async Task<IEnumerable<double>> GetFavorList()
#else
        public static IEnumerable<double> GetFavorList()
#endif
        {
#if WINDOWS_UWP
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
            var path = Application.Context.GetDatabasePath(SAVE_DATABASE_NAME).AbsolutePath;
            var folder = Path.GetDirectoryName(path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var jsStr = File.ReadAllText(path);
#endif
            return jsStr != "" ? FromJson<List<double>>(jsStr) : new List<double>();
        }

#if WINDOWS_UWP
        public static async Task AddFavor(double item)
#else
        public static void AddFavor(double item)
#endif
        {
#if WINDOWS_UWP
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
            var path = Application.Context.GetDatabasePath(SAVE_DATABASE_NAME).AbsolutePath;
            var folder = Path.GetDirectoryName(path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var jsStr = File.ReadAllText(path);
#endif
            var items = jsStr != "" ? FromJson<List<double>>(jsStr) : new List<double>();
            var index = items.FindIndex((a) => a == item);
            if (index == -1)
            {
                items.Add(item);
                jsStr = ToJson(items);
#if WINDOWS_UWP
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
                File.WriteAllText(path, jsStr);
#endif
            }
        }


#if WINDOWS_UWP
        public static async Task RemoveFavor(double item)
#else
        public static void RemoveFavor(double item)
#endif
        {
#if WINDOWS_UWP
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
            var path = Application.Context.GetDatabasePath(SAVE_DATABASE_NAME).AbsolutePath;
            var folder = Path.GetDirectoryName(path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var jsStr = File.ReadAllText(path);
#endif
            var items = jsStr != "" ? FromJson<List<double>>(jsStr) : new List<double>();
            var index = items.FindIndex((a) => a == item);
            if (index != -1)
            {
                items.RemoveAt(index);
                jsStr = ToJson(items);
#if WINDOWS_UWP
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
                File.WriteAllText(path, jsStr);
#endif
            }
        }

#if WINDOWS_UWP
        public static async Task<bool> IsFavor(double item)
#else
        public static bool IsFavor(double item)
#endif
        {
#if WINDOWS_UWP
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
#else
            var path = Application.Context.GetDatabasePath(SAVE_DATABASE_NAME).AbsolutePath;
            var folder = Path.GetDirectoryName(path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var jsStr = File.ReadAllText(path);
#endif
            var items = jsStr != "" ? FromJson<List<double>>(jsStr) : new List<double>();
            var index = items.FindIndex((a) => a == item);
            return index != -1;
        }
    }
}
