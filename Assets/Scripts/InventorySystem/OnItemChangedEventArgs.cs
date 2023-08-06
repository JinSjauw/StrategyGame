using System;
using Items;

namespace InventorySystem
{
    public class OnItemChangedEventArgs : EventArgs
    {
        public ItemContainer item;
    }
}