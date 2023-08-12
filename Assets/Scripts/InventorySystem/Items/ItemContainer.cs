using System.Collections.Generic;
using InventorySystem.Grid;
using Items;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace InventorySystem
{
    public class ItemContainer : MonoBehaviour
    {
        [SerializeField] private Image _itemRenderer;
        [SerializeField] private Image _backgroundRenderer;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private RectTransform _containerRect;
        [SerializeField] private RectTransform _iconContainerRect;
        
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private Sprite backGroundSprite;
        [SerializeField] private int width;
        [SerializeField] private int height;
            
        [SerializeField] private ItemType itemType;
        [SerializeField] private BaseItem itemData;
        
        
        private List<InventorySlot> _occupiedSlots = new List<InventorySlot>();
        private GridPosition _itemPosition;
        private bool _rotated = false;
        private int _amount = 1;
        
        public RectTransform containerRect { get => _containerRect; }
        
        public void Initialize(BaseItem item)
        {
            if (item == null) { return; }
            
            itemData = item;
            itemType = item.GetItemType();
            itemSprite = item.GetSprite();
            
            _amount = item.GetAmount();
            _amountText.text = _amount.ToString();
            
            width = item.GetWidth();
            height = item.GetHeight();
            
            _itemPosition = item.GetItemPosition();
            _rotated = item.IsRotated();
            Vector2 rotatedSize = new Vector2(GetWidth() * InventoryGrid.TileSizeWidth, GetHeight() * InventoryGrid.TileSizeHeight);
            Vector2 size = new Vector2(width * InventoryGrid.TileSizeWidth, height * InventoryGrid.TileSizeHeight);
            _containerRect.sizeDelta = rotatedSize;
            _itemRenderer.GetComponent<RectTransform>().sizeDelta = size;
            _iconContainerRect.sizeDelta = size;
            _iconContainerRect.rotation = Quaternion.Euler(0, 0, _rotated ? 90 : 0);

            _backgroundRenderer.sprite = backGroundSprite;
            _itemRenderer.sprite = itemSprite;
        }
        public void ShowBackground(bool state)
        {
            _backgroundRenderer.enabled = state;
        }
        public void SetGridPosition(GridPosition gridPosition)
        {
            _itemPosition = gridPosition;
            itemData.SetItemPosition(gridPosition);
        }
        public GridPosition GetGridposition()
        {
            return _itemPosition;
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
            if (_rotated)
            {
                return height;
            }
            return width;
        }
        public int GetHeight()
        {
            if (_rotated)
            {
                return width;
            }
            return height;
        }
        
        public int GetAmount()
        {
            _amount = itemData.GetAmount();
            _amountText.text = _amount.ToString();
            return _amount;
        }
        
        public void AddAmount(int amount)
        {
            _amount += amount;
            itemData.SetAmount(_amount);
            _amountText.text = _amount.ToString();
        }
        
        public void Rotate()
        {
            _rotated = !_rotated;
            itemData.Rotate(_rotated);
            _containerRect.sizeDelta = new Vector2(GetWidth() * InventoryGrid.TileSizeWidth, GetHeight() * InventoryGrid.TileSizeHeight);
            _iconContainerRect.rotation = Quaternion.Euler(0, 0, _rotated ? 90 : 0);
        }
        public ItemType GetItemType()
        {
            return itemType;
        }
        
        /*public void Use(Unit unit)
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
        }*/
       
    }
}


