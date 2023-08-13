using UnityEngine;

namespace Items
{
   [CreateAssetMenu(menuName = "Items/BulletConfig")]
    public class Bullet : BaseItem
    {
        [SerializeField] private int _damage;
        [SerializeField] private float _velocity;
        [SerializeField] private Sprite _AmmoSprite;

        public Color colorTest;
        private RaycastHit2D _hitPoint;
        public int damage { get => _damage; }
        public float velocity { get => _velocity; }
        public Sprite ammoSprite { get => _AmmoSprite; }
        public Bullet Copy()
        {
            //Only call this when it gets spawned for the first time;
            return Instantiate(this);
        }
        public override ItemType GetItemType()
        {
            return ItemType.Ammo;
        }
        public override Sprite GetSprite()
        {
            return _AmmoSprite;
        }
    } 
}

