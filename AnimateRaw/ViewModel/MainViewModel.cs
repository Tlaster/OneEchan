using System.ComponentModel;
using AnimateRaw.Common;

namespace AnimateRaw.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public AnimateListIncrementalLoadingClass RawList { get; private set; }
        public bool IsLoading { get; private set; }
        public MainViewModel()
        {
            Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Init()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            RawList = new AnimateListIncrementalLoadingClass();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawList)));
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        public void Refresh()
        {
            Init();
        }
    }
}
