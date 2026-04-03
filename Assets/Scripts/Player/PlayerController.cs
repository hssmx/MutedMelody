using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Input;
using MutedMelody.Platforms;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovementData _data;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private NotePlatformManager _platformManager;
        
        [Header("State Data")]
        public PlayerStateData State = new PlayerStateData();

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private InputBuffer _inputBuffer;

        private float _coyoteTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _inputBuffer = InputBuffer.Instance;

            // Prevent unwanted physics rotations
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.Gameplay.Jump.performed += OnJumpPerformed;
                InputManager.Instance.Gameplay.Jump.canceled += OnJumpCanceled;
            }
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.Gameplay.Jump.performed -= OnJumpPerformed;
                InputManager.Instance.Gameplay.Jump.canceled -= OnJumpCanceled;
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _inputBuffer.QueueJump();
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            State.IsJumping = false;
        }
        
        private void Update()
        {
            if (!State.IsAlive) return;

            // Read Movement and Jump Inputs Safely
            if (InputManager.Instance != null)
            {
                State.CurrentMoveInput = InputManager.Instance.Gameplay.Move.ReadValue<Vector2>();

                if (InputManager.Instance.Gameplay.Jump.WasPressedThisFrame())
                {
                    if (InputBuffer.Instance != null) InputBuffer.Instance.QueueJump();
                }

                if (InputManager.Instance.Gameplay.Jump.WasReleasedThisFrame())
                {
                    State.IsJumping = false;
                }
            }

            CheckGrounded();

            // Update coyote timer
            if (State.IsGrounded)
            {
                _coyoteTimer = _data.coyoteTime;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }

            // Implement Jump Execution Logic Safely
            if (InputBuffer.Instance != null && InputBuffer.Instance.ConsumeJump() && (State.IsGrounded || _coyoteTimer > 0f))
            {
                ExecuteJump();
            }
            
            if (InputManager.Instance.Gameplay.SpawnPlatform.WasPressedThisFrame())
            {
                if (_platformManager != null) _platformManager.SpawnPlatform();
            }

            // Implement Sprite Flipping
            if (State.CurrentMoveInput.x > 0) _spriteRenderer.flipX = false;
            else if (State.CurrentMoveInput.x < 0) _spriteRenderer.flipX = true;

            // Update Velocity State
            State.Velocity = _rb.linearVelocity;
        }
        private void FixedUpdate()
        {
            if (!State.IsAlive) return;

            // P03.19: FixedUpdate Structure
            CalculateHorizontalMovement();
            CalculateVariableGravity();
        }

        private void ExecuteJump()
        {
            _coyoteTimer = 0f;
            State.IsJumping = true;

            // Reset Y velocity before jumping so downward momentum doesn't eat the jump force
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _data.jumpForce);
        }

        // P03.20: Implement Horizontal Movement
        private void CalculateHorizontalMovement()
        {
            float targetSpeed = State.CurrentMoveInput.x * _data.maxSpeed;
            float speedDif = targetSpeed - _rb.linearVelocity.x;

            // Calculate acceleration rate based on whether we are speeding up or slowing down
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? (1 / _data.accelerationTime) : (1 / _data.decelerationTime);
            
            float movement = speedDif * accelRate;
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        // P03.21 & P03.22: Variable Gravity & Fall Clamping
        private void CalculateVariableGravity()
        {
            // Falling
            if (_rb.linearVelocity.y < 0)
            {
                _rb.gravityScale = _data.gravityScale * _data.fallMultiplier;
                
                // P03.22: Fall Speed Clamping
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, _data.maxFallSpeed));
            }
            // Rising + Jump Released (Variable Jump Height)
            else if (_rb.linearVelocity.y > 0 && !State.IsJumping)
            {
                _rb.gravityScale = _data.gravityScale * _data.jumpCutMultiplier;
            }
            // Rising + Jump Held (Full Jump)
            else
            {
                _rb.gravityScale = _data.gravityScale;
            }
        }

        private void CheckGrounded()
        {
            Vector2 boxCenter = (Vector2)_collider.bounds.center + Vector2.down * (_collider.bounds.extents.y - 0.05f);
            Vector2 boxSize = new Vector2(_collider.bounds.size.x * 0.9f, 0.1f);
            
            State.IsGrounded = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0.1f, _groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider2D>();
            if (_collider != null)
            {
                Gizmos.color = Color.red;
                Vector2 boxCenter = (Vector2)_collider.bounds.center + Vector2.down * (_collider.bounds.extents.y - 0.05f);
                Vector2 boxSize = new Vector2(_collider.bounds.size.x * 0.9f, 0.1f);
                Gizmos.DrawWireCube(boxCenter + Vector2.down * 0.1f, boxSize);
            }
        }
    }
}