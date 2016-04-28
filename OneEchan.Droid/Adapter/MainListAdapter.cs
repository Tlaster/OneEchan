using System;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.Widget;
using OneEchan.Droid.ViewHolder;
using OneEchan.Core.Common.Api.Model;

namespace OneEchan.Droid.Adapter
{
    public class MainListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public List<AnimateListModel> Items { get; }
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

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolderBase).SetText(Resource.Id.MainListLayoutName, Items[position].Name).SetText(Resource.Id.MainListLayoutUpdateTime, Shared.Common.Helper.UpdateTimeHelper.GetUpdate(Items[position].LastUpdateTime));
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