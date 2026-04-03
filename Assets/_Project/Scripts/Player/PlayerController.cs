using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.InputSystem;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References (P03.09)")]
        [SerializeField] private PlayerMovementData _data;
        [SerializeField] private LayerMask _groundLayer; // Required for P03.14

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private InputBuffer _inputBuffer;

        [Header("State Flags (P03.09)")]
        [SerializeField, ReadOnly] private bool _isGrounded;
        private bool _isJumping;
        private float _coyoteTimer;
        private Vector2 _moveInput;

        // P03.10: Implement Awake()
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _inputBuffer = InputBuffer.Instance; 
        }

        // P03.11: Subscribe to Jump Input Events
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

        // P03.12: Implement OnJumpPerformed
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _inputBuffer.QueueJump();
        }

        // P03.13: Implement OnJumpCanceled
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _isJumping = false;
        }

        private void Update()
        {
            CheckGrounded();
        }

        // P03.14: Implement Ground Detection
        private void CheckGrounded()
        {
            // Calculate a box slightly smaller than the collider width, positioned at the bottom edge
            Vector2 boxCenter = (Vector2)_collider.bounds.center + Vector2.down * (_collider.bounds.extents.y - 0.05f);
            Vector2 boxSize = new Vector2(_collider.bounds.size.x * 0.9f, 0.1f);
            
            _isGrounded = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0.1f, _groundLayer);
        }

        // Bonus: Visually draw the BoxCast in the Editor so you can debug the exact ground detection zone
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

    // Helper attribute to see states in the inspector without allowing edits
    public class ReadOnlyAttribute : PropertyAttribute { }
}
