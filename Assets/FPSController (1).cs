using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    // movement
    [Header("Movement")]
    public float groundAccel = 85f;
    public float airAccel = 30f;
    public float maxGroundSpeed = 16f;
    public float maxAirSpeed = 26f;
    public float groundFriction = 10f;

    // jump
    [Header("Jumping")]
    public float jumpForce = 8.5f;
    public float gravity = 30f;
    public float bunnyHopBonus = 1.1f;

    // timingy
    [Header("Timing")]
    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.15f;
    public float slideBufferTime = 0.25f;

    //slide
    [Header("Slide")]
    public float slideSpeed = 24f;
    public float slideDuration = 0.8f;
    public float slideFriction = 2.5f;
    public float slideJumpBoost = 1.15f;
    public float slideCameraOffset = -0.7f;
    public float slideHeight = 1.0f;

    // dash
    [Header("Dash")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    // wall run
    [Header("Wall Run")]
    public float wallRunGravity = 10f;
    public float wallDetectionDistance = 0.8f;

    // kamera
    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.1f;
    public float baseFOV = 75f;
    public float maxFOV = 100f;
    public float cameraLerpSpeed = 12f;

    
    private CharacterController _cc;
    private PlayerInput _input;
    private InputAction _move, _look, _jump, _slide, _dash;

    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;
    private float _pitch;

    private bool _isSliding;
    private float _slideTimer;
    private float _slideBufferTimer;
    private float _defaultHeight;

    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;

    private bool _isWallRunning;
    private float _jumpBufferTimer;
    private float _coyoteTimer;

    private Vector3 _cameraStartLocalPos;
    private Camera _cam;

 
    public float DashCooldownRemaining => _dashCooldownTimer;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _cam = playerCamera.GetComponent<Camera>();

        _cameraStartLocalPos = playerCamera.localPosition;
        _defaultHeight = _cc.height;
    }

    void OnEnable()
    {
        _move = _input.actions.FindAction("Move", true);
        _look = _input.actions.FindAction("Look", true);
        _jump = _input.actions.FindAction("Jump", true);
        _slide = _input.actions.FindAction("Slide", false);
        _dash = _input.actions.FindAction("Dash", false);

        _move.Enable();
        _look.Enable();
        _jump.Enable();
        _slide?.Enable();
        _dash?.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleLook();
        HandleTimers();
        HandleMovement();
        HandleCamera();
    }

    void HandleLook()
    {
        if (Time.timeScale == 0f) return;

        Vector2 look = _look.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * look.x * mouseSensitivity);

        _pitch -= look.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -89f, 89f);
        playerCamera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void HandleTimers()
    {
        if (_jump.triggered) _jumpBufferTimer = jumpBufferTime;
        else _jumpBufferTimer -= Time.deltaTime;

        if (_cc.isGrounded) _coyoteTimer = coyoteTime;
        else _coyoteTimer -= Time.deltaTime;

        if (_slide != null && _slide.triggered) _slideBufferTimer = slideBufferTime;
        else _slideBufferTimer -= Time.deltaTime;

        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        bool grounded = _cc.isGrounded;
        Vector2 input = _move.ReadValue<Vector2>();
        Vector3 wishDir = (transform.right * input.x + transform.forward * input.y).normalized;

        // dash
        if (_dash != null && _dash.triggered && !_isDashing && _dashCooldownTimer <= 0f)
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashCooldownTimer = dashCooldown;
            _horizontalVelocity = (wishDir != Vector3.zero ? wishDir : transform.forward) * dashSpeed;
            _verticalVelocity = 0f;
        }

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f) _isDashing = false;
        }
        else
        {
            if (grounded && _slideBufferTimer > 0f && !_isSliding)
            {
                if (_horizontalVelocity.magnitude > 3f)
                    StartSlide();
            }

            if (_isSliding)
            {
                _slideTimer -= Time.deltaTime;
                _horizontalVelocity *= Mathf.Exp(-slideFriction * Time.deltaTime);

                if (_slideTimer <= 0f || _jump.triggered)
                    EndSlide();
            }
            else
            {
                float accel = grounded ? groundAccel : airAccel;
                float maxSpeed = grounded ? maxGroundSpeed : maxAirSpeed;

                if (wishDir != Vector3.zero)
                {
                    _horizontalVelocity += wishDir * accel * Time.deltaTime;
                    _horizontalVelocity = Vector3.ClampMagnitude(_horizontalVelocity, maxSpeed);
                }

                if (grounded)
                    _horizontalVelocity *= Mathf.Exp(-groundFriction * Time.deltaTime);
            }
        }

        // wall run (je to trosku buggy, na tom zapracovat)
        _isWallRunning = false;
        if (!grounded)
        {
            if (Physics.Raycast(transform.position, transform.right, wallDetectionDistance) ||
                Physics.Raycast(transform.position, -transform.right, wallDetectionDistance))
            {
                _isWallRunning = true;
            }
        }

        // jumpo
        if (_coyoteTimer > 0f && _jumpBufferTimer > 0f)
        {
            _verticalVelocity = jumpForce;
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;

            if (_isSliding)
            {
                _horizontalVelocity *= slideJumpBoost;
                EndSlide();
            }
            else if (_horizontalVelocity.magnitude > maxGroundSpeed)
            {
                _horizontalVelocity *= bunnyHopBonus;
            }
        }

        // gravitace
        float gravityToApply = _isWallRunning ? wallRunGravity : gravity;
        if (!grounded || _verticalVelocity > 0)
            _verticalVelocity -= gravityToApply * Time.deltaTime;
        else
            _verticalVelocity = -2f;

        Vector3 move = _horizontalVelocity + Vector3.up * _verticalVelocity;
        _cc.Move(move * Time.deltaTime);
    }

    void StartSlide()
    {
        _isSliding = true;
        _slideTimer = slideDuration;
        _slideBufferTimer = 0f;
        _cc.height = slideHeight;
        _horizontalVelocity = transform.forward * slideSpeed;
    }

    void EndSlide()
    {
        _isSliding = false;
        _cc.height = _defaultHeight;
    }

    void HandleCamera()
    {
        Vector3 targetPos = _cameraStartLocalPos;
        if (_isSliding) targetPos.y += slideCameraOffset;

        playerCamera.localPosition = Vector3.Lerp(
            playerCamera.localPosition,
            targetPos,
            cameraLerpSpeed * Time.deltaTime
        );

        float currentSpeed = _horizontalVelocity.magnitude;
        float speedPercent = Mathf.Clamp01(currentSpeed / maxAirSpeed);
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedPercent);
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, Time.deltaTime * 5f);
    }
}