using AnimateRaw.Extension;
using AnimateRaw.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Windows.Data.Json;
using Windows.UI.Popups;
using AnimateRaw.Shared;

namespace AnimateRaw.ViewModel
{
    public class AnimateDetailViewModel:INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public List<AnimateSetModel> SetList { get; private set; }
        public bool IsLoading { get; private set; }
        private double _id;

        public event PropertyChangedEventHandler PropertyChanged;

        public AnimateDetailViewModel(double id,string name)
        {
            _id = id;
            Name = name;
            Init();
        }

        private async void Init()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                using (var client = new HttpClient())
                {
                    var jsstr = await client.GetStringAsync($"http://oneechan.moe/api/detail?id={_id}&prefLang={LanguageHelper.PrefLang}");
                    var obj = JsonObject.Parse(jsstr);
                    SetList = (from item in obj.GetNamedArray("SetList")
                               select new AnimateSetModel
                               {
                                   ClickCount = item.GetNamedNumber("ClickCount"),
                                   FileName = item.GetNamedString("FileName"),
                                   FilePath = item.GetNamedString("FilePath"),
                                   FileThumb = item.GetNamedString("FileThumb"),
                               }).OrderBy(a => a.FileName).ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SetList)));
                }
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                await new MessageDialog("Can not get the detail", "Error").ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        public void Refresh() => Init();
        internal async void Click(string fileName)
        {
            using (var client = new HttpClient())
                await client.GetStringAsync($"http://oneechan.moe/api/detail?id={_id}&filename={fileName}&prefLang={LanguageHelper.PrefLang}");
        }
    }
}
