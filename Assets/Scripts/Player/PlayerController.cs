using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Input;
using MutedMelody.Platforms;
using MutedMelody.Core;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovementData _movementData;
        [SerializeField] private InputBuffer _inputBuffer;
        [SerializeField] private NotePlatformManager _platformManager;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundLayer;

        [Header("Debug")]
        [SerializeField] private bool _drawGroundCheck = true;
        [SerializeField] private float _groundCheckRadius = 0.15f;

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private InputManager _inputManager;
        private InputManager _subscribedInputManager;
        private bool _callbacksRegistered;

        private float _moveInput;
        private bool _jumpHeld;
        private bool _isGrounded;
        private float _lastGroundedTime;
        private float _invincibleTimer;
        private float _localQueuedJumpUntil = -1f;

        public bool IsGrounded => _isGrounded;
        public bool IsFacingRight { get; private set; } = true;
        private int _movementLocks = 0;
        public bool IsMovementLocked => _movementLocks > 0;
        
        public void AddMovementLock() => _movementLocks++;
        public void RemoveMovementLock() => _movementLocks = Mathf.Max(0, _movementLocks - 1);

        public bool IsInvincible { get; private set; }
        private float _invincibilityTimer;
        
        public Rigidbody2D Body => _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_movementData == null)
            {
                Debug.LogError("[PlayerController] Missing PlayerMovementData reference.", this);
                enabled = false;
                return;
            }

            _rb.gravityScale = _movementData.gravityScale;
        }

        private void OnEnable()
        {
            TryRegisterInputCallbacks();
        }

        private void OnDisable()
        {
            UnregisterInputCallbacks();
        }

        private void Update()
        {
            if (!_callbacksRegistered)
            {
                TryRegisterInputCallbacks();
            }

            if (_inputBuffer == null)
            {
                Singleton<InputBuffer>.TryGetInstance(out _inputBuffer);
            }

            if (_inputManager == null)
            {
                Singleton<InputManager>.TryGetInstance(out _inputManager);
            }

            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0f)
                {
                    IsInvincible = false;
                }
            }

            UpdateGroundedState();

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }

            _moveInput = (IsMovementLocked || _inputManager == null)
                ? 0f
                : _inputManager.Gameplay.Move.ReadValue<Vector2>().x;

            UpdateFacing();
        }

        private void FixedUpdate()
        {
            if (_movementData == null)
                return;

            ApplyHorizontalMovement();
            HandleBufferedJump();
            ApplyBetterJumpGravity();
            ClampFallSpeed();
        }

        public void SetInvincible(float duration)
        {
            IsInvincible = true;
            _invincibilityTimer = duration;
        }


        public void ForceStopHorizontal()
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        }

        private void TryRegisterInputCallbacks()
        {
            if (_callbacksRegistered)
                return;

            if (!Singleton<InputManager>.TryGetInstance(out var manager))
                return;

            manager.Gameplay.Jump.performed += OnJumpPerformed;
            manager.Gameplay.Jump.canceled += OnJumpCanceled;
            manager.Gameplay.SpawnPlatform.performed += OnPlatformSpawnPerformed;

            _inputManager = manager;
            _subscribedInputManager = manager;
            _callbacksRegistered = true;
        }

        private void UnregisterInputCallbacks()
        {
            if (!_callbacksRegistered || _subscribedInputManager == null)
            {
                _callbacksRegistered = false;
                _subscribedInputManager = null;
                return;
            }

            _subscribedInputManager.Gameplay.Jump.performed -= OnJumpPerformed;
            _subscribedInputManager.Gameplay.Jump.canceled -= OnJumpCanceled;
            _subscribedInputManager.Gameplay.SpawnPlatform.performed -= OnPlatformSpawnPerformed;

            _callbacksRegistered = false;
            _subscribedInputManager = null;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpHeld = true;

            if (_inputBuffer != null)
            {
                _inputBuffer.QueueJump();
            }
            else
            {
                _localQueuedJumpUntil = Time.time + _movementData.jumpBufferTime;
            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _jumpHeld = false;
        }

        private void OnPlatformSpawnPerformed(InputAction.CallbackContext context)
        {
            if (IsMovementLocked)
                return;

            _platformManager?.SpawnPlatform();
        }

        private void ApplyHorizontalMovement()
        {
            float targetSpeed = _moveInput * _movementData.maxSpeed;
            float rampTime = Mathf.Abs(targetSpeed) > 0.01f ? _movementData.accelerationTime : _movementData.decelerationTime;
            float accelerationRate = rampTime > 0.0001f
                ? _movementData.maxSpeed / rampTime
                : float.PositiveInfinity;

            float newVelocityX = Mathf.MoveTowards(_rb.linearVelocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(newVelocityX, _rb.linearVelocity.y);
        }

        private void HandleBufferedJump()
        {
            bool shouldJump = false;

            if (_inputBuffer != null)
            {
                shouldJump = _inputBuffer.ConsumeJump();
            }
            else if (_localQueuedJumpUntil >= Time.time)
            {
                shouldJump = true;
                _localQueuedJumpUntil = -1f;
            }

            if (!shouldJump)
                return;

            bool canUseCoyoteTime = (Time.time - _lastGroundedTime) <= _movementData.coyoteTime;
            if (!_isGrounded && !canUseCoyoteTime)
                return;

            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _movementData.jumpForce);
            _isGrounded = false;
        }

        private void ApplyBetterJumpGravity()
        {
            Vector2 velocity = _rb.linearVelocity;

            if (velocity.y > 0.01f)
            {
                float multiplier = _jumpHeld ? 1f : _movementData.jumpCutMultiplier;
                velocity.y += Physics2D.gravity.y * _movementData.gravityScale * (multiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (velocity.y < -0.01f)
            {
                velocity.y += Physics2D.gravity.y * _movementData.gravityScale * (_movementData.fallMultiplier - 1f) * Time.fixedDeltaTime;
            }

            _rb.linearVelocity = velocity;
        }

        private void ClampFallSpeed()
        {
            Vector2 velocity = _rb.linearVelocity;
            float maxFallMagnitude = Mathf.Abs(_movementData.maxFallSpeed);

            if (velocity.y < -maxFallMagnitude)
            {
                velocity.y = -maxFallMagnitude;
                _rb.linearVelocity = velocity;
            }
        }

        private void UpdateGroundedState()
        {
            if (_groundCheck == null)
            {
                _isGrounded = false;
                return;
            }

            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        }

        private void UpdateFacing()
        {
            if (_moveInput > 0.01f)
            {
                IsFacingRight = true;
            }
            else if (_moveInput < -0.01f)
            {
                IsFacingRight = false;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = !IsFacingRight;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGroundCheck || _groundCheck == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}
