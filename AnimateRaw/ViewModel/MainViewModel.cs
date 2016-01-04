using AnimateRaw.Extension;
using AnimateRaw.Shared.Helper;
using AnimateRaw.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;
using AnimateRaw.Common;

namespace AnimateRaw.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public AnimateListIncrementalLoadingClass RawList { get; private set; }
        public List<AnimateListModel> FavorList { get; private set; }
        public bool IsLoading { get; private set; }
        public MainViewModel()
        {
            Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Init()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                RawList = new AnimateListIncrementalLoadingClass();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawList)));
                //await GetRawList();
                await GetFavorList();
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                await new MessageDialog("Can not get the favor list", "Error").ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        //private async Task GetRawList()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var jsstr = await client.GetStringAsync("http://tlaster.me/list");
        //        var obj = JsonObject.Parse(jsstr);
        //        if (obj.GetNamedBoolean("Success"))
        //        {
        //            RawList = (from item in obj.GetNamedArray("List")
        //                       select new AnimateListModel
        //                       {
        //                           ID = item.GetNamedNumber("ID"),
        //                           Name = item.GetNamedString("Name"),
        //                           LastUpdateBeijing = DateTime.Parse(item.GetNamedString("LastUpdate")),
        //                       }).OrderBy(a => a.LastUpdate).ToList();
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawList)));
        //        }
        //    }
        //}

        internal async Task GetFavorList()
        {
            var str = await FavorHelper.GetFavorString();
            if (str.Length == 0)
            {
                return;
            }
            using (var client = new HttpClient())
            {
                using (var res = await client.PostAsync("http://tlaster.me/api/list", new StringContent(str)))
                {
                    var ret = await res.Content.ReadAsStringAsync();
                    FavorList = (from item in JsonArray.Parse(ret)
                                 select new AnimateListModel
                                 {
                                     ID = item.GetNamedNumber("ID"),
                                     Name = item.GetNamedString("Name"),
                                     LastUpdateBeijing = DateTime.Parse(item.GetNamedString("LastUpdate")),
                                 }).OrderBy(a=>a.LastUpdate).ToList();
                }   
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorList)));
        }

        public void Refresh()
        {
            Init();
        }
    }
}
