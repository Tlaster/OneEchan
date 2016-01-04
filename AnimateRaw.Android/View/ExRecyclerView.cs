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

namespace AnimateRaw.Android
{
    public class ExRecyclerView : RecyclerView
    {
        public event EventHandler<OnScrollEventArgs> OnScroll;
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

        public ExRecyclerView(Context context):base(context)
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
            OnScroll?.Invoke(this, new OnScrollEventArgs(dx, dy));
        }
    }
    public class OnScrollEventArgs : EventArgs
    {
        public OnScrollEventArgs(int dx,int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }
        public int dx { get; set; }
        public int dy { get; set; }
    }
}