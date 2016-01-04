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
using Windows.UI.Xaml.Controls;

namespace AnimateRaw.ViewModel
{
    public class AnimateDetailViewModel:INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public List<AnimateSetModel> SetList { get; private set; }
        public bool IsFavor { get; private set; }
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
                    var jsstr = await client.GetStringAsync($"http://tlaster.me/api/detail?id={_id}");
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
                    IsFavor = await FavorHelper.IsFavor(_id);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
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
        public async void FavorClick()
        {
            if (IsFavor)
                await FavorHelper.RemoveFavor(_id);
            else
                await FavorHelper.AddFavor(_id);
            IsFavor = await FavorHelper.IsFavor(_id);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
        }
        internal async void Click(string fileName)
        {
            using (var client = new HttpClient())
                await client.GetStringAsync($"http://tlaster.me/api/detail?id={_id}&filename={fileName}");
        }
    }
}
