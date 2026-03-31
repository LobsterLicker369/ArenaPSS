using UnityEngine;
using UnityEngine.InputSystem;


public class GunAnimator : MonoBehaviour
{


    [Header("Sway")]
    public float swayAmount = 0.02f;
    public float swayMaxAmount = 0.06f;
    public float swaySmoothing = 6f;

    [Header("Bob")]
    public float bobSpeed = 8f;
    public float bobAmountX = 0.003f;
    public float bobAmountY = 0.005f;
    public float bobSmoothing = 8f;

    [Header("Shoot Kick")]
    public float kickAngle = 45f;    
    public float kickSpeed = 20f;
    public float returnSpeed = 8f;

    private Vector3 _initialLocalPosition;
    private Vector3 _currentSway;
    private Vector3 _currentBob;
    private Vector3 _currentKick;
    private float _bobTimer;
    private bool _isKicking;
    private float _kickProgress;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;

    void Awake()
    {
      

        _initialLocalPosition = transform.localPosition;

        
        _playerInput = GetComponentInParent<PlayerInput>();
        if (_playerInput != null)
        {
            _moveAction = _playerInput.actions.FindAction("Move");
            _lookAction = _playerInput.actions.FindAction("Look");
        }
    }

    void Update()
    {
        HandleSway();
        HandleBob();
        HandleKick();

        transform.localPosition = _initialLocalPosition + _currentSway + _currentBob + _currentKick;
    }

   
    public void PlayShoot()
    {
    
        _isKicking = true;
        _kickProgress = 0f;
    }

    void HandleSway()
    {
        if (_lookAction == null) return;

        Vector2 look = _lookAction.ReadValue<Vector2>();

        Vector3 targetSway = new Vector3(
            Mathf.Clamp(-look.x * swayAmount, -swayMaxAmount, swayMaxAmount),
            Mathf.Clamp(-look.y * swayAmount, -swayMaxAmount, swayMaxAmount),
            0f
        );

        _currentSway = Vector3.Lerp(_currentSway, targetSway, Time.deltaTime * swaySmoothing);
    }

    void HandleBob()
    {
        if (_moveAction == null) return;

        Vector2 move = _moveAction.ReadValue<Vector2>();
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            _bobTimer += Time.deltaTime * bobSpeed;
            Vector3 targetBob = new Vector3(
                Mathf.Sin(_bobTimer) * bobAmountX,
                Mathf.Abs(Mathf.Sin(_bobTimer)) * bobAmountY,
                0f
            );
            _currentBob = Vector3.Lerp(_currentBob, targetBob, Time.deltaTime * bobSmoothing);
        }
        else
        {
            _bobTimer = 0f;
            _currentBob = Vector3.Lerp(_currentBob, Vector3.zero, Time.deltaTime * bobSmoothing);
        }
    }

    private float _currentKickAngle;
    private bool _isReturning; 

    void HandleKick()
    {
        if (_isKicking)
        {
         
            _kickProgress += Time.deltaTime * kickSpeed;
            _currentKickAngle = Mathf.Lerp(0f, kickAngle, _kickProgress);
            transform.localRotation = Quaternion.Euler(-_currentKickAngle, 0f, 0f);

            if (_kickProgress >= 1f)
            {
                _isKicking = false;
                _isReturning = true;
                _kickProgress = 0f;
            }
        }
        else if (_isReturning)
        {
          
            _kickProgress += Time.deltaTime * returnSpeed;
            _currentKickAngle = Mathf.Lerp(kickAngle, 0f, _kickProgress);
            transform.localRotation = Quaternion.Euler(-_currentKickAngle, 0f, 0f);

            if (_kickProgress >= 1f)
            {
                _isReturning = false;
                _kickProgress = 0f;
                _currentKickAngle = 0f;
            }
        }
    }
}