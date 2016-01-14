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
using Android.Support.V7.Widget;
using AnimateRaw.Android.ViewHolder;
using Android.Graphics;
using System.Net.Http;
using Android.Support.V7.App;

namespace AnimateRaw.Android.Adapter
{
    public class DetailListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<AnimateSetModel> Items { get; private set; }
        public DetailListAdapter(List<AnimateSetModel> items) : base()
        {
            Items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override int ItemCount => Items.Count;

        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            DetailViewHolder vh = holder as DetailViewHolder;
            vh.Name.Text = Items[position].FileName;
            vh.ClickCount.Text = $"Click Count:{Items[position].ClickCount}";
            if (string.IsNullOrEmpty(Items[position].FileThumb))
            {
                vh.Image.SetBackgroundColor(Color.MediumVioletRed);
                vh.Image.SetImageResource(Resource.Drawable.SplashScreen);
            }
            else
            {

                vh.Image.SetImageBitmap(await GetImageBitmapFromUrl(Items[position].FileThumb));
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
            DetailViewHolder vh = new DetailViewHolder(itemView);
            itemView.Click += (s, e) => ItemClick?.Invoke(s, vh.LayoutPosition);
            return vh;
        }
    }
}