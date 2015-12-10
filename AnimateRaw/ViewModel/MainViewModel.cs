using AnimateRaw.Extension;
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
        public List<AnimateListModel> RawList { get; set; }
        public MainViewModel()
        {
            Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Init()
        {
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
                }
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                await new MessageDialog("Can not get the list", "Error").ShowAsync();
            }
        }
        public void Refresh()
        {
            Init();
        }
    }
}
