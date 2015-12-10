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
    public class DetailListAdapter : BaseAdapter<AnimateSetModel>
    {
        public List<AnimateSetModel> Items { get; private set; }
        private Activity _context;
        public DetailListAdapter(Activity context, List<AnimateSetModel> items) : base()
        {
            this._context = context;
            this.Items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override AnimateSetModel this[int position]
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
            view.FindViewById<TextView>(Resource.Id.MainListLayoutName).Text = Items[position].FileName;
            view.FindViewById<TextView>(Resource.Id.MainListLayoutUpdateTime).Text = $"Click Count:{Items[position].ClickCount}";
            return view;
        }

    }
}