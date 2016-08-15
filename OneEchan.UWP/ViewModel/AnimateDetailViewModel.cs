using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
using OneEchan.Shared.Common.Helper;
using OneEchan.Core.Models;
using OneEchan.Core.Api;

namespace OneEchan.ViewModel
{
    public class AnimateDetailViewModel:INotifyPropertyChanged
    {
        public string Name { get; }
        public List<DetailList> SetList { get; private set; }
        public bool IsLoading { get; private set; }
        public int ID { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AnimateDetailViewModel(int id,string name)
        {
            ID = id;
            Name = name;
            Init();
        }

        private async void Init()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                var item = await Detail.GetDetail(ID, LanguageHelper.PrefLang);
                SetList = item.List;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SetList)));
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                await new MessageDialog("Can not get the detail", "Error").ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        public void Refresh() => Init();

    }
}
