using System;
using System.Collections;
using System.Collections.Generic;
using CustomInput;
using Player;
using UnitSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Mouse Click And Drag
    private bool _mouseMoved;
    private bool _mouseStopped;
    private float _mouseMoveTimer;
    
    private bool _holding;
    private float _holdTimer;

    private bool _followPlayer = false;
    
    private Vector2 _currentMousePosition;
    private Vector2 _lastMousePosition;
    
    private Vector2 _startPoint;
    private Vector2 _endPoint;

    private PlayerUnit _playerUnit;
    
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
        _inputReader.CameraFollowToggle += CameraFollowUnit;
        _inputReader.MoveCameraStartEvent += MoveCameraStart;
        _inputReader.MoveCameraStopEvent += MoveCameraStop;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
        _inputReader.MouseMoveStopEvent += MouseMoveStop;
    }

    private void OnDestroy()
    {
        _inputReader.CenterCameraEvent -= CenterOnUnit;
        _inputReader.MoveCameraStartEvent -= MoveCameraStart;
        _inputReader.MoveCameraStopEvent -= MoveCameraStop;
        _inputReader.MouseMoveStartEvent -= MouseMoveStart;
        _inputReader.MouseMoveStopEvent -= MouseMoveStop;
        _inputReader.CameraFollowToggle -= CameraFollowUnit;
    }

    public void MoveCameraStart()
    {
        Debug.Log($"MOVING CAMERA");
        _holding = true;
        _followPlayer = false;
        _startPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    public void MoveCameraStop()
    {
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
        if (_playerUnit == null)
        {
            _playerUnit = _playerManager.GetCurrentUnit();
            return;
        }
        
        _endPoint = Vector2.zero;
        _startPoint = Vector2.zero;
        _holding = false;
        _mouseMoved = false;
        _mouseStopped = true;
        _cameraFollow.position = _playerUnit.transform.position;
    }
    
    private void CameraFollowUnit()
    {
        Debug.Log($"FOLLOWTOGGLE");
        _followPlayer = true;
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
        if (_followPlayer)
        {
            CenterOnUnit();
        }
        HandleMouseDrag();
        MoveCamera();
    }
}
