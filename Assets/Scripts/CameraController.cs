using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Mouse Click And Drag
    private bool _mouseMoved;
    private bool _mouseStopped;
    private float _mouseMoveTimer;
    
    private bool _holding;
    private float _holdTime = 0.2f;
    private float _holdTimer;

    private Vector2 _currentMousePosition;
    private Vector2 _lastMousePosition;
    
    private Vector2 _startPoint;
    private Vector2 _endPoint;

    private Camera _camera;
    [SerializeField] private float _cameraSmoothTime;
    private Vector2 _refVel;
    
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private float _dragSpeed;

    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private InputReader _inputReader;
    
    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _inputReader.CenterCameraEvent += CenterOnUnit;
        _inputReader.MoveCameraStartEvent += MoveCameraStart;
        _inputReader.MoveCameraStopEvent += MoveCameraStop;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
        _inputReader.MouseMoveStopEvent += MouseMoveStop;
    }
    
    public void MoveCameraStart()
    {
        //Debug.Log("Started " + context.started);
        _holding = true;
        _startPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    public void MoveCameraStop()
    {
        //Debug.Log("Started " + context.started);
        _holding = false;
        _startPoint = Vector2.zero;
        _endPoint = Vector2.zero;
    }
    public void MouseMoveStart()
    {
        _mouseMoved = true;
    }
    public void MouseMoveStop()
    {
        _mouseMoved = false;
    }
    public void CenterOnUnit()
    {
        _endPoint = Vector2.zero;
        _startPoint = Vector2.zero;
        _holding = false;
        _mouseMoved = false;
        _mouseStopped = true;
        _cameraFollow.position = _playerManager.GetCurrentUnit().transform.position;
    }
    private void HandleMouseDrag()
    {
        if (!_mouseMoved)
        {
            _mouseMoveTimer += Time.deltaTime;
            if (_mouseMoveTimer > 0.5f)
            {
                _mouseStopped = true;
                _mouseMoveTimer = 0;
            }
        }
        else
        {
            _mouseStopped = false;
        }
        
        if (_holding && !_mouseStopped)
        {
            //Do drag logic
            _endPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Vector2 direction = (_endPoint - _startPoint);
            Vector3 newPosition = -(Vector3)direction.normalized * direction.magnitude * _dragSpeed * Time.deltaTime;
            _cameraFollow.position += newPosition;
        }
    }

    private void MoveCamera()
    {
        Vector3 tempPosition = Vector2.SmoothDamp(_cameraTransform.position, _cameraFollow.position, ref _refVel, _cameraSmoothTime);
        _cameraTransform.position = tempPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMouseDrag();
        MoveCamera();
    }
}
