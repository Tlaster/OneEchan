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

namespace AnimateRaw.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public List<AnimateListModel> RawList { get; private set; }
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
                using (var client = new HttpClient())
                {
                    var jsstr = await client.GetStringAsync("http://tlaster.me/getanimate");
                    RawList = (from item in JsonArray.Parse(jsstr)
                               select new AnimateListModel
                               {
                                   ID = item.GetNamedNumber("ID"),
                                   Name = item.GetNamedString("Name"),
                                   LastUpdate = DateTime.Now - DateTime.Parse(item.GetNamedString("LastUpdate")),
                               }).OrderBy(a => a.LastUpdate).ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawList)));
                    await GetFavorList();
                }
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                await new MessageDialog("Can not get the list", "Error").ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        internal async Task GetFavorList()
        {
            FavorList = (from id in await FavorHelper.GetFavorList()
                         from item in RawList
                         where item.ID == id
                         select item).ToList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorList)));
        }

        public void Refresh()
        {
            Init();
        }
    }
}
