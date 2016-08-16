using System;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.Widget;
using OneEchan.Droid.ViewHolder;
using Android.Graphics;
using System.Net.Http;
using OneEchan.Core.Models;

namespace OneEchan.Droid.Adapter
{
    public class DetailListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
		public List<DetailList> Items { get; }
		public DetailListAdapter(List<DetailList> items) : base()
        {
            Items = items;
        }
        public override long GetItemId(int position) => position;

        public override int ItemCount => Items.Count;

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as ViewHolderBase;
			vh.SetText(Resource.Id.DetailListLayoutName,Items[position].Set.ToString()).SetText(Resource.Id.DetailListLayoutClickCount, $"{Items[position].ClickCount} Views");
            if (string.IsNullOrEmpty(Items[position].FileThumb))
            {
                vh.SetBackgroundColor(Resource.Id.DetailListImage, Color.MediumVioletRed).SetImageResource(Resource.Id.DetailListImage, Resource.Drawable.SplashScreen);
            }
            else
            {
                vh.SetImageBitmap(Resource.Id.DetailListImage, await GetImageBitmapFromUrl(Items[position].FileThumb));
            }
        }
        private async System.Threading.Tasks.Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception)
            {

            }
            return imageBitmap;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
               Inflate(Resource.Layout.DetailListLayout, parent, false);
            var vh = new ViewHolderBase(itemView, Resource.Id.DetailListLayoutName, Resource.Id.DetailListLayoutClickCount, Resource.Id.DetailListImage);
            itemView.Click += (s, e) => ItemClick?.Invoke(s, vh.LayoutPosition);
            return vh;
        }
    }
}