using StarterAssets;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 6.0f;
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 1.0f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    public Vector2 MoveInput;
    public Vector2 MouseInput;

    [Space(10)]
    [Tooltip("The height the _playerTransform can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    private bool _canJump;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    // cinemachine
    private float _cinemachineTargetPitch;

    // _playerTransform
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] Inventory _inventory;
    [SerializeField] CraftingSystem _craft;
    private Pickup nearbyPickup;
    private BoxPickup nearbyBoxPickup;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif

    private PlayerNewInput _input;
    private GameObject _mainCamera;
    
    private int _attackDamage = 20;
    private bool _hasAttacked = false;
    [SerializeField] private int _attackCounter = 0;

    private bool _isInventoryOpen = false;
    private const float _threshold = 0.01f;
    private float lastHitTime = 0;
    private float hitCooldown = 1f;

    private bool _isCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return false;
#endif
        }
    }

    public int AttackCounter { get => _attackCounter; set => _attackCounter = value; }
    public bool IsInventoryOpen { get => _isInventoryOpen; set => _isInventoryOpen = value; }
    public int AttackDamage { get => _attackDamage; set => _attackDamage = value; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _input = new PlayerNewInput();
        _mainCamera = Camera.main.gameObject;
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        _input.Player.Look.performed += ctx => MouseInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += ctx => MouseInput = Vector2.zero;
        _input.Player.Jump.started += _ => _canJump = true;
        _input.Player.Attack.started += _ => Attack();
        _input.Player.Inventory.started += _ => Inventory();
        _input.Player.Interact.started += _ => Interact();
    }
   

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _playerHealth.OnDeath += Death;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        if (_playerHealth.IsAlive)
            Move();
    }

    private void LateUpdate()
    {
        if (_inventory.GetInventory.activeSelf || _craft.GetCraft.activeSelf)
        {
            _isInventoryOpen = true;
            _input.Player.Disable();
            _input.UI.Enable();
            _input.UI.Inventory.started += _ => Inventory();
            _input.UI.Escape.started += _ => Inventory();

            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            _isInventoryOpen = false;
            _input.Player.Enable();
            _input.UI.Disable();

            Cursor.lockState = CursorLockMode.Locked;
        }

        CameraRotation();
    }



    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        // if there is an input
        if (MouseInput.sqrMagnitude >= _threshold)
        {
            // Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = _isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            // Update pitch (up and down rotation)
            _cinemachineTargetPitch += MouseInput.y * RotationSpeed * deltaTimeMultiplier;
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update yaw (left and right rotation)
            _rotationVelocity = MouseInput.x * RotationSpeed * deltaTimeMultiplier;

            // Apply pitch to the camera target
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // Apply yaw to the player transform
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void Move()
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        float targetSpeed = MoveSpeed;

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (MoveInput == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(MoveInput.x, 0.0f, MoveInput.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate _playerTransform when the _playerTransform is moving
        if (MoveInput != Vector2.zero)
        {
            // move
            inputDirection = transform.right * MoveInput.x + transform.forward * MoveInput.y;
        }

        // move the _playerTransform
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_canJump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            _canJump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Attack()
    {
        _attackCounter++;

        if (_attackCounter >= 3)
        {
            _attackCounter = 0;
        }
    }

    public void Inventory()
    {
        if (!_craft.GetCraft.activeSelf)
        {
            _inventory.SetInventory();
        }
        else
        {
            _craft.SetCraft();
        }
    }

    public void Interact()
    {
        if (nearbyPickup != null && nearbyPickup.IsPlayerNearby())
        {
            nearbyPickup.Interact();
        }

        else if (nearbyBoxPickup != null && nearbyBoxPickup.IsPlayerNearby())
        {
            nearbyBoxPickup.Interact();
        }
    }

    private void Death()
    {
        transform.position = _spawnPoint.position;
        StartCoroutine(ResetHealth());
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Pickup>(out Pickup pickup))
        {
            nearbyPickup = pickup;
        }
        else if (other.TryGetComponent<BoxPickup>(out BoxPickup boxPickup))
        {
            nearbyBoxPickup = boxPickup;
        }

        if (other.CompareTag("EnemyHand"))
        {
            if (Time.time - lastHitTime < hitCooldown)
                return;

            _playerHealth.TakeDamage(50);

            lastHitTime = Time.time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Pickup>(out Pickup pickup) && nearbyPickup == pickup)
        {
            nearbyPickup = null;
        }
        else if (other.TryGetComponent<BoxPickup>(out BoxPickup Boxpickup) && nearbyBoxPickup == Boxpickup)
        {
            nearbyBoxPickup = null;
        }
    }

    IEnumerator ResetHealth()
    {
        yield return new WaitForSeconds(1f);
        _playerHealth.ResetHealth();
    }   
}


