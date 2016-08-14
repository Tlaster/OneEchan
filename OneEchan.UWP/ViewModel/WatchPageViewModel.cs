using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneEchan.Core.Common.Api.Model;
using OneEchan.Shared.Common.Helper;
using Nito.AsyncEx;

namespace OneEchan.UWP.ViewModel
{
    public class WatchPageViewModel
    {
        public WatchPageViewModel(int id, double set)
        {
            ID = id;
            Set = set;
            Item = NotifyTaskCompletion.Create(Core.Common.Api.Detail.GetVideo(id, set, LanguageHelper.PrefLang));
        }

        public int ID { get; }
        public double Set { get; }
        public INotifyTaskCompletion<SetResult> Item { get; }
    }
}
