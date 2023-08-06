using Items;
using UnitSystem;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    //Reticle Display
    [SerializeField] private Transform northReticle;
    [SerializeField] private Transform southReticle;
    [SerializeField] private Transform westReticle;
    [SerializeField] private Transform eastReticle;

    [SerializeField] private float reticleOffset;
    
    //From weapon. When you fire in the weapon the bullet wil get an random angle as forward direction.
    //Only representation in the class.
    [SerializeField] private float _accuracy;
    [SerializeField] private float _recoilVerticalRange;
    [SerializeField] private float _recoilHorizontalRange;

    [SerializeField] private float _currentSpread;
    [SerializeField] private float _maxSpread;
    [SerializeField] private float _minSpread;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float reticleReturnSpeed;

    [SerializeField] private AnimationCurve returnCurve;
    [SerializeField] private AnimationCurve reticleReturnCurve;
    

    [SerializeField] private Transform red;
    [SerializeField] private Transform green;
    
    private PlayerUnit _playerUnit;
    
    private Vector2 _center;
    private Vector2 _mousePosition;
    private Vector2 _direction;
    private Vector3 _unitPosition;
    private Vector3 _crosshairPosition;
    
    private float _returnCurrent;
    private float _reticleReturnCurrent;

    private float _deviationDistance;
    private Vector2 _pointA;
    private Vector2 _pointB;

    private void Update()
    {
        if (_playerUnit == null)
        {
            return;
        }
        //transform.position = _center;
        _unitPosition = _playerUnit.transform.position;
        _crosshairPosition = transform.position;
        
        //Lerp back to mousePosition
        Center();
        UpdateReticle();
        
        //Debug Lines
        DebugReticle();
    }

    private void UpdateReticle()
    {
        //Get Distance from unit to mouse position
        float distance = Vector2.Distance(_unitPosition, _mousePosition);

        Quaternion rotationA = Quaternion.AngleAxis(_accuracy / 2, Vector3.forward);
        Quaternion rotationB = Quaternion.AngleAxis(-(_accuracy / 2), Vector3.forward);

         _direction =  _crosshairPosition - _unitPosition;
        
        _pointA = _unitPosition + rotationA * _direction.normalized * distance;
        _pointB = _unitPosition + rotationB * _direction.normalized * distance;
        
        _deviationDistance = Vector2.Distance(_pointA, _pointB);

        _minSpread = _deviationDistance / 2;
        _maxSpread = _minSpread * 3.5f;

        _reticleReturnCurrent = Mathf.MoveTowards(_reticleReturnCurrent, 1, reticleReturnSpeed * Time.deltaTime);

        _currentSpread = Mathf.Lerp(_maxSpread, _minSpread, reticleReturnCurve.Evaluate(_reticleReturnCurrent)) + reticleOffset;

        northReticle.localPosition = new Vector3(0, _currentSpread, 0);
        southReticle.localPosition = new Vector3(0, -_currentSpread, 0);
        
        westReticle.localPosition = new Vector3(_currentSpread, 0, 0);
        eastReticle.localPosition = new Vector3(-_currentSpread, 0, 0);
    }

    private void DebugReticle()
    {
        red.up = _direction.normalized;
        green.up = _direction.normalized;
        
        Vector2 spreadPositionA = ((Vector2)transform.position - _pointA).normalized * _currentSpread;
        Vector2 spreadPositionB = ((Vector2)transform.position - _pointB).normalized * _currentSpread;
        
        red.transform.position = transform.position + (Vector3)spreadPositionA;
        green.transform.position = transform.position + (Vector3)spreadPositionB;
        
        Debug.DrawLine(_playerUnit.transform.position, transform.position, Color.blue);
        Debug.DrawLine(_playerUnit.transform.position, _pointA, Color.red);
        Debug.DrawLine(_playerUnit.transform.position, _pointB, Color.green);
        Debug.DrawRay(_pointA, (_pointB - _pointA) * _deviationDistance, Color.magenta);
    }

    private void Center()
    {
        _returnCurrent = Mathf.MoveTowards(_returnCurrent, 1, returnSpeed * Time.deltaTime);
        transform.position =Vector2.Lerp(_center, _mousePosition, returnCurve.Evaluate(_returnCurrent));
    }

    //Have a equip weapon event and do listen to it. It initializes all relevant variables
    public void Initialize(PlayerUnit playerUnit)
    {
        _playerUnit = playerUnit;
    }

    public void OnEquipWeapon(Weapon weapon)
    {
        _accuracy = weapon.Accuracy;
        _recoilHorizontalRange = weapon.Recoil / 10f;
        _recoilVerticalRange = weapon.Recoil / 10f;
    }
    
    public void GetMousePosition(Vector2 position)
    {
        _mousePosition = position;
    }
    
    //Listen to Weapon.FiredShot
    public void Shoot(Vector2 position)
    {
        _reticleReturnCurrent = 0;
        _returnCurrent = 0;
        
        //Applying basic recoil
        _center = new Vector2(position.x + Random.Range(-_recoilHorizontalRange, _recoilHorizontalRange), position.y + Random.Range(.5f, _recoilVerticalRange));
        //_center = position;
        
        _currentSpread = _maxSpread;
    }
}
