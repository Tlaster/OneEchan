using AnimateRaw.Extension;
using AnimateRaw.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace AnimateRaw.ViewModel
{
    public class AnimateDetailViewModel:INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public List<AnimateSetModel> SetList { get; private set; }
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
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync($"http://tlaster.me/getanimate?id={_id}");
                SetList = (from item in JsonObject.Parse(jsstr).GetNamedArray("SetList")
                           select new AnimateSetModel
                           {
                               ClickCount = item.GetNamedNumber("ClickCount"),
                               FileName = item.GetNamedString("FileName"),
                               FilePath = item.GetNamedString("FilePath"),
                           }).OrderBy(a => a.FileName).ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SetList)));
            }
        }
        internal async void Click(string fileName)
        {
            using (var client = new HttpClient())
                await client.GetStringAsync($"http://tlaster.me/getanimate?id={_id}&filename={fileName}");
        }
    }
}
