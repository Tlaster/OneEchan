using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
using OneEchan.Shared;
using OneEchan.Core.Common.Api.Model;

namespace OneEchan.ViewModel
{
    public class AnimateDetailViewModel:INotifyPropertyChanged
    {
        public string Name { get; }
        public List<AnimateSetModel> SetList { get; private set; }
        public bool IsLoading { get; private set; }
        private int _id;

        public event PropertyChangedEventHandler PropertyChanged;

        public AnimateDetailViewModel(int id,string name)
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
                var item = await Core.Common.Api.Detail.GetDetail(_id, LanguageHelper.PrefLang);
                SetList = item.SetList;
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
        internal async void Click(int fileName)
        {
            await Core.Common.Api.Detail.AddClick(_id, fileName, LanguageHelper.PrefLang);
        }
    }
}
