using System;

namespace InventorySystem
{
    public class OnItemChangedEventArgs : EventArgs
    {
        public ItemContainer item;

        public OnItemChangedEventArgs(ItemContainer itemContainer)
        {
            item = itemContainer;
        }
    }
}