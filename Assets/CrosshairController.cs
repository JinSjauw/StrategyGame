using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    //Reticle Display
    [SerializeField] private Transform northReticle;
    [SerializeField] private Transform southReticle;
    [SerializeField] private Transform westReticle;
    [SerializeField] private Transform eastReticle;
    
    //Variables
    [SerializeField] private Unit unit;

    [SerializeField] private float _accuracy;
    [SerializeField] private float _currentSpread;
    [SerializeField] private float _maxSpread;
    [SerializeField] private float _minSpread;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float reticleReturnSpeed;
    
    [SerializeField] private AnimationCurve returnCurve;
    [SerializeField] private AnimationCurve reticleReturnCurve;
    
    [SerializeField] private Vector2 _center;
    
    private Vector2 _mousePosition;
    
    private float _returnCurrent;
    private float _reticleReturnCurrent;
    
    public event UnityAction CrosshairChanged = delegate {  };

    private void Update()
    {
        //transform.position = _center;
        
        //Lerp back to mousePosition
        Center();
        UpdateReticle();
    }

    private void UpdateReticle()
    {
        //Have the reticle open/close based on radius
        //Go back to _minSpread via lerping
        //Draw Lines representing angles

        //Get Distance from unit to mouse position
        //Draw 2 angled line vector (angle = accuracy / 2)
        //Use distance between the 2 angled vectors as a radius for reticle and spread.
        //This way it increases the further you go.
        //Maybe only do this at a certain distance?

        _reticleReturnCurrent = Mathf.MoveTowards(_reticleReturnCurrent, 1, reticleReturnSpeed * Time.deltaTime);

        _currentSpread = Mathf.Lerp(_maxSpread, _minSpread, reticleReturnCurve.Evaluate(_reticleReturnCurrent));
        
        northReticle.localPosition = new Vector3(0, _currentSpread, 0);
        southReticle.localPosition = new Vector3(0, -_currentSpread, 0);
        
        westReticle.localPosition = new Vector3(_currentSpread, 0, 0);
        eastReticle.localPosition = new Vector3(-_currentSpread, 0, 0);
    }

    private void Center()
    {
        _returnCurrent = Mathf.MoveTowards(_returnCurrent, 1, returnSpeed * Time.deltaTime);
        transform.position =Vector2.Lerp(_center, _mousePosition, returnCurve.Evaluate(_returnCurrent));
    }
    
    //Have a equip weapon event and do listen to it. It initializes all relevant variables
    
    public void GetMousePosition(Vector2 position)
    {
        _mousePosition = position;
    }
    
    public void UpdatePosition(Vector2 position)
    {
        _reticleReturnCurrent = 0;
        _returnCurrent = 0;
        _center = position;
        
        _currentSpread = _maxSpread;
        
        CrosshairChanged.Invoke();
    }
}
