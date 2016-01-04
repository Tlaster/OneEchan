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
using Android.Support.V7.Widget;

namespace AnimateRaw.Android.ViewHolder
{
    public class DetailViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }
        public TextView ClickCount { get; private set; }
        public ImageView Image { get; private set; }


        public DetailViewHolder(View itemView) : base (itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.DetailListLayoutName);
            ClickCount = itemView.FindViewById<TextView>(Resource.Id.DetailListLayoutClickCount);
            Image = ItemView.FindViewById<ImageView>(Resource.Id.DetailListImage);
        }
    }
}