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

namespace AnimateRaw.Android
{
    public class MainViewHolder: RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }
        public TextView UpdateTime { get; private set; }


        public MainViewHolder(View itemView) : base (itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.MainListLayoutName);
            UpdateTime = itemView.FindViewById<TextView>(Resource.Id.MainListLayoutUpdateTime);
        }

    }
}