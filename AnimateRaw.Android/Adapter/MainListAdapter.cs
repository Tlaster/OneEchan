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

namespace AnimateRaw.Android.Adapter
{
    public class MainListAdapter : BaseAdapter<AnimateListModel>
    {
        public List<AnimateListModel> Items { get; private set; }
        private Activity _context;
        public MainListAdapter(Activity context, List<AnimateListModel> items) : base()
        {
            this._context = context;
            this.Items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override AnimateListModel this[int position]
        {
            get { return Items[position]; }
        }
        public override int Count
        {
            get { return Items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = _context.LayoutInflater.Inflate(Resource.Layout.MainListLayout, null);
            view.FindViewById<TextView>(Resource.Id.MainListLayoutName).Text = Items[position].Name;
            view.FindViewById<TextView>(Resource.Id.MainListLayoutUpdateTime).Text = GetUpdate(Items[position].LastUpdate);
            return view;
        }
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
    }
}