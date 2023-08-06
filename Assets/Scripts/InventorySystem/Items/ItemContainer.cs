using System.Collections.Generic;
using InventorySystem.Grid;
using Items;
using UnitSystem;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace InventorySystem
{
    public class ItemContainer : MonoBehaviour
    {
        [SerializeField] private Image _itemRenderer;
        [SerializeField] private Image _backgroundRenderer;
        
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private Sprite backGroundSprite;
        [SerializeField] private int width;
        [SerializeField] private int height;
            
        [SerializeField] private ItemType itemType;
        [SerializeField] private BaseItem itemData;
        
        private List<InventorySlot> _occupiedSlots = new List<InventorySlot>();
        private GridPosition _gridPosition;
        
        private bool rotated = false;
        public RectTransform containerRect { get; private set; }

        public void Initialize(BaseItem item)
        {
            itemData = item;
            itemType = itemData.GetItemType();
            itemSprite = itemData.GetSprite();
    
            width = itemData.GetWidth();
            height = itemData.GetHeight();
            
            containerRect = GetComponent<RectTransform>();

            Vector2 size = new Vector2(width * InventoryGrid.TileSizeWidth, height * InventoryGrid.TileSizeHeight);
            containerRect.sizeDelta = size;
            _itemRenderer.GetComponent<RectTransform>().sizeDelta = size;
            
            _backgroundRenderer.sprite = backGroundSprite;
            _itemRenderer.sprite = itemSprite;

        }

        public void ShowBackground(bool state)
        {
            _backgroundRenderer.enabled = state;
        }
        
        public void SetGridPosition(GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public GridPosition GetGridposition()
        {
            return _gridPosition;
        }
        
        public bool OccupySlot(InventorySlot slot)
        {
            if (!_occupiedSlots.Contains(slot))
            {
                _occupiedSlots.Add(slot);
                slot.OccupySlot(this);
                return true;
            }
            return false;
        }
        
        public void ClearSlots()
        {
            for (int i = 0; i < _occupiedSlots.Count; i++)
            {
                _occupiedSlots[i].ClearSlot();
            }
            _occupiedSlots.Clear();
        }
        
        public BaseItem GetItem() { return itemData; }

        public int GetWidth()
        {
            if (rotated)
            {
                return height;
            }
            return width;
        }

        public int GetHeight()
        {
            if (rotated)
            {
                return width;
            }
            return height;
        }
        
        public void Rotate()
        {
            rotated = !rotated;
            containerRect.rotation = Quaternion.Euler(0, 0, rotated == true ? 90 : 0);
        }
        
        public void Use(Unit unit)
        {
            //Switch depeding on the itemtype
    
            switch (itemType)
            {
                case ItemType.Weapon: 
                    unit.weapon = itemData as Weapon;
                    break;
                case ItemType.Armor:
                    Debug.Log("Equipping Armor");
                    break;
                case ItemType.Consumables:
                    Debug.Log("Using Consumable: ");
                    //Call the consumable.Use() function
                    //Heal, bandage, etc,
                    break;
                //For the rest there is no use functionality
                //May be for attachments?
            }
        }

        
    }
}


