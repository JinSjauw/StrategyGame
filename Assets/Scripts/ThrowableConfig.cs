using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Items/ThrowableConfig")]
    public class ThrowableConfig : ScriptableObject
    {
        public Sprite sprite;
        public float fuseTimer;
        public int turnTimer;
        public float radius;
        public int damage;
    }
}