using Items;
using UnitSystem;
using UnityEngine;

namespace InventorySystem
{
    public class ItemContainer : MonoBehaviour
    {
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private int width;
        [SerializeField] private int height;
            
        [SerializeField] private ItemType itemType;
        [SerializeField] private BaseItem itemData;
    
        private SpriteRenderer _spriteRenderer;
    
        private void Awake()
        {
            Initialize();
        }
    
        public void Initialize()
        {
            itemType = itemData.GetItemType();
            itemSprite = itemData.GetSprite();
    
            width = itemData.GetWidth();
            height = itemData.GetHeight();
    
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = itemSprite;
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


