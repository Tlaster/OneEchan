using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AnimateRaw.Shared.Model;
using System.Collections.ObjectModel;
using Android.Support.V7.Widget;
using AnimateRaw.Android.ViewHolder;

namespace AnimateRaw.Android.Adapter
{
    public class MainListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public List<AnimateListModel> Items { get; private set; }
        public MainListAdapter(List<AnimateListModel> items) : base()
        {
            Items = items;
        }
        public void Add(List<AnimateListModel> list)
        {
            Items.AddRange(list);
            NotifyDataSetChanged();
        }
        public override long GetItemId(int position) => position;

        public override int ItemCount => Items.Count;

        private string GetUpdate(TimeSpan time)
        {
            if (time.Days != 0)
            {
                return $"{time.Days} days ago";
            }
            if (time.Hours != 0)
            {
                return $"{time.Hours} hours ago";
            }
            if (time.Minutes != 0)
            {
                return $"{time.Minutes} minutes ago";
            }
            return "Just now";
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolderBase).SetText(Resource.Id.MainListLayoutName, Items[position].Name).SetText(Resource.Id.MainListLayoutUpdateTime, GetUpdate(Items[position].LastUpdate));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
               Inflate(Resource.Layout.MainListLayout, parent, false);
            var vh = new ViewHolderBase(itemView, Resource.Id.MainListLayoutName, Resource.Id.MainListLayoutUpdateTime);
            itemView.Click += (s, e) => ItemClick?.Invoke(s, vh.LayoutPosition);
            return vh;
        }
    }
}