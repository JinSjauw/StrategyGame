using UnityEngine;

[CreateAssetMenu(menuName = "Items/BulletConfig")]
[System.Serializable]
public class BulletConfig : ScriptableObject
{
    //SFX & VFX variables
    
    //Penetration Value
    //Damage;
    [SerializeField] private int _damage;
    [SerializeField] private float _velocity;
    [SerializeField] private Sprite _bulletSprite;
    
    [SerializeField] private Transform impactEffect;
    
    public Color colorTest;
    
    private RaycastHit2D _hitPoint;
    public int damage { get => _damage; }
    public float velocity { get => _velocity; }
    public Sprite bulletSprite { get => _bulletSprite; }
    
    public BulletConfig Copy()
    {
        //Only call this when it gets spawned for the first time;
        return Instantiate(this);
    }

    public bool DetectCollision(Vector2 currentPosition, Vector2 lastPosition, bool ignore = false)
    {
        //Add combined layerMask
        _hitPoint = Physics2D.Linecast(lastPosition, currentPosition);

        if (!_hitPoint.collider)
        {
            return false;
        }
        
        if (_hitPoint.collider.CompareTag("HalfCover") && ignore)
        {
            Debug.Log("Skipped HalfCover");
            return false;
        }
        
        if (_hitPoint.collider.CompareTag("Obstacles") || _hitPoint.collider.CompareTag("HalfCover") && !ignore)
        {
            Debug.Log("Hit Tag: " + _hitPoint.collider.tag);
            return true;
        }

        if (_hitPoint.collider.CompareTag("UnitHead") || _hitPoint.collider.CompareTag("UnitBody"))
        {
            Debug.Log("HIT UNIT: " + _hitPoint.collider.name);
            IDamageable hitUnit = _hitPoint.collider.GetComponentInParent<IDamageable>();
            hitUnit.TakeDamage(_damage);
            
            return true;
        }
        
        return false;
    }

    public Vector2 Impact()
    {
        //Debug.Log("Playing Impact @: " + _hitPoint.point);
        return _hitPoint.point;
    }
}
