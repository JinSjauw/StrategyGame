using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Miscellaneous")]
    public class Miscellaneous : BaseItem
    {
        [SerializeField] private Sprite _sprite;
        public override ItemType GetItemType()
        {
            return ItemType.Miscellaneous;
        }

        public override Sprite GetSprite()
        {
            return _sprite;
        }
    }
}

