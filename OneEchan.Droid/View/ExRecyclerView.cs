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
using Android.Util;

namespace OneEchan.Droid
{

    public class ExRecyclerView : RecyclerView
    {
        public event EventHandler<ScrollEventArgs> Scroll;
        public event EventHandler LoadMore;
        public Adapter ViewAdapter
        {
            get { return GetAdapter(); }
            set { SetAdapter(value); }
        }
        public LayoutManager ViewLayoutManager
        {
            get { return GetLayoutManager(); }
            set { SetLayoutManager(value); }
        }

        public ExRecyclerView(Context context) : base(context)
        {

        }
        public ExRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        public ExRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

        }
        protected ExRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
        public override void OnScrolled(int dx, int dy)
        {
            base.OnScrolled(dx, dy);
            Scroll?.Invoke(this, new ScrollEventArgs(dx, dy));
            if (dy <= 0)
                return;
            var visibleItemCount = ViewLayoutManager.ChildCount;
            var totalItemCount = ViewLayoutManager.ItemCount;
            var pastVisiblesItems = (ViewLayoutManager is LinearLayoutManager ? ViewLayoutManager as LinearLayoutManager : ViewLayoutManager is GridLayoutManager ? ViewLayoutManager as GridLayoutManager : null).FindFirstVisibleItemPosition();
            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount - 3)
                LoadMore?.Invoke(this, new EventArgs());
        }
    }
    public class ScrollEventArgs : EventArgs
    {
        public ScrollEventArgs(int dx,int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }
        public int dx { get; set; }
        public int dy { get; set; }
    }
}