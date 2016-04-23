using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace OneEchan.Droid.ViewHolder
{
    internal class ViewHolderBase : RecyclerView.ViewHolder
    {
        internal Dictionary<int, View> Views { get; } = new Dictionary<int, View>();
        internal ViewHolderBase(View itemView, params int[] ids) : base(itemView)
        {
            foreach (var id in ids)
            {
                Views.Add(id, itemView.FindViewById(id));
            }
        }
        internal ViewHolderBase SetText(int id, string text)
        {
            (Views[id] as TextView).Text = text;
            return this;
        }
        internal ViewHolderBase SetBackgroundResource(int id, int res)
        {
            Views[id].SetBackgroundResource(res);
            return this;
        }
        internal ViewHolderBase SetBackgroundColor(int id, Color color)
        {
            Views[id].SetBackgroundColor(color);
            return this;
        }
        internal ViewHolderBase SetImageBitmap(int id, Bitmap bm)
        {
            (Views[id] as ImageView).SetImageBitmap(bm);
            return this;
        }
        internal ViewHolderBase SetTextFormatted(int id, Java.Lang.ICharSequence text)
        {
            (Views[id] as TextView).TextFormatted = text;
            return this;
        }

        internal ViewHolderBase SetImageResource(int id, int res)
        {
            (Views[id] as ImageView).SetImageResource(res);
            return this;
        }
    }
}