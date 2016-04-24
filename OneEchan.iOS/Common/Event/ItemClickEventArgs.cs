using System;
using System.Collections.Generic;
using System.Text;

namespace OneEchan.iOS.Common.Event
{
    public class ItemClickEventArgs<T> : EventArgs
    {
        public T ClickedItem { get; }
        public ItemClickEventArgs(T clickedItem)
        {
            ClickedItem = clickedItem;
        }
    }
}
