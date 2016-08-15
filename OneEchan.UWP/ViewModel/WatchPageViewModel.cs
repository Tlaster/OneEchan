using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OneEchan.Core.Api;
using OneEchan.Core.Models;
using OneEchan.Shared.Common.Helper;
using PropertyChanged;
using Windows.UI.Xaml.Controls;

namespace OneEchan.UWP.ViewModel
{
    //[ImplementPropertyChanged]
    public class WatchPageViewModel : INotifyPropertyChanged
    {
        public WatchPageViewModel(int id, double set)
        {
            ID = id;
            Set = set;
            Init();
        }
        private async void Init()
        {
            IsLoading = true;
            Item = await Detail.GetVideo(ID, Set, LanguageHelper.PrefLang);
            SelectedQualityIndex = 0;
            IsLoading = false;
        }

        public int ID { get; }
        public double Set { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private SetResult _item;

        public SetResult Item
        {
            get { return _item; }
            set { _item = value; OnPropertyChanged(nameof(QualityList)); }
        }

        public Dictionary<string, string> QualityList => Item?.ToDictionary();

        private int _selectedQualityIndex = -1;

        public int SelectedQualityIndex
        {
            get { return _selectedQualityIndex; }
            set { _selectedQualityIndex = value; OnPropertyChanged(); OnPropertyChanged(nameof(Source)); }
        }
        
        public Uri Source => new Uri(QualityList?.Values?.ToList()[QualityList.Count > 0 && SelectedQualityIndex == -1 ? 0 : SelectedQualityIndex] ?? "ms-appx:///Assets/SplashScreen.png");
    }
}
