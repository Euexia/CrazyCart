using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateManager : MonoBehaviour
{
    CharBaseState _currentState;
    CharStateFactory _states;

    public Camera _camera;

    private Transform _character;
    private Rigidbody _rb;
    private Animator _animator;
    private CharCoroutines _coroutines;

    private Vector3 _direction;
    private Vector3 _inputs = Vector3.zero;
    private bool _isAnimating = false;

    public float _speed = 5f;
    public float _attackDamage = 20;

    public CharBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public Transform Character { get { return _character; } }
    public Rigidbody Rb { get { return _rb; } }
    public Animator Animator { get { return _animator; } }
    public Camera Camera { get { return _camera; } }

    public Vector3 Direction { get { return _direction; } set { _direction = value; } }
    public Vector3 Inputs { get { return _inputs; } }
    public bool IsAnimating { get { return _isAnimating; } set { _isAnimating = value; } }

    public float Speed { get { return _speed; } set { _speed = value; } }

    private void Awake()
    {
        _character = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _coroutines = GetComponent<CharCoroutines>();

        _states = new CharStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }

    void Update()
    {
        GetInputs();
        _currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateState();
        RotateCharacterWithInput();
    }

    private void GetInputs()
    {
        _inputs = Vector3.zero;

        // Assign movement to ZQSD controls
        _inputs.x = Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        _inputs.z = Input.GetKey(KeyCode.Z) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;

        _inputs.Normalize();
        _inputs = IsoVectorConvert(_inputs);
    }

    private Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45f, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }

    public void RotateCharacterWithInput()
    {
        Ray _cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
        Plane _groundPlane = new(Vector3.up, new Vector3(0, _character.position.y, 0));

        if (_groundPlane.Raycast(_cameraRay, out float rayLength))
        {
            Vector3 pointToLook = _cameraRay.GetPoint(rayLength);
            _character.LookAt(new Vector3(pointToLook.x, _character.position.y, pointToLook.z));
        }
    }


    public void MovementAnimation()
    {
        _direction = _character.InverseTransformDirection(_rb.velocity);
        if (_rb.velocity != Vector3.zero)
        {
            if (0 <= _direction.z & _direction.z <= 5)
            {
                _speed = 5f;
                _animator.SetInteger("Direction", 1);
            }
            else if (-5 < _direction.z & _direction.z < 0)
            {
                _speed = 3.5f;
                _animator.SetInteger("Direction", -1);
            }
        }
        else
        {
            _animator.SetInteger("Direction", 0);
        }
    }

    public void AnimationStart()
    {
        _isAnimating = true;
    }

    public void AnimationEnd()
    {
        _isAnimating = false;
        _coroutines.StartCustomCoroutine();
    }

 
}
