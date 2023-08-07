using Items;
using UnityEngine;
using UnityEngine.EventSystems;


namespace InventorySystem
{
    public class EquipmentSlot : MonoBehaviour, IDropHandler
    {
        [SerializeField] private ItemType slotType;

        private ItemContainer _itemContainer;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                RectTransform draggedObject = eventData.pointerDrag.GetComponent<RectTransform>();
                RectTransform equipmentSlot = GetComponent<RectTransform>();

                _itemContainer = draggedObject.GetComponent<ItemContainer>();
                if (_itemContainer.GetItemType() == slotType)
                {
                    draggedObject.position = equipmentSlot.position;
                    draggedObject.SetParent(equipmentSlot.parent);
                    draggedObject.SetAsLastSibling();
                }
            }
        }
    }

}
