using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Input;
using MutedMelody.Platforms;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovementData _movementData;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private InputBuffer _inputBuffer;
        [SerializeField] private NotePlatformManager _platformManager;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundLayer;

        [Header("Debug")]
        [SerializeField] private bool _drawGroundCheck = true;

        private Rigidbody2D _rb;
        private float _moveInput;
        private bool _isGrounded;
        private bool _jumpHeld;
        private float _lastGroundedTime;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_movementData == null)
            {
                Debug.LogError("[PlayerController] Missing PlayerMovementData reference.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_inputManager == null)
            {
                return;
            }

            _inputManager.JumpPerformed += OnJumpPerformed;
            _inputManager.JumpCanceled += OnJumpCanceled;
            _inputManager.PlatformSpawnPerformed += OnPlatformSpawnPerformed;
        }

        private void OnDisable()
        {
            if (_inputManager == null)
            {
                return;
            }

            _inputManager.JumpPerformed -= OnJumpPerformed;
            _inputManager.JumpCanceled -= OnJumpCanceled;
            _inputManager.PlatformSpawnPerformed -= OnPlatformSpawnPerformed;
        }

        private void Update()
        {
            if (_inputManager == null)
            {
                return;
            }

            _moveInput = _inputManager.MoveInput.x;
            UpdateGroundedState();
            UpdateFacing();

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            ApplyHorizontalMovement();
            HandleBufferedJump();
            ApplyBetterJumpGravity();
            ClampFallSpeed();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpHeld = true;

            if (_inputBuffer != null)
            {
                _inputBuffer.BufferJump();
            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _jumpHeld = false;
        }

        private void OnPlatformSpawnPerformed(InputAction.CallbackContext context)
        {
            if (_platformManager == null)
            {
                return;
            }

            _platformManager.SpawnPlatform();
        }

        private void ApplyHorizontalMovement()
        {
            float targetSpeed = _moveInput * _movementData.moveSpeed;
            float accel = Mathf.Abs(targetSpeed) > 0.01f ? _movementData.acceleration : _movementData.deceleration;
            float newVelocityX = Mathf.MoveTowards(_rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);

            _rb.linearVelocity = new Vector2(newVelocityX, _rb.linearVelocity.y);
        }

        private void HandleBufferedJump()
        {
            if (_inputBuffer == null)
            {
                return;
            }

            if (!_inputBuffer.HasBufferedJump())
            {
                return;
            }

            bool canUseCoyoteTime = (Time.time - _lastGroundedTime) <= _movementData.coyoteTime;

            if (_isGrounded || canUseCoyoteTime)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _movementData.jumpForce);
                _isGrounded = false;
                _inputBuffer.ConsumeJump();
            }
        }

        private void ApplyBetterJumpGravity()
        {
            Vector2 velocity = _rb.linearVelocity;

            if (velocity.y > 0.01f)
            {
                float gravityMultiplier = _jumpHeld ? _movementData.ascendGravityMultiplier : _movementData.jumpCutGravityMultiplier;
                velocity.y += Physics2D.gravity.y * (gravityMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (velocity.y < -0.01f)
            {
                velocity.y += Physics2D.gravity.y * (_movementData.fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
            }

            _rb.linearVelocity = velocity;
        }

        private void ClampFallSpeed()
        {
            Vector2 velocity = _rb.linearVelocity;

            if (velocity.y < -_movementData.maxFallSpeed)
            {
                velocity.y = -_movementData.maxFallSpeed;
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

            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _movementData.groundCheckRadius, _groundLayer);
        }

        private void UpdateFacing()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            if (_moveInput > 0.01f)
            {
                _spriteRenderer.flipX = false;
            }
            else if (_moveInput < -0.01f)
            {
                _spriteRenderer.flipX = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGroundCheck || _groundCheck == null || _movementData == null)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_groundCheck.position, _movementData.groundCheckRadius);
        }
    }
}
